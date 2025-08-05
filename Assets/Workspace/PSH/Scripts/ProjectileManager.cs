using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviourPun
{
    public static ProjectileManager Instance { get; private set; }

    [SerializeField] Texture2D explosionMask;

    private FloatingTextSpawner floatingTextSpawner;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        floatingTextSpawner = GetComponent<FloatingTextSpawner>();
    }
    [PunRPC]
    public void RPC_RequestFireProjectile(Vector3 firePointPosition, Quaternion firePointRotation, float powerCharge, bool onDamageBuff, object[] damageBuffArray, int ownerActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // 포탄 생성
        GameObject bullet = PhotonNetwork.Instantiate("Prefabs/Projectile", firePointPosition, firePointRotation);
        Projectile bulletScript = bullet.GetComponent<Projectile>();

        EffectSpawner.Instance.SpawnFire(firePointPosition, firePointRotation);

        // 데미지 버프 적용
        if (onDamageBuff)
        {
            List<object> damageBuffList = new List<object>(damageBuffArray);
            bulletScript.ApplyDamageBuff(damageBuffList);
        }

        // 발사 파워 적용
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = firePointRotation * Vector2.up * powerCharge;
        }

        // 포탄의 소유자 설정 (옵션: Projectile 스크립트에서 사용 가능)
        bulletScript.SetOwnerActorNumber(ownerActorNumber);

        // 모든 클라이언트에 포탄 ViewID 동기화 (카메라 타겟용)
        PhotonView bulletPhotonView = bullet.GetComponent<PhotonView>();
        photonView.RPC(nameof(RPC_SetBulletTarget), RpcTarget.All, bulletPhotonView.ViewID);
    }

    [PunRPC]
    private void RPC_SetBulletTarget(int bulletViewID)
    {
        PhotonView bulletView = PhotonView.Find(bulletViewID);
        if (bulletView != null)
        {
            CameraController.Instance.vcamBullet.Follow = bulletView.transform;
            CameraController.Instance.vcamBullet.Priority = 20;
            Debug.Log($"[ProjectileManager] 카메라 타겟을 ViewID {bulletViewID}인 포탄으로 설정");
        }
    }

    // 지형 파괴 및 데미지 적용 결과를 동기화하는 RPC (Projectile.cs에서 호출)
    [PunRPC]
    public void RPC_ApplyExplosionEffects(Vector2 explosionPoint, int explosionRadiusX, int explosionRadiusY,
        float explosionScale, int bulletViewID, int[] hitPlayerActorNumbers, int realDamage)
    {
        // 지형 파괴
        DeformableTerrain terrain = FindObjectOfType<DeformableTerrain>();
        if (terrain != null)
        {
            if (explosionRadiusX > 0 && explosionRadiusY > 0) // 원형/타원형 파괴
            {
                terrain.DestroyTerrain(explosionPoint, explosionRadiusX, explosionRadiusY);
            }
            //TODO : 이거 적용시킬 조건 우선은 포탄 explosionRadius0으로 하면 될듯
            else if (explosionMask != null) // 마스크 파괴
            {
                terrain.DestroyTerrain(explosionPoint, explosionMask, explosionScale);
            }
        }

        // 플레이어 데미지 적용 (각 클라이언트에서 해당 플레이어에게 데미지 적용)
        foreach (int actorNumber in hitPlayerActorNumbers)
        {
            PlayerController player = FindObjectOfType<MSKTurnController>().GetPlayerController(actorNumber);
            if (player != null)
            {
                // 피격 이펙트 (각 클라이언트에서 생성)
                if (EffectSpawner.Instance != null)
                {
                    EffectSpawner.Instance.SpawnExplosion(player.transform.position);
                    string str = realDamage.ToString();
                    floatingTextSpawner.SpawnText(str, player.transform.position);
                }
                player.OnHit(realDamage);
                Debug.Log($"[ProjectileManager] 플레이어 {actorNumber}에게 {realDamage} 데미지 적용");
            }
        }

        // 포탄 파괴 (모든 클라이언트에서 해당 포탄 파괴)
        PhotonView bulletView = PhotonView.Find(bulletViewID);
        if (bulletView != null)
        {
            Projectile bulletScript = bulletView.GetComponent<Projectile>();
            if (bulletScript != null)
            {
                bulletScript.BeginDestroyRoutine(true); // 폭발 이펙트와 함께 파괴
            }
        }
    }
}

using Game;
using ParrelSync.NonCore;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviourPun
{
    public int explosionRadiusx = 100; // 텍스처 픽셀 단위
    public int explosionRadiusy = 100;
    public Texture2D explosionMask;
    public float explosionScale = 1f;
    public int damage = 50;
    private int realDamage;

    private float worldPerPixel; // Terrain 기준
    private DeformableTerrain terrain;

    private bool hasCollided = false;
    private int ownerActorNumber; // 포탄을 발사한 플레이어의 ActorNumber

    [SerializeField] GameObject explosionEffect;
    [SerializeField] float delay = 2f;

    private Rigidbody2D rb;

    private bool isTeamDamage;//팀킬가능한지
    private Game.Team myTeam;

    [Header("기즈모")]
    private Vector2 gizmoCenter;
    private float gizmoRadius;

    [Header("바람")]
    [SerializeField] private float windEffectMultiplier = 1f; // 바람의 영향을 받는 정도

    private Vector2 windForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Projectile에 Rigidbody2D 컴포넌트가 없습니다!");
        }
    }

    private void Start()
    {
        terrain = FindObjectOfType<DeformableTerrain>();

        // Terrain의 스프라이트/텍스처 기준으로 계산
        var sr = terrain.GetComponent<SpriteRenderer>();
        var tex = sr.sprite.texture;
        worldPerPixel = sr.bounds.size.x / tex.width;

        Debug.Log($"worldPerPixel = {worldPerPixel}");

        // 마스터 클라이언트에서만 팀킬 여부와 팀 정보를 설정
        if (PhotonNetwork.IsMasterClient)
        {
            isTeamDamage = PhotonNetwork.CurrentRoom.GetDamageType();
            myTeam = PhotonNetwork.LocalPlayer.GetTeam(); // 마스터 클라이언트의 팀
        }
        realDamage = damage;

        // 바람 정보 저장
        if (WindManager.Instance != null)
        {
            windForce = WindManager.Instance.CurrentWind;
        }
    }

    // 바람
    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (rb != null)
            {
                rb.AddForce(windForce * windEffectMultiplier, ForceMode2D.Force);
            }
        }
    }
    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        // 속도가 0에 가까울 정도로 작지 않을 때만 방향을 업데이트합니다.
        if (rb != null && rb.velocity.sqrMagnitude > 0.01f)
        {
            // 속도 벡터의 방향을 각도로 변환합니다.
            // Atan2는 y, x 순서로 인자를 받습니다.
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

            // Z축을 기준으로 회전하는 쿼터니언을 생성합니다.
            // 이 코드는 스프라이트가 기본적으로 오른쪽(→)을 향하고 있을 때를 가정합니다.
            // 만약 스프라이트가 위쪽(↑)을 향하고 있다면 'angle - 90f'로 수정해야 합니다.
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!PhotonNetwork.IsMasterClient) return; // 마스터 클라이언트에서만 충돌 처리

        Vector2 explosionPoint = collision.contacts[0].point;

        // 데미지 처리
        float pixelRadius = Mathf.Max(explosionRadiusx, explosionRadiusy);
        float worldRadius = pixelRadius * worldPerPixel;

        List<int> hitPlayerActorNumbers = new List<int>();
        var colliders = Physics2D.OverlapCircleAll(explosionPoint, worldRadius);

        foreach (var hit in colliders)
        {
            if (!hit.CompareTag("Player"))
                continue;

            var pv = hit.GetComponent<PhotonView>();
            if (pv == null) continue;

            Game.Team otherTeam = pv.Owner.GetTeam();
            // 포탄을 발사한 플레이어의 팀 정보를 가져와서 비교
            Game.Team projectileOwnerTeam = PhotonNetwork.CurrentRoom.GetPlayer(ownerActorNumber).GetTeam();

            if (!isTeamDamage && otherTeam == projectileOwnerTeam)
            {
                Debug.Log("아군이다 사격 중지!");
                continue;
            }
            hitPlayerActorNumbers.Add(pv.Owner.ActorNumber);
        }

        // ProjectileManager를 통해 모든 클라이언트에 폭발 효과 동기화
        ProjectileManager.Instance.photonView.RPC(nameof(ProjectileManager.RPC_ApplyExplosionEffects), RpcTarget.All,
            explosionPoint, explosionRadiusx, explosionRadiusy, explosionScale,
            photonView.ViewID, hitPlayerActorNumbers.ToArray(), realDamage);

        // 마스터 클라이언트에서는 즉시 파괴 루틴 시작
        BeginDestroyRoutine(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient) return; // 마스터 클라이언트에서만 충돌 처리

        if (collision.CompareTag("MapBoundary"))
        {
            if (!hasCollided)
            {
                Debug.Log("포탄이 맵 밖으로 나감");
                // ProjectileManager를 통해 모든 클라이언트에 포탄 파괴 동기화
                ProjectileManager.Instance.photonView.RPC(nameof(ProjectileManager.RPC_ApplyExplosionEffects), RpcTarget.All,
                    Vector2.zero, 0, 0, 0f, // 지형 파괴 없음
                    photonView.ViewID, new int[0], 0); // 데미지 없음

                BeginDestroyRoutine(false);
            }
        }
    }

    public void SetOwnerActorNumber(int actorNumber)
    {
        ownerActorNumber = actorNumber;
    }

    public void BeginDestroyRoutine(bool hasExplosionEffect)
    {
        if (hasCollided) return;
        hasCollided = true;
        StartCoroutine(DestroyRoutine(hasExplosionEffect));
    }
    private IEnumerator DestroyRoutine(bool hasExplosionEffect)
    {
        //투사체 비활성화
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        //폭발이펙트
        if (explosionEffect != null && hasExplosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);


        //몇초후 카메라 무브
        yield return new WaitForSeconds(delay);

        if (CameraController.Instance != null)
            CameraController.Instance.ReturnToPlayerCam();

        Destroy(gameObject);
    }

    public void ApplyDamageBuff(List<object> DamageBuff)
    {
        float Fixed = 0;
        float Ratio = 1;
        if (DamageBuff[0] != null)
        {
            Fixed = (float)DamageBuff[0];
        }
        if (DamageBuff[1] != null)
        {
            Ratio = (float)DamageBuff[1];
        }
        realDamage = (int)(damage * Ratio + Fixed);
    }
}

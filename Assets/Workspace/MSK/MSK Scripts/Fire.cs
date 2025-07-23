using Photon.Pun;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePivot;       // 회전할 포신 부분
    [SerializeField] private Transform firePoint;       // 실제 폭탄이 나갈 위치
    [SerializeField] private GameObject bombPrefab;

    [Header("Controls")]
    [SerializeField] private float angle = 45f;         // 포신의 현재 각도
    [SerializeField] private float angleStep = 0.5f;      // 각도 변화량
    [SerializeField] private float maxPower = 10f;         // 폭탄 발사 속도
    [SerializeField] private float chargingSpeed = 10f;    // 차지속도

    private float powerCharge = 0f;         // 차지
    private bool isCharging = false;        // 차지 중인지 여부

    private void Update()
    {
        // 각도 조절 (Up/Down)
        if (Input.GetKey(KeyCode.UpArrow))
        {
            angle += angleStep;
            if (angle > 90f) angle = 90f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            angle -= angleStep;
            if (angle < 0f) angle = 0f;
        }

        // 포신 회전 
        firePivot.localRotation = Quaternion.Euler(0, 0, angle);


        // 스페이스바 누르고 있으면 차지 시작
        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            Debug.Log("차지");
            powerCharge += chargingSpeed * Time.deltaTime;
            powerCharge = Mathf.Clamp(powerCharge, 0f, maxPower);
        }

        // 스페이스바에서 손을 뗐을 때 발사
        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log($"발사, 힘 : {powerCharge}");
            Shoot();
            powerCharge = 0f;
            isCharging = false;
        }
        Debug.DrawRay(firePoint.position, firePoint.up * 2f, Color.red);
    }

    // 폭탄 생성하고 속도 설정
    private void Shoot()
    {
        //Bomb newBomb = PhotonNetwork.Instantiate(bombPrefab, firePoint.position, Quaternion.identity);
        //newBomb.SetVelocity(firePoint.up * powerCharge);
    }
}

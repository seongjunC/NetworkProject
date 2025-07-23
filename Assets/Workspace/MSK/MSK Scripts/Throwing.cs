using UnityEngine;

public class Throwing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform firePivot;       // 회전할 포신 부분 (예: FirePivot)
    [SerializeField] Transform firePoint;       // 실제 폭탄이 나갈 위치
    [SerializeField] Bomb bombPrefab;

    [Header("Controls")]
    [SerializeField] float angle = 45f;         // 현재 각도
    [SerializeField] float angleStep = 1f;      // 각도 변화량
    [SerializeField] float power = 10f;         // 폭탄 발사 속도

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

        // 포신 회전 적용 (localRotation 이용)
        firePivot.localRotation = Quaternion.Euler(0, 0, angle);

        // 발사 (스페이스)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        // 디버그: 발사 방향 그리기
        Debug.DrawRay(firePoint.position, firePoint.up * 2f, Color.red);
    }

    // 폭탄 생성하고 속도 설정
    private void Shoot()
    {
        Bomb newBomb = Instantiate(bombPrefab, firePoint.position, Quaternion.identity);
        newBomb.SetVelocity(firePoint.up * power);
    }
}

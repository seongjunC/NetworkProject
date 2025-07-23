using UnityEngine;

public class Throwing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform firePivot;       // ȸ���� ���� �κ� (��: FirePivot)
    [SerializeField] Transform firePoint;       // ���� ��ź�� ���� ��ġ
    [SerializeField] Bomb bombPrefab;

    [Header("Controls")]
    [SerializeField] float angle = 45f;         // ���� ����
    [SerializeField] float angleStep = 1f;      // ���� ��ȭ��
    [SerializeField] float power = 10f;         // ��ź �߻� �ӵ�

    private void Update()
    {
        // ���� ���� (Up/Down)
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

        // ���� ȸ�� ���� (localRotation �̿�)
        firePivot.localRotation = Quaternion.Euler(0, 0, angle);

        // �߻� (�����̽�)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        // �����: �߻� ���� �׸���
        Debug.DrawRay(firePoint.position, firePoint.up * 2f, Color.red);
    }

    // ��ź �����ϰ� �ӵ� ����
    private void Shoot()
    {
        Bomb newBomb = Instantiate(bombPrefab, firePoint.position, Quaternion.identity);
        newBomb.SetVelocity(firePoint.up * power);
    }
}

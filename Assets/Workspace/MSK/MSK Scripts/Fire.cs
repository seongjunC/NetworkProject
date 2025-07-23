using Photon.Pun;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePivot;       // ȸ���� ���� �κ�
    [SerializeField] private Transform firePoint;       // ���� ��ź�� ���� ��ġ
    [SerializeField] private GameObject bombPrefab;

    [Header("Controls")]
    [SerializeField] private float angle = 45f;         // ������ ���� ����
    [SerializeField] private float angleStep = 0.5f;      // ���� ��ȭ��
    [SerializeField] private float maxPower = 10f;         // ��ź �߻� �ӵ�
    [SerializeField] private float chargingSpeed = 10f;    // �����ӵ�

    private float powerCharge = 0f;         // ����
    private bool isCharging = false;        // ���� ������ ����

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

        // ���� ȸ�� 
        firePivot.localRotation = Quaternion.Euler(0, 0, angle);


        // �����̽��� ������ ������ ���� ����
        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            Debug.Log("����");
            powerCharge += chargingSpeed * Time.deltaTime;
            powerCharge = Mathf.Clamp(powerCharge, 0f, maxPower);
        }

        // �����̽��ٿ��� ���� ���� �� �߻�
        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log($"�߻�, �� : {powerCharge}");
            Shoot();
            powerCharge = 0f;
            isCharging = false;
        }
        Debug.DrawRay(firePoint.position, firePoint.up * 2f, Color.red);
    }

    // ��ź �����ϰ� �ӵ� ����
    private void Shoot()
    {
        //Bomb newBomb = PhotonNetwork.Instantiate(bombPrefab, firePoint.position, Quaternion.identity);
        //newBomb.SetVelocity(firePoint.up * powerCharge);
    }
}

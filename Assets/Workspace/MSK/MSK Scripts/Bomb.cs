using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ź ������ �ϴ� Ŭ����
public class Bomb : MonoBehaviour
{
    private Cutter _cut;                      // Cutter ��ũ��Ʈ ���� (�ڸ��� ���)
    private bool _dead;                       // ��ź�� �������� ����
    [SerializeField] private Rigidbody2D _rigidbody;          // ���� �̵��� ���� Rigidbody2D ������Ʈ
    [SerializeField] private GameObject _explosionPrefab;     // ���� ȿ�� ������
    [SerializeField] private int damageAmount = 50;

    // �ܺο��� �ӵ� �����ϴ� �Լ� (�߻� �ӵ� �� ȸ�� ��ũ)
    public void SetVelocity(Vector2 value)
    {
        _rigidbody.velocity = value;                     // �̵� �ӵ� ����
        _rigidbody.AddTorque(Random.Range(-8f, 8f));     // ���� ȸ�� ��ũ �߰� (���鼭 ������)
    }

    private void Start()
    {
        _cut = FindObjectOfType<Cutter>();       // �� �� Cutter ��ũ��Ʈ ã�Ƽ� �Ҵ�
    }

    // �ݶ��̴� �浹 �� ȣ��
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_dead) return;                        // �̹� �������� ����


        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.OnHit(damageAmount);
            }
        }
        // Cutter ��ġ�� ��ź ��ġ�� �ű� (�ڸ��� ��ġ ����)
        _cut.transform.position = transform.position;

        // DoCut �Լ��� ������ ȣ�� (0.001�� �� ȣ��)
        Invoke(nameof(DoCut), 0.001f);

        _dead = true;                            // ���� ���·� ����
    }

    // Cutter�� �ڸ��� �Լ� ȣ�� �� �ڽ�(��ź) �ı� �� ���� ȿ�� ����
    private void DoCut()
    {
        _cut.DoCut();                            // �ڸ��� ����
        Destroy(gameObject);                     // ��ź ������Ʈ �ı�
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);   // ���� ����Ʈ ����
    }
}
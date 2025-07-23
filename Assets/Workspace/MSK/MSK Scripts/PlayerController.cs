using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 2f; // �ӵ�
    [SerializeField] private Transform player;
    //  [SerializeField] private float _maxMove = 10f;  // �ִ� �̵��Ÿ�
    [SerializeField] private int _hp = 100;         // hp

    private bool _isDead = false;                   // �������

    private void FixedUpdate()
    {
        if (_isDead) 
            return;

        // �̵� ó��
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            player.localScale = new Vector3(-horizontal, 1f, 1f);
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = horizontal * _speed;
            _rigidbody.velocity = velocity;
        }

        // ȸ�� ���� ���� ,������ ����
        float z = _rigidbody.rotation;
        z = Mathf.Clamp(z, -45f, 45f);
        _rigidbody.MoveRotation(z);
    }
    //  �ǰ� ó��
    public void OnHit(int damage)
    {
        _hp -= damage;
        Debug.Log("�ǰ�");
        if (_hp <= 0)
            _isDead = true;
    }
}

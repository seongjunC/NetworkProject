using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 2f; // 속도
    [SerializeField] private Transform player;
    //  [SerializeField] private float _maxMove = 10f;  // 최대 이동거리
    [SerializeField] private int _hp = 100;         // hp

    private bool _isDead = false;                   // 사망여부

    private void FixedUpdate()
    {
        if (_isDead) 
            return;

        // 이동 처리
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            player.localScale = new Vector3(-horizontal, 1f, 1f);
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = horizontal * _speed;
            _rigidbody.velocity = velocity;
        }

        // 회전 각도 제한 ,뒤집힘 방지
        float z = _rigidbody.rotation;
        z = Mathf.Clamp(z, -45f, 45f);
        _rigidbody.MoveRotation(z);
    }
    //  피격 처리
    public void OnHit(int damage)
    {
        _hp -= damage;
        Debug.Log("피격");
        if (_hp <= 0)
            _isDead = true;
    }
}

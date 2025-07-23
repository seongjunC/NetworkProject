using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private float _maxMove = 10f;
    [SerializeField] private int _hp = 100;

    private bool _isDead = false;

    private void FixedUpdate()
    {
        if (_isDead)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            player.localScale = new Vector3(-horizontal, 1f, 1f);
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = horizontal * _speed;
            _rigidbody.velocity = velocity;
        }
    }
    public void OnHit(int damage)
    {
        _hp -= damage;
        Debug.Log("ÇÇ°Ý");
        if (_hp <= 0)
            _isDead = true;
    }
}

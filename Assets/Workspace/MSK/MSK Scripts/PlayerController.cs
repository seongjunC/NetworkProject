using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] float _speed = 2f;
    [SerializeField] float _jumpSpeed = 5f;
    [SerializeField] Transform player;

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            player.localScale = new Vector3(-horizontal, 1f,1f);
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = horizontal * _speed;
            _rigidbody.velocity = velocity;
        }

    }
}

using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 2f; // �ӵ�
    [SerializeField] private Transform player;
    [SerializeField] private float _maxMove = 5f;  // �ִ� �̵��Ÿ�
    [SerializeField] private int _hp = 100;         // hp


    private float _movable;
    private bool _isDead = false;                   // �������
    private bool _isMoving = false;                 // ������ ����
    public bool IsAttacked { get; private set; } = false;
    private void Awake()
    {
        //�̵� ������ �Ÿ��� �̵� �ִ�Ÿ��� ����
        _movable = _maxMove;

        if (photonView.IsMine) // �� ĳ������ ���� ���
        {
            TestBattleManager battleManager = FindObjectOfType<TestBattleManager>();
            if (battleManager != null)
            {
                battleManager.RegisterPlayer(this);
            }
        }
    }
    private void FixedUpdate()
    {
        //  �÷��̾ �׾��ų�, �� �÷��̾ �ƴ϶�� ������ ���� ��Ż
        if (!photonView.IsMine)
            return;
        if (_isDead) 
            return;
        // �̵� ó��
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (_movable <= 0)
            return;
        if (horizontal != 0)
        {
            _isMoving = true;
            _movable -= Mathf.Abs(horizontal) * _speed * Time.deltaTime;
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
    public void ResetTurn()
    {
        //�̵� ������ �Ÿ��� �̵� �ִ�Ÿ��� ����
        _movable = _maxMove;
        SetAttacked(false);
        Debug.Log("�̵��� ����");
    }
    public void SetAttacked(bool value)
    {
        IsAttacked = value;
    }
}

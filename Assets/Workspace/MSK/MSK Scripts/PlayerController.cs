using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 2f; // �ӵ�
    [SerializeField] private Transform player;
    [SerializeField] private float _maxMove = 5f;  // �ִ� �̵��Ÿ�
    [SerializeField] private int _hp = 100;         // hp

    [SerializeField] private TextMeshProUGUI _textMeshPro;
    private float _movable;
    private bool _isDead = false;                   // �������
    private bool isControllable = false;

    public bool IsAttacked { get; private set; } = false;

    public Action OnPlayerAttacked;
    public Action OnPlayerDied;

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
        //  �г��� �ʱ�ȭ
        _textMeshPro.text = photonView.IsMine ? PhotonNetwork.NickName : photonView.Owner.NickName;
    }

    private void FixedUpdate()
    {
        //  �÷��̾ �׾��ų�, �� �÷��̾ �ƴ϶�� ������ ���� ��Ż
        if (!photonView.IsMine)
            return;
        if (_isDead) 
            return;
        if (!isControllable)
            return;

        // �̵� ó��
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (_movable <= 0)
            return;
        if (horizontal != 0)
        {
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
            PlayerDead();
    }

    //  �÷��̾� ���ൿ
    public void ResetTurn()
    {
        //�̵� ������ �Ÿ��� �̵� �ִ�Ÿ��� ����
        _movable = _maxMove;
        SetAttacked(false);
        Debug.Log("���� & ������ ����");
    }

    //  ���� ���� ���� �ٲ�
    public void SetAttacked(bool value)
    {
        IsAttacked = value;
        if (IsAttacked == true)
        {
            OnPlayerAttacked?.Invoke();
        }
    }

    //  �÷��̾� ��� �� �ı�ó��
    public void PlayerDead()
    {   
        Destroy(gameObject);
        photonView.RPC("RPC_PlayerDead", RpcTarget.All);
        _isDead = true;
        // TODO : �� �޴������� �÷��̾� ���� ����� �̺�Ʈ �����ϱ�
    }

    public void EnableControl(bool enable)
    {
        Debug.Log($" {photonView.IsMine}, {PhotonNetwork.NickName}, {isControllable}");
        isControllable = enable;
        if (isControllable == true) 
        {
            ResetTurn();
        }
    }
}

using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private float _maxMove = 5f;
    [SerializeField] private float _maxhp = 200;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private TankData _data;

    public bool _isDead { get; private set; } = false;
    public bool OnBarrier { get; private set; } = false;

    public PlayerInfo myInfo;
    public float _hp;
    public float _movable;

    public bool isControllable { get; private set; } = false;
    public bool IsAttacked { get; private set; } = false;

    public Action OnPlayerAttacked;
    public Action OnPlayerDied;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            _movable = _maxMove;
            _hp = _maxhp;
            TestBattleManager battleManager = FindObjectOfType<TestBattleManager>();
            MSK_UIManager uiManager = FindObjectOfType<MSK_UIManager>();

            if (battleManager != null)
                battleManager.RegisterPlayer(this);
            if (uiManager != null)
                uiManager.RegisterPlayer(this);

            PlayerSetUp();
        }

        // 닉네임 색상 설정
        _textMeshPro.text = photonView.IsMine
            ? $"<color=#00aaff>{PhotonNetwork.NickName}</color>"
            : $"<color=#ff4444>{photonView.Owner.NickName}</color>";
    }

    private void PlayerSetUp()
    {
        myInfo = new PlayerInfo(photonView.Owner);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || _isDead || !isControllable)
            return;

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

            // 이동 중일 때만 회전 제한
            float z = _rigidbody.rotation;
            z = Mathf.Clamp(z, -45f, 45f);
            _rigidbody.MoveRotation(z);
        }
    }

    [PunRPC]
    public void OnHit(int damage)
    {
        if (OnBarrier)
        {
            if (damage > 100000)
            {
                PlayerDead();
            }
            OnBarrier = false;
            Debug.Log("배리어로 1회 피격 방어");
            return;
        }
        _hp -= damage;
        Debug.Log("피격");

        if (_hp <= 0)
            PlayerDead();
    }

    public void EndPlayerTurn()
    {
        _movable = 0;
        SetAttacked(true);
        isControllable = false;
    }

    public void ResetTurn()
    {
        _movable = _maxMove;
        SetAttacked(false);
        isControllable = true;
    }

    public void SetAttacked(bool value)
    {
        if (IsAttacked == value) return;

        IsAttacked = value;

        if (IsAttacked)
        {
            OnPlayerAttacked?.Invoke();
            isControllable = false;
        }
    }

    public void PlayerDead()
    {
        if (_isDead) return;
        _isDead = true;
        OnPlayerAttacked = null;
        OnPlayerDied?.Invoke();
        photonView.RPC("RPC_PlayerDead", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_PlayerDead()
    {
        Destroy(gameObject);
    }

    public void EnableControl(bool enable)
    {
        isControllable = enable;
    }
    public void ApplyBarrier()
    {
        OnBarrier = true;
    }

    public void FixedHealToPlayer(int Amount)
    {
        float beforeHp = _hp;
        _hp = MathF.Min(_hp + Amount, _maxhp);
        Debug.Log($"{_hp - beforeHp} 만큼 체력 회복");
    }
    public void RatioHealToPlayer(int Amount)
    {
        float beforeHp = _hp;
        _hp = MathF.Min(_hp + _maxhp * Amount / 100, _maxhp);
        Debug.Log($"{_hp - beforeHp} 만큼 체력 회복");
    }
}

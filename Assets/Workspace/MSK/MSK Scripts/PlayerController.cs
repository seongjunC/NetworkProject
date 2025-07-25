using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 2f; // 속도
    [SerializeField] private Transform player;
    [SerializeField] private float _maxMove = 5f;  // 최대 이동거리
    [SerializeField] private int _hp = 100;         // hp

    [SerializeField] private TextMeshProUGUI _textMeshPro;
    private float _movable;
    private bool _isDead = false;                   // 사망여부
    private bool isControllable = false;

    public bool IsAttacked { get; private set; } = false;

    public Action OnPlayerAttacked;
    public Action OnPlayerDied;

    private void Awake()
    {
        //이동 가능한 거리를 이동 최대거리로 설정
        _movable = _maxMove;

        if (photonView.IsMine) // 내 캐릭터일 때만 등록
        {
            TestBattleManager battleManager = FindObjectOfType<TestBattleManager>();
            if (battleManager != null)
            {
                battleManager.RegisterPlayer(this);
            }
        }
        //  닉네임 초기화
        _textMeshPro.text = photonView.IsMine ? PhotonNetwork.NickName : photonView.Owner.NickName;
    }

    private void FixedUpdate()
    {
        //  플레이어가 죽었거나, 내 플래이어가 아니라면 움직임 권한 박탈
        if (!photonView.IsMine)
            return;
        if (_isDead) 
            return;
        if (!isControllable)
            return;

        // 이동 처리
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
            PlayerDead();
    }

    //  플레이어 재행동
    public void ResetTurn()
    {
        //이동 가능한 거리를 이동 최대거리로 설정
        _movable = _maxMove;
        SetAttacked(false);
        Debug.Log("공격 & 움직임 가능");
    }

    //  공격 가능 여부 바꿈
    public void SetAttacked(bool value)
    {
        IsAttacked = value;
        if (IsAttacked == true)
        {
            OnPlayerAttacked?.Invoke();
        }
    }

    //  플레이어 사망 시 파괴처리
    public void PlayerDead()
    {   
        Destroy(gameObject);
        photonView.RPC("RPC_PlayerDead", RpcTarget.All);
        _isDead = true;
        // TODO : 턴 메니저에게 플레이어 죽음 사실을 이벤트 전달하기
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

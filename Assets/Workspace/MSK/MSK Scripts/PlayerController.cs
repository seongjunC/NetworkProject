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
    [SerializeField] private float _maxhp = 200;         // hp
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    private bool _isDead = false;                   // 사망여부
    // 안계셔서 일단 임시로 추가했습니다. 
    // 추후 myInfo에 플레이어정보를 넣어야합니다.
    public PlayerInfo myInfo;
    public float _hp;
    public float _movable;

    public bool isControllable { get; private set; } = false;
    public bool IsAttacked { get; private set; } = false;

    public Action OnPlayerAttacked;
    public Action OnPlayerDied;

    private void Awake()
    {
        if (photonView.IsMine) // 내 캐릭터일 때만 등록
        {
            //이동 가능한 거리를 이동 최대거리로 설정
            _movable = _maxMove;
            _hp = _maxhp;
            TestBattleManager battleManager = FindObjectOfType<TestBattleManager>();
            MSK_UIManager uiManager = FindObjectOfType<MSK_UIManager>();

            if (battleManager != null)
            {
                battleManager.RegisterPlayer(this);
                uiManager.RegisterPlayer(this);
            }
        }
        //  닉네임 초기화,
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
    [PunRPC]
    public void OnHit(int damage)
    {
        // TODO : 플레이어 피격 처리 시 방 설정에 따른 피격여부
        _hp -= damage;
        Debug.Log("피격");
        if (_hp <= 0)
            PlayerDead();
    }

    //  플레이어 턴 종료
    public void EndPlayerTurn()
    {
        _movable = 0;
        SetAttacked(true);
        isControllable = false;
    }

    //  플레이어 재행동
    public void ResetTurn()
    {
        //이동 가능한 거리를 이동 최대거리로 설정
        _movable = _maxMove;
        SetAttacked(false);
        isControllable = true;
    }

    //  공격 가능 여부 바꿈
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

    //  플레이어 사망 시 파괴처리
    public void PlayerDead()
    {
        Destroy(gameObject);
        OnPlayerAttacked -= OnPlayerAttacked;
        _isDead = true;
        // TODO : 턴 메니저에게 플레이어 죽음 사실을 이벤트 전달하기
    }

    public void EnableControl(bool enable)
    {
        isControllable = enable;
    }
}

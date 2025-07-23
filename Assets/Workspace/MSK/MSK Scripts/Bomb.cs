using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 폭탄 역할을 하는 클래스
public class Bomb : MonoBehaviour
{
    private Cutter _cut;                      // Cutter 스크립트 참조 (자르기 기능)
    private bool _dead;                       // 폭탄이 터졌는지 여부
    [SerializeField] private Rigidbody2D _rigidbody;          // 물리 이동을 위한 Rigidbody2D 컴포넌트
    [SerializeField] private GameObject _explosionPrefab;     // 폭발 효과 프리팹
    [SerializeField] private int damageAmount = 50;

    // 외부에서 속도 설정하는 함수 (발사 속도 및 회전 토크)
    public void SetVelocity(Vector2 value)
    {
        _rigidbody.velocity = value;                     // 이동 속도 지정
        _rigidbody.AddTorque(Random.Range(-8f, 8f));     // 랜덤 회전 토크 추가 (돌면서 떨어짐)
    }

    private void Start()
    {
        _cut = FindObjectOfType<Cutter>();       // 씬 내 Cutter 스크립트 찾아서 할당
    }

    // 콜라이더 충돌 시 호출
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_dead) return;                        // 이미 터졌으면 무시


        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.OnHit(damageAmount);
            }
        }
        // Cutter 위치를 폭탄 위치로 옮김 (자르기 위치 설정)
        _cut.transform.position = transform.position;

        // DoCut 함수를 딜레이 호출 (0.001초 후 호출)
        Invoke(nameof(DoCut), 0.001f);

        _dead = true;                            // 터짐 상태로 변경
    }

    // Cutter의 자르기 함수 호출 후 자신(폭탄) 파괴 및 폭발 효과 생성
    private void DoCut()
    {
        _cut.DoCut();                            // 자르기 실행
        Destroy(gameObject);                     // 폭탄 오브젝트 파괴
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);   // 폭발 이펙트 생성
    }
}
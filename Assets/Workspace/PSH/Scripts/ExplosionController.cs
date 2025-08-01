using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private readonly int ExplodeHash = Animator.StringToHash("Hit");
    private readonly int FireHash = Animator.StringToHash("Fire");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.Log("애니메이터를 찾을 수 없습니다");
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 피격시 
    public void TriggerHit()
    {
        if (animator != null)
        {
            animator.Play(ExplodeHash);
        }
    }
    //발사시
    public void TriggerFire()
    {
        if (animator != null)
        {
            animator.Play(FireHash);
        }
    }


    //애니메이션이 끝나면 파괴
    public void AnimationEnded()
    {
        Destroy(gameObject);
    }
}

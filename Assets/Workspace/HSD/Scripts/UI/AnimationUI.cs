using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUI : MonoBehaviour
{
    protected Animator anim;

    protected static readonly int inHash    = Animator.StringToHash("In");
    protected static readonly int outHash   = Animator.StringToHash("Out");

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        anim ??= GetComponent<Animator>();
    }

    protected void AnimationStart()
    {
        anim.SetTrigger(inHash);
        gameObject.SetActive(true);
    }
    private void AnimationEndEvent() => gameObject.SetActive(false);
}

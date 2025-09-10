using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoxBelt : MonoBehaviour, IPoolable
{
    [SerializeField] private Animator animator;
    public void CutBelt()
    {
        animator.SetTrigger("Cut");
    }
    public void Return()
    {
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }
    public void OnGet()
    {
    }

    public void OnReturn()
    {
        animator.Rebind();
        animator.Play("Idle");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBag : MonoBehaviour
{
    private Animator anim;

    private float animTime = 0;
    public bool IsPackOver { get { return animTime >= 1.0f; } }
    public bool IsPacking
    {
        get
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            return stateInfo.shortNameHash == closeAnimIndex;
        }
    }

    private string closeAnimName = "PaperBag_close";
    private int closeAnimIndex;

    private void Start()
    {
        anim = GetComponent<Animator>();
        closeAnimIndex = Animator.StringToHash(closeAnimName);
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == closeAnimIndex)
        {
            animTime = stateInfo.normalizedTime;
        }
    }

    public void PlayPackAnimation()
    {
        anim.SetTrigger("Closed");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PapichAndButterflyAnimation : MonoBehaviour
{

    [SerializeField] Animator _papichAnimation;
    [SerializeField] Animator _butterflyAnimation;
    float _playDelay = 3f;

    void Start()
    {
        InvokeRepeating("PlayAnimation", 1.0f, _playDelay);
    }

    private void PlayAnimation()
    {
        if (!_butterflyAnimation.GetCurrentAnimatorStateInfo(0).IsName("Scripted butterfly"))
        {
            _papichAnimation.Play("Papich animation");
            _butterflyAnimation.Play("Scripted butterfly");
        }
    }


}

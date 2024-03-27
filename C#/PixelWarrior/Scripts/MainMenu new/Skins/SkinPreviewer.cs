using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * �������� �������� �����
 */
public class SkinPreviewer : MonoBehaviour
{
    [SerializeField] GameObject _skinPreviewAnimation;
    [SerializeField] GameObject _skinAnimationParticles;

    public void OnPreview()
    {
        _skinAnimationParticles.SetActive(true);
        _skinPreviewAnimation.SetActive(true);
    }


    public void OnPreviewClose()
    {
        _skinAnimationParticles.SetActive(false);
        _skinPreviewAnimation.SetActive(false);
    }
}

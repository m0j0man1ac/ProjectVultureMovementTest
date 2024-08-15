using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DOTweenExtensions
{
    public static Tweener DORotatePitch(this Transform target, float endValue, float duration)
    {
        // Custom getter and setter for the Y-axis rotation
        return DOTween.To(
            () => target.eulerAngles.x,
            x => target.eulerAngles = new Vector3(x, target.eulerAngles.y, target.eulerAngles.z),
            endValue,
            duration
        );
    }
}

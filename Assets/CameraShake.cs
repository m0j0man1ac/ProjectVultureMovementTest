using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 1.0f; // Duration of the shake
    public float intensity = 1.0f; // Initial intensity of the shake

    private float shakeDuration;
    private float shakeIntensity;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void StartShake(float shakeDuration, float shakeIntensity)
    {
        this.shakeDuration = shakeDuration;
        this.intensity = shakeIntensity;
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            shakeIntensity = Mathf.Pow(1f-(elapsed / duration), 3f) * intensity;

            float x = Mathf.PerlinNoise(Time.time * shakeIntensity, 0.0f) * 2 - 1;
            float y = Mathf.PerlinNoise(0.0f, 1f + Time.time * shakeIntensity) * 2 - 1;

            transform.localPosition = originalPosition + new Vector3(x, y, 0) * shakeIntensity;

            elapsed += Time.deltaTime;
            //shakeIntensity = Mathf.Lerp(shakeIntensity, 0, elapsed / shakeDuration); // Decay the intensity
            yield return null;
        }

        transform.localPosition = originalPosition; // Reset to original position
    }
}

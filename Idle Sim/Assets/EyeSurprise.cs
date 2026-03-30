using UnityEngine;
using System.Collections;

public class EyeSurprise : MonoBehaviour
{
    public Transform eyeball;
    public float scaleMultiplier = 1.3f;
    public float duration = 1.5f;

    private Vector3 originalScale;
    private Coroutine currentRoutine;

    void Start()
    {
        if (eyeball != null)
            originalScale = eyeball.localScale;
    }

    public void TriggerSurprise()
    {
        // Stop any current animation and reset scale
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (eyeball != null)
            eyeball.localScale = originalScale;

        currentRoutine = StartCoroutine(SurpriseEffect());
    }

    IEnumerator SurpriseEffect()
    {
        if (eyeball == null)
            yield break;

        Vector3 targetScale = originalScale * scaleMultiplier;

        // Grow over 0.2 seconds
        float growTime = 0.2f;
        float t = 0;
        while (t < 1f)
        {
            eyeball.localScale = Vector3.Lerp(originalScale, targetScale, t);
            t += Time.deltaTime / growTime;
            yield return null;
        }
        eyeball.localScale = targetScale;

        // Wait
        yield return new WaitForSeconds(duration);

        // Shrink over 0.2 seconds
        float shrinkTime = 0.2f;
        t = 0;
        while (t < 1f)
        {
            eyeball.localScale = Vector3.Lerp(targetScale, originalScale, t);
            t += Time.deltaTime / shrinkTime;
            yield return null;
        }
        eyeball.localScale = originalScale;

        currentRoutine = null;
    }
}
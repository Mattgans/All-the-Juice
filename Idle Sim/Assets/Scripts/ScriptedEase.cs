using UnityEngine;

/// <summary>
/// Eases a GameObject's scale from zero to its original scale using v = k * (goal - x) * dt.
/// Attach to any object that should "pop in" with an ease when activated.
/// Call TriggerEase() or enable the object to start the animation.
/// </summary>
public class ScriptedEase : MonoBehaviour
{
    [Header("Ease Settings")]
    [Tooltip("Speed constant for the ease. Higher = faster settle.")]
    public float easeSpeed = 8f;

    [Tooltip("Overshoot multiplier (1.0 = no overshoot, 1.3 = 30% overshoot)")]
    public float overshootAmount = 1.2f;

    [Tooltip("Threshold to snap to goal and stop easing")]
    public float snapThreshold = 0.01f;

    private Vector3 goalScale;
    private Vector3 currentScale;
    private bool isEasing = false;
    private bool hasOvershot = false;

    void OnEnable()
    {
        TriggerEase();
    }

    /// <summary>
    /// Call this to start the ease-in animation from zero scale.
    /// </summary>
    public void TriggerEase()
    {
        goalScale = transform.localScale;
        // Start from zero
        currentScale = Vector3.zero;
        transform.localScale = currentScale;
        hasOvershot = false;
        isEasing = true;
    }

    void Update()
    {
        if (!isEasing) return;

        Vector3 target;

        // Phase 1: Overshoot past the goal
        if (!hasOvershot)
        {
            target = goalScale * overshootAmount;
            currentScale += easeSpeed * (target - currentScale) * Time.deltaTime;

            // Once we pass the goal scale, switch to settle phase
            if (currentScale.x >= goalScale.x * (overshootAmount - 0.05f))
            {
                hasOvershot = true;
            }
        }
        // Phase 2: Settle back to goal
        else
        {
            target = goalScale;
            currentScale += easeSpeed * (target - currentScale) * Time.deltaTime;

            // Snap when close enough
            if (Vector3.Distance(currentScale, goalScale) < snapThreshold)
            {
                currentScale = goalScale;
                isEasing = false;
            }
        }

        transform.localScale = currentScale;
    }

    /// <summary>
    /// Trigger a "punch" ease - briefly scales up then returns to current scale.
    /// Useful for celebrating events like resource gains.
    /// </summary>
    public void TriggerPunchEase()
    {
        goalScale = transform.localScale;
        currentScale = goalScale * overshootAmount;
        transform.localScale = currentScale;
        hasOvershot = true; // Go straight to settle phase
        isEasing = true;
    }
}

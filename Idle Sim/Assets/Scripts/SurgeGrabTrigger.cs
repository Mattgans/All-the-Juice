using UnityEngine;

/// <summary>
/// Attach this to the Surge Object (e.g. "box golg").
/// Detects when anything with a collider touches it and calls GrabSurge().
/// Requires a Collider with Is Trigger checked on this object.
/// Has a grace period after spawning to prevent instant collection from
/// overlapping colliders at the spawn location.
/// </summary>
public class SurgeGrabTrigger : MonoBehaviour
{
    [Tooltip("If set, only colliders with this tag trigger the grab. Leave empty to accept any collision.")]
    public string handTag = "";

    [Tooltip("Seconds after spawn before grab is allowed (prevents instant collection)")]
    public float grabGracePeriod = 1.0f;

    private float spawnTime = -999f;

    /// <summary>
    /// Called by SurpriseSurge when the surge spawns to reset the grace timer.
    /// </summary>
    public void OnSurgeSpawned()
    {
        spawnTime = Time.time;
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore collisions during grace period after spawn
        if (Time.time - spawnTime < grabGracePeriod)
            return;

        if (!string.IsNullOrEmpty(handTag) && !other.CompareTag(handTag))
            return;

        if (SurpriseSurge.Instance != null)
        {
            Debug.Log($"Surge grabbed by: {other.gameObject.name}");
            SurpriseSurge.Instance.GrabSurge();
        }
    }
}

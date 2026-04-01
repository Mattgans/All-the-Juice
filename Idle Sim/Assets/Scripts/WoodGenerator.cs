using UnityEngine;

public class OakGenerator : MonoBehaviour
{
    public int baseOakPerTick = 10;
    public float tickInterval = 5f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tickInterval)
        {
            if (ResourceManager.Instance != null && TycoonManager.Instance != null)
            {
                // Multiply the base rate by the current Tycoon multiplier
                float multiplier = TycoonManager.Instance.oakProductionMultiplier;

                // Apply prestige multiplier
                if (PrestigeManager.Instance != null)
                    multiplier *= PrestigeManager.Instance.GetPrestigeMultiplier();

                // Apply surprise surge multiplier
                if (SurpriseSurge.Instance != null)
                    multiplier *= SurpriseSurge.Instance.GetSurgeMultiplier();

                int totalProduced = Mathf.RoundToInt(baseOakPerTick * multiplier);

                ResourceManager.Instance.AddOak(totalProduced);
            }
            timer = 0f;
        }
    }
}
using UnityEngine;

public class MapleGenerator : MonoBehaviour
{
    public int baseMaplePerTick = 10;
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
                float multiplier = TycoonManager.Instance.mapleProductionMultiplier;
                int totalProduced = Mathf.RoundToInt(baseMaplePerTick * multiplier);
                
                ResourceManager.Instance.AddMaple(totalProduced);
            }
            timer = 0f;
        }
    }
}
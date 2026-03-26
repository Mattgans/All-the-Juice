using UnityEngine;
using TMPro;
using Tiny; // Required for the Trail script

// This class allows us to see a list of Trails for EACH generator in the Inspector
[System.Serializable]
public class TrailGroup
{
    public string generatorName; // Just for labeling in Inspector
    public Trail[] trails;       // Drag the 5 color scripts for ONE generator here
}

public class TycoonManager : MonoBehaviour
{
    public static TycoonManager Instance;

    [Header("Global Settings")]
    public float priceMultiplier = 1.5f;

    [Header("Oak Logic")]
    public int oakGenBaseCost = 10;
    public int oakGenCount = 0;
    public float oakProductionMultiplier = 1.0f;
    [Space]
    public TextMeshProUGUI oakGenCostText;
    public TextMeshProUGUI oakMultText;
    public GameObject[] oakGenModels; 
    [Space]
    public TrailGroup[] oakGeneratorTrailGroups; // Set Size to 4 in Inspector

    [Header("Maple Logic")]
    public int mapleGenBaseCost = 50;
    public int mapleGenCount = 0;
    public float mapleProductionMultiplier = 1.0f;
    [Space]
    public TextMeshProUGUI mapleGenCostText;
    public TextMeshProUGUI mapleMultText;
    public GameObject[] mapleGenModels;
    [Space]
    public TrailGroup[] mapleGeneratorTrailGroups; // Set Size to 4 in Inspector

    [Header("Upgrade Effects")]
    public ParticleSystem upgradeParticles;
    public AudioSource upgradeAudio;

    [Header("House Settings")]
    public int oakHouseCost = 5000;
    public int mapleHouseCost = 5000;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateTycoonUI();
        // Initialize all trails to the current multiplier level
        UpdateAllTrailVisuals(oakGeneratorTrailGroups, oakProductionMultiplier, false);
        UpdateAllTrailVisuals(mapleGeneratorTrailGroups, mapleProductionMultiplier, false);
    }

    // --- OAK UPGRADES ---

    public void BuyOakGenerator()
    {
        int currentCost = GetExponentialCost(oakGenBaseCost, oakGenCount);
        if (ResourceManager.Instance.oakCount >= currentCost && oakGenCount < 4)
        {
            ResourceManager.Instance.AddOak(-currentCost);
            oakGenModels[oakGenCount].SetActive(true);
            oakGenCount++;
            
            // Ensure the newly bought generator has the correct trail color active
            UpdateAllTrailVisuals(oakGeneratorTrailGroups, oakProductionMultiplier, false);
            UpdateTycoonUI();
        }
    }

    public void UpgradeOakMultiplier()
    {
        int currentLevel = Mathf.FloorToInt(oakProductionMultiplier);
        int upgradeCost = currentLevel * 100;

        if (ResourceManager.Instance.oakCount >= upgradeCost && oakProductionMultiplier < 5.0f)
        {
            ResourceManager.Instance.AddOak(-upgradeCost);
            oakProductionMultiplier += 1.0f;
            
            // This updates EVERY generator's trail color at once
            UpdateAllTrailVisuals(oakGeneratorTrailGroups, oakProductionMultiplier, true);
            UpdateTycoonUI();
        }
    }

    // --- MAPLE UPGRADES ---

    public void BuyMapleGenerator()
    {
        int currentCost = GetExponentialCost(mapleGenBaseCost, mapleGenCount);
        if (ResourceManager.Instance.mapleCount >= currentCost && mapleGenCount < 4)
        {
            ResourceManager.Instance.AddMaple(-currentCost);
            mapleGenModels[mapleGenCount].SetActive(true);
            mapleGenCount++;
            
            UpdateAllTrailVisuals(mapleGeneratorTrailGroups, mapleProductionMultiplier, false);
            UpdateTycoonUI();
        }
    }

    public void UpgradeMapleMultiplier()
    {
        int currentLevel = Mathf.FloorToInt(mapleProductionMultiplier);
        int upgradeCost = currentLevel * 100;

        if (ResourceManager.Instance.mapleCount >= upgradeCost && mapleProductionMultiplier < 5.0f)
        {
            ResourceManager.Instance.AddMaple(-upgradeCost);
            mapleProductionMultiplier += 1.0f;
            
            UpdateAllTrailVisuals(mapleGeneratorTrailGroups, mapleProductionMultiplier, true);
            UpdateTycoonUI();
        }
    }

    // --- LOGIC FOR MULTIPLE GROUPS ---

    private void UpdateAllTrailVisuals(TrailGroup[] groups, float multiplierValue, bool playEffects)
    {
        if (groups == null) return;

        int targetIndex = Mathf.FloorToInt(multiplierValue) - 1;

        // Loop through every generator (1 to 4)
        foreach (TrailGroup group in groups)
        {
            if (group.trails == null) continue;

            // Inside each generator, loop through its 5 color trails
            for (int i = 0; i < group.trails.Length; i++)
            {
                if (group.trails[i] != null)
                {
                    group.trails[i].enabled = (i == targetIndex);
                }
            }
        }

        if (playEffects && Application.isPlaying)
        {
            if (upgradeAudio != null) upgradeAudio.Play();
            if (upgradeParticles != null) upgradeParticles.Play();
        }
    }

    private int GetExponentialCost(int baseCost, int ownedCount)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(priceMultiplier, ownedCount));
    }

    public void UpdateTycoonUI()
    {
        // Oak UI
        if (oakGenCostText != null) {
            string costText = oakGenCount >= 4 ? "MAX" : $"{GetExponentialCost(oakGenBaseCost, oakGenCount)} Oak";
            oakGenCostText.text = $"Oak Generator\nCost: {costText}";
        }
        if (oakMultText != null) {
            float nextCost = Mathf.FloorToInt(oakProductionMultiplier) * 100;
            string costDisplay = oakProductionMultiplier >= 5.0f ? "MAX" : $"{nextCost} Oak";
            oakMultText.text = $"Oak Multiplier\nCost: {costDisplay}\nCurrent: {oakProductionMultiplier}x";
        }

        // Maple UI
        if (mapleGenCostText != null) {
            string costText = mapleGenCount >= 4 ? "MAX" : $"{GetExponentialCost(mapleGenBaseCost, mapleGenCount)} Maple";
            mapleGenCostText.text = $"Maple Generator\nCost: {costText}";
        }
        if (mapleMultText != null) {
            float nextCost = Mathf.FloorToInt(mapleProductionMultiplier) * 100;
            string costDisplay = mapleProductionMultiplier >= 5.0f ? "MAX" : $"{nextCost} Maple";
            mapleMultText.text = $"Maple Multiplier\nCost: {costDisplay}\nCurrent: {mapleProductionMultiplier}x";
        }
    }

    public void TryPurchaseOakHouse(HouseToggler toggler)
    {
        if (ResourceManager.Instance.oakCount >= oakHouseCost)
        {
            ResourceManager.Instance.AddOak(-oakHouseCost);
            toggler.EnableHouse();
        }
        else
        {
            Debug.Log("Not enough Oak for the house!");
        }
    }

    public void TryPurchaseMapleHouse(HouseToggler toggler)
    {
        if (ResourceManager.Instance.mapleCount >= mapleHouseCost)
        {
            ResourceManager.Instance.AddMaple(-mapleHouseCost);
            toggler.EnableHouse();
        }
        else
        {
            Debug.Log("Not enough Oak for the house!");
        }
    }
}
using UnityEngine;
using TMPro;

public class TycoonManager : MonoBehaviour
{
    public static TycoonManager Instance;

    [Header("Global Settings")]
    [Tooltip("How much the cost increases each time you buy a generator (1.5 = 50% increase).")]
    public float priceMultiplier = 1.5f;

    [Header("Oak Logic")]
    public int oakGenBaseCost = 10;
    public int oakGenCount = 0;
    public float oakProductionMultiplier = 1.0f;
    [Space]
    public TextMeshProUGUI oakGenCostText;
    public TextMeshProUGUI oakMultText;
    public GameObject[] oakGenModels; // Ensure size is 4

    [Header("Maple Logic")]
    public int mapleGenBaseCost = 50;
    public int mapleGenCount = 0;
    public float mapleProductionMultiplier = 1.0f;
    [Space]
    public TextMeshProUGUI mapleGenCostText;
    public TextMeshProUGUI mapleMultText;
    public GameObject[] mapleGenModels; // Ensure size is 4

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
    }

    // --- OAK PURCHASES ---

    public void BuyOakGenerator()
    {
        int currentCost = GetExponentialCost(oakGenBaseCost, oakGenCount);

        if (ResourceManager.Instance.oakCount >= currentCost && oakGenCount < 4)
        {
            ResourceManager.Instance.AddOak(-currentCost);
            oakGenModels[oakGenCount].SetActive(true);
            oakGenCount++;
            UpdateTycoonUI();
        }
    }

    public void UpgradeOakMultiplier()
    {
        // Cost: 100 for 1x->2x, 200 for 2x->3x, etc.
        int currentLevel = Mathf.FloorToInt(oakProductionMultiplier);
        int upgradeCost = currentLevel * 100;

        if (ResourceManager.Instance.oakCount >= upgradeCost && oakProductionMultiplier < 5.0f)
        {
            ResourceManager.Instance.AddOak(-upgradeCost);
            oakProductionMultiplier += 1.0f;
            UpdateTycoonUI();
        }
    }

    // --- MAPLE PURCHASES ---

    public void BuyMapleGenerator()
    {
        int currentCost = GetExponentialCost(mapleGenBaseCost, mapleGenCount);

        if (ResourceManager.Instance.mapleCount >= currentCost && mapleGenCount < 4)
        {
            ResourceManager.Instance.AddMaple(-currentCost);
            mapleGenModels[mapleGenCount].SetActive(true);
            mapleGenCount++;
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
            UpdateTycoonUI();
        }
    }

    // --- HOUSE PURCHASES (Linking to your HouseToggler) ---

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
            Debug.Log("Not enough Maple for the house!");
        }
    }

    // --- UTILITIES ---

    private int GetExponentialCost(int baseCost, int ownedCount)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(priceMultiplier, ownedCount));
    }

    public void UpdateTycoonUI()
    {
        // Oak UI
        if (oakGenCostText != null)
        {
            string costText = oakGenCount >= 4 ? "MAX" : $"{GetExponentialCost(oakGenBaseCost, oakGenCount)} Oak";
            oakGenCostText.text = $"Oak Generator\nCost: {costText}";
        }
        
        if (oakMultText != null)
        {
            float nextCost = Mathf.FloorToInt(oakProductionMultiplier) * 100;
            string costDisplay = oakProductionMultiplier >= 5.0f ? "MAX" : $"{nextCost} Oak";
            oakMultText.text = $"Oak Generator Multiplier\nCost: {costDisplay}\nCurrent: {oakProductionMultiplier}x";
        }

        // Maple UI
        if (mapleGenCostText != null)
        {
            string costText = mapleGenCount >= 4 ? "MAX" : $"{GetExponentialCost(mapleGenBaseCost, mapleGenCount)} Maple";
            mapleGenCostText.text = $"Maple Generator\nCost: {costText}";
        }

        if (mapleMultText != null)
        {
            float nextCost = Mathf.FloorToInt(mapleProductionMultiplier) * 100;
            string costDisplay = mapleProductionMultiplier >= 5.0f ? "MAX" : $"{nextCost} Maple";
            mapleMultText.text = $"Maple Generator Multiplier\nCost: {costDisplay}\nCurrent: {mapleProductionMultiplier}x";
        }
    }
}
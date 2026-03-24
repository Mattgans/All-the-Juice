using UnityEngine;
using TMPro;
using System.Collections;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Oak Settings")]
    public int oakCount = 0;
    public TextMeshProUGUI oakText;
    public int oakGoal = 1000;

    [Header("Maple Settings")]
    public int mapleCount = 0;
    public TextMeshProUGUI mapleText;

    [Header("Unlockables")]
    public GameObject mapleBlockerPlane; // The wall/floor blocking the path
    public GameObject achievementPopup;  // The UI Panel for the achievement
    public float popupDuration = 3f;     // How long the popup stays visible
    
    [Header("Achievement Trophies")]
    public GameObject trophy1_Woodcutter; 
    public GameObject trophy2_Explorer;   
    public GameObject trophy3_Tycoon;     

    // These are the World Space Canvases (the signs) you just made
    public GameObject cup1Canvas; 
    public GameObject cup2Canvas;
    public GameObject cup3Canvas;

    // Bools prevent the "SetActive" code from running every single frame
    private bool t1Unlocked, t2Unlocked, t3Unlocked;

    private bool mapleUnlocked = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        if (achievementPopup != null) achievementPopup.SetActive(false);
    }

    public void AddOak(int amount)
    {
        oakCount += amount;
        UpdateUI();
        // Achievement 1: 500 Oak
        if (!t1Unlocked && oakCount >= 500)
        {
            t1Unlocked = true;
            trophy1_Woodcutter.SetActive(true);
            cup1Canvas.SetActive(true);
        }

        // Check for the unlock goal
        // Achievement 2: 5,000 Oak (Maple Unlock)
        if (!mapleUnlocked && oakCount >= oakGoal)
        {
            t2Unlocked = true;               // Mark the trophy as unlocked
            trophy2_Explorer.SetActive(true); // Show the physical trophy
            cup2Canvas.SetActive(true);
            UnlockMapleArea();               // Clear the planes/blockers
        }
    }

    void UnlockMapleArea()
    {
        mapleUnlocked = true;

        // Disable the physical blocker
        if (mapleBlockerPlane != null) 
            mapleBlockerPlane.SetActive(false);

        // Show the achievement popup
        if (achievementPopup != null)
            StartCoroutine(ShowAchievement());
    }

    IEnumerator ShowAchievement()
    {
        achievementPopup.SetActive(true);
        yield return new WaitForSeconds(popupDuration);
        achievementPopup.SetActive(false);
    }

    public void AddMaple(int amount)
    {
        mapleCount += amount;
        UpdateUI();
        // Achievement 3: 5,000 Maple (Grand House Goal)
        if (!t3Unlocked && mapleCount >= 5000)
        {
            t3Unlocked = true;
            cup3Canvas.SetActive(true);
            trophy3_Tycoon.SetActive(true);
        }
    }

    void UpdateUI()
    {
        if (oakText != null) oakText.text = $"Oak: {oakCount}";
        if (mapleText != null) mapleText.text = $"Maple: {mapleCount}";
    }
}
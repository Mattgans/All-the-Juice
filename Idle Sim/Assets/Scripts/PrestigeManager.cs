using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Handles prestige: resets all progress in exchange for a permanent multiplier.
/// Prestige unlocks once the player earns enough Maple.
/// Attach to a GameObject in the scene with a UI button wired to TryPrestige().
/// </summary>
public class PrestigeManager : MonoBehaviour
{
    public static PrestigeManager Instance;

    [Header("Prestige Settings")]
    [Tooltip("Minimum maple count required to prestige")]
    public int prestigeThreshold = 5000;
    public int prestigeCount = 0;
    [Tooltip("Permanent multiplier bonus per prestige level")]
    public float prestigeMultiplierBonus = 0.5f;

    [Header("UI")]
    public TextMeshProUGUI prestigeButtonText;
    public TextMeshProUGUI prestigeInfoText;
    public GameObject prestigePanel;

    [Header("Juicy Feedback")]
    public ParticleSystem prestigeParticles;
    public AudioSource prestigeAudio;
    [Tooltip("Additional particle systems around the scene to fire on prestige")]
    public ParticleSystem[] sceneWideParticles;

    [Header("Screen Flash")]
    public CanvasGroup flashOverlay;
    public float flashDuration = 1.5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (flashOverlay != null)
            flashOverlay.alpha = 0f;
        UpdatePrestigeUI();
    }

    /// <summary>
    /// Returns the current prestige multiplier (1.0 base + bonus per prestige).
    /// Generators should multiply their output by this value.
    /// </summary>
    public float GetPrestigeMultiplier()
    {
        return 1.0f + (prestigeCount * prestigeMultiplierBonus);
    }

    /// <summary>
    /// Returns the current maple count directly from ResourceManager.
    /// </summary>
    int GetCurrentMaple()
    {
        if (ResourceManager.Instance != null)
            return ResourceManager.Instance.mapleCount;
        return 0;
    }

    /// <summary>
    /// Wire this to your Prestige button's OnClick / WhenSelect event.
    /// </summary>
    public void TryPrestige()
    {
        int maple = GetCurrentMaple();
        if (maple < prestigeThreshold)
        {
            Debug.Log($"Need {prestigeThreshold} maple to prestige. Current: {maple}");
            return;
        }

        StartCoroutine(DoPrestige());
    }

    IEnumerator DoPrestige()
    {
        prestigeCount++;

        // --- Juicy feedback throughout the whole scene ---
        if (prestigeAudio != null)
            prestigeAudio.Play();

        if (prestigeParticles != null)
            prestigeParticles.Play();

        if (sceneWideParticles != null)
        {
            foreach (var ps in sceneWideParticles)
            {
                if (ps != null) ps.Play();
            }
        }

        if (flashOverlay != null)
        {
            yield return StartCoroutine(FlashScreen());
        }

        // --- Reset all game state ---
        ResetGameState();

        UpdatePrestigeUI();
        if (TycoonManager.Instance != null)
            TycoonManager.Instance.UpdateTycoonUI();

        Debug.Log($"Prestige! Now at level {prestigeCount}. Multiplier: {GetPrestigeMultiplier()}x");
    }

    void ResetGameState()
    {
        // Reset resources, achievements, trophies, and unlocks
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.ResetAll();
        }

        // Reset generators and multipliers
        if (TycoonManager.Instance != null)
        {
            TycoonManager.Instance.oakGenCount = 0;
            TycoonManager.Instance.oakProductionMultiplier = 1.0f;
            TycoonManager.Instance.mapleGenCount = 0;
            TycoonManager.Instance.mapleProductionMultiplier = 1.0f;

            // Hide all generator models
            foreach (var model in TycoonManager.Instance.oakGenModels)
            {
                if (model != null) model.SetActive(false);
            }
            foreach (var model in TycoonManager.Instance.mapleGenModels)
            {
                if (model != null) model.SetActive(false);
            }

            // Reset trail visuals back to tier 1
            TycoonManager.Instance.UpdateTycoonUI();
        }
    }

    IEnumerator FlashScreen()
    {
        float halfDuration = flashDuration / 2f;
        float timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            flashOverlay.alpha += 8f * (1f - flashOverlay.alpha) * Time.deltaTime;
            yield return null;
        }
        flashOverlay.alpha = 1f;

        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            flashOverlay.alpha += 6f * (0f - flashOverlay.alpha) * Time.deltaTime;
            yield return null;
        }
        flashOverlay.alpha = 0f;
    }

    void UpdatePrestigeUI()
    {
        if (prestigeButtonText != null)
        {
            int maple = GetCurrentMaple();
            if (maple >= prestigeThreshold)
                prestigeButtonText.text = "PRESTIGE!";
            else
                prestigeButtonText.text = $"Prestige\n({maple}/{prestigeThreshold})";
        }

        if (prestigeInfoText != null)
        {
            if (prestigeCount > 0)
                prestigeInfoText.text = $"Prestige: {prestigeCount}\nBonus: {GetPrestigeMultiplier()}x";
            else
                prestigeInfoText.text = "Prestige: 0\nBonus: 1.0x";
        }
    }

    void Update()
    {
        UpdatePrestigeUI();
    }
}

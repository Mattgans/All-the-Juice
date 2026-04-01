using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Spawns a free temporary power-up at random intervals.
/// The player must interact with it before it fades away.
/// Grants a temporary multiplier boost on all generation.
///
/// Hides the surge by moving it far off-screen so no components are toggled.
/// Particles are controlled via SetActive on their GameObjects.
/// </summary>
public class SurpriseSurge : MonoBehaviour
{
    public static SurpriseSurge Instance;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 45f;
    public float maxSpawnInterval = 90f;
    public float surgeDuration = 10f;

    [Header("Boost Settings")]
    public float surgeMultiplier = 3.0f;
    public float boostDuration = 15f;

    [Header("Surge Object")]
    public GameObject surgeObject;
    public TextMeshProUGUI surgeStatusText;

    [Header("Juicy Feedback - Spawn")]
    [Tooltip("Drag the spawn particle GameObject here (looping particle)")]
    public GameObject spawnParticleObject;
    public AudioSource spawnAudio;

    [Header("Juicy Feedback - Grab")]
    [Tooltip("Drag the grab/explosion particle GameObject here")]
    public GameObject grabParticleObject;
    public AudioSource grabAudio;

    [Header("Ease Animation")]
    public float easeSpeed = 8f;

    // State
    private bool surgeActive = false;
    private bool boostActive = false;
    private float currentBoostMultiplier = 1.0f;
    private float spawnTimer;
    private float surgeTimer;
    private float boostTimer;
    private Vector3 surgeOriginalScale;
    private Vector3 surgeSpawnPosition;
    private float pulsePhase = 0f;

    private static readonly Vector3 HIDDEN_POSITION = new Vector3(0f, -9999f, 0f);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (surgeObject != null)
        {
            surgeOriginalScale = surgeObject.transform.localScale;
            surgeSpawnPosition = surgeObject.transform.position;
            surgeObject.transform.position = HIDDEN_POSITION;
        }

        // Hide both particle objects at start
        if (spawnParticleObject != null) spawnParticleObject.SetActive(false);
        if (grabParticleObject != null) grabParticleObject.SetActive(false);

        ResetSpawnTimer();
        UpdateStatusText();
    }

    void Update()
    {
        if (!surgeActive && !boostActive)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
                SpawnSurge();
        }

        if (surgeActive)
        {
            surgeTimer -= Time.deltaTime;

            // Pulse using ease formula
            if (surgeObject != null)
            {
                pulsePhase += Time.deltaTime * 3f;
                float pulseFactor = 1f + Mathf.Sin(pulsePhase) * 0.15f;
                Vector3 targetScale = surgeOriginalScale * pulseFactor;
                Vector3 currentScale = surgeObject.transform.localScale;
                surgeObject.transform.localScale = currentScale + easeSpeed * (targetScale - currentScale) * Time.deltaTime;
            }

            if (surgeTimer <= 0f)
                DespawnSurge();
        }

        if (boostActive)
        {
            boostTimer -= Time.deltaTime;
            UpdateStatusText();
            if (boostTimer <= 0f)
                EndBoost();
        }
    }

    void SpawnSurge()
    {
        surgeActive = true;
        surgeTimer = surgeDuration;
        pulsePhase = 0f;

        // Hide grab particles from previous cycle
        if (grabParticleObject != null) grabParticleObject.SetActive(false);

        if (surgeObject != null)
        {
            surgeObject.transform.position = surgeSpawnPosition;
            surgeObject.transform.localScale = surgeOriginalScale;

            var grabTrigger = surgeObject.GetComponent<SurgeGrabTrigger>();
            if (grabTrigger != null)
                grabTrigger.OnSurgeSpawned();
        }

        // Turn on spawn particles (Play On Awake will make them emit)
        if (spawnParticleObject != null) spawnParticleObject.SetActive(true);
        if (spawnAudio != null) spawnAudio.Play();
    }

    /// <summary>
    /// Called when the player touches the surge object via SurgeGrabTrigger.
    /// </summary>
    public void GrabSurge()
    {
        if (!surgeActive) return;
        surgeActive = false;

        // Turn off spawn particles
        if (spawnParticleObject != null) spawnParticleObject.SetActive(false);

        // Turn on grab particles then auto-disable after they finish
        if (grabParticleObject != null)
        {
            grabParticleObject.SetActive(true);
            StartCoroutine(DisableAfterDelay(grabParticleObject, 2f));
        }
        if (grabAudio != null) grabAudio.Play();

        // Hide surge object
        if (surgeObject != null)
            surgeObject.transform.position = HIDDEN_POSITION;

        StartBoost();
    }

    void StartBoost()
    {
        boostActive = true;
        boostTimer = boostDuration;
        currentBoostMultiplier = surgeMultiplier;
        UpdateStatusText();
        Debug.Log($"Surge boost active! {surgeMultiplier}x for {boostDuration}s");
    }

    void EndBoost()
    {
        boostActive = false;
        currentBoostMultiplier = 1.0f;
        UpdateStatusText();
        ResetSpawnTimer();
        Debug.Log("Surge boost ended.");
    }

    void DespawnSurge()
    {
        surgeActive = false;

        // Turn off spawn particles
        if (spawnParticleObject != null) spawnParticleObject.SetActive(false);

        if (surgeObject != null)
            surgeObject.transform.position = HIDDEN_POSITION;

        ResetSpawnTimer();
    }

    IEnumerator DisableAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) obj.SetActive(false);
    }

    void ResetSpawnTimer()
    {
        spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    public float GetSurgeMultiplier()
    {
        return currentBoostMultiplier;
    }

    void UpdateStatusText()
    {
        if (surgeStatusText == null) return;

        if (boostActive)
            surgeStatusText.text = $"SURGE! {surgeMultiplier}x ({boostTimer:F0}s)";
        else
            surgeStatusText.text = "";
    }
}

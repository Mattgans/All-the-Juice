using UnityEngine;
using System.Collections;

public class GooglyEyeBlink : MonoBehaviour
{
    public GameObject eyeball;       // The whole eyeball (white + pupil)
    public GameObject eyeball2;  
    public float minBlinkDelay = 2f; // Minimum time between blinks
    public float maxBlinkDelay = 5f; // Maximum time between blinks
    public float blinkDuration = 0.1f; // How long the eye disappears

    void Start()
    {
        if (eyeball != null)
            StartCoroutine(BlinkLoop());
    }

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            // Wait a random amount of time before blinking
            float waitTime = Random.Range(minBlinkDelay, maxBlinkDelay);
            yield return new WaitForSeconds(waitTime);

            // “Blink” by hiding the eyeball
            eyeball.SetActive(false);
            eyeball2.SetActive(false);
            yield return new WaitForSeconds(blinkDuration);

            // Re-enable eyeball
            eyeball.SetActive(true);
            eyeball2.SetActive(true);
        }
    }
}
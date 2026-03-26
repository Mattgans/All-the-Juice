using UnityEngine;
using Oculus.Haptics;

public class HapticTest : MonoBehaviour
{
    // Drag your Haptic Source component here in the inspector
    public HapticSource hapticSource;

    void Start()
    {
        if (hapticSource != null)
        {
            // This will trigger the vibration as soon as the axe is "born" in the scene
            hapticSource.Play(); 
            Debug.Log("Auto-playing haptic source for testing...");
        }
    }
}
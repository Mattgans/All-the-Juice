using UnityEngine;
using Oculus.Haptics;

// Rename the class here to avoid the conflict
public class HapticInitializer : MonoBehaviour
{
    private void Awake()
    {
        try 
        {
            // This 'kicks' the Haptics Instance to life
            var init = Haptics.Instance;
            Debug.Log("Haptics SDK Initialized Successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Haptics SDK failed to initialize: " + e.Message);
        }
    }
}
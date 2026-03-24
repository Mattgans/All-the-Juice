using UnityEngine;

public class HouseToggler : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Drag your Baker_house object here from the Hierarchy")]
    public GameObject bakerHouse;

    /// <summary>
    /// This method turns the house ON. 
    /// You can link this to your Button's 'When Select()' event.
    /// </summary>
    public void EnableHouse()
    {
        if (bakerHouse != null)
        {
            bakerHouse.SetActive(true);
            Debug.Log("Baker House has been enabled!");
        }
        else
        {
            Debug.LogError("Baker House reference is missing on the HouseToggler script!");
        }
    }

    /// <summary>
    /// Optional: This method flips the house on and off each time you click.
    /// </summary>
    public void ToggleHouse()
    {
        if (bakerHouse != null)
        {
            bool currentState = bakerHouse.activeSelf;
            bakerHouse.SetActive(!currentState);
        }
    }
}
using UnityEngine;
using System.Collections;

public class GooglyEye : MonoBehaviour
{
    [Header("References")]
    public Transform target;   // Main Camera
    public Transform pupil;    // black cylinder

    [Header("Look Settings")]
    public float moveAmount = 0.1f; // how far pupil can move
    public float speed = 5f;

   
    private Vector3 startLocalPos;
    private Vector3 originalScale;


    void Start()
    {
        startLocalPos = pupil.localPosition;
        originalScale = transform.localScale;
       
    }

    void Update()
    {
        if (target == null || pupil == null) return;

        // Direction from eye to player
        Vector3 worldDir = target.position - transform.position;

        // Convert to local space
        Vector3 localDir = transform.InverseTransformDirection(worldDir);

        // Normalize and scale
        localDir = localDir.normalized;

        // Only move in X and Y (not Z)
        Vector3 targetOffset = new Vector3(localDir.x, localDir.y, 0) * moveAmount;

        // Smooth movement
        pupil.localPosition = Vector3.Lerp(
            pupil.localPosition,
            startLocalPos + targetOffset,
            Time.deltaTime * speed
        );
    }
}
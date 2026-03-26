using UnityEngine;
using Oculus.Haptics;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using System.Collections;

public class AxeBehavior : MonoBehaviour
{
    public float velocityThreshold = 1.2f; 
    public string treeTag = "Tree";
    private Rigidbody rb;

    [Header("Haptic Settings")]
    public float duration;
    public float amplitude;
    public float frequency;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(treeTag))
        {
            if (rb != null && rb.linearVelocity.magnitude > velocityThreshold)
            {
                ResourceTree tree = other.GetComponentInParent<ResourceTree>();
                if (tree != null)
                {
                    tree.GetHit();
                    TriggerHaptics();
                    Debug.Log("Meta Axe hit!");
                }
            }
        }
    }

    public Transform leftHand;
    public Transform rightHand;

    private bool isHeldLeft = false;
    private bool isHeldRight = false;

    void Update()
    {
        // Simple distance check
        float leftDist = Vector3.Distance(transform.position, leftHand.position);
        float rightDist = Vector3.Distance(transform.position, rightHand.position);

        if (leftDist < 0.2f)
            isHeldLeft = true;
        else
            isHeldLeft = false;

        if (rightDist < 0.2f)
            isHeldRight = true;
        else
            isHeldRight = false;
    }

    public void TriggerHaptics()
    {
        if (isHeldRight)
        {
            StartCoroutine(TriggerHapticsRoutine(OVRInput.Controller.RTouch));
        }
        if (isHeldLeft)
            StartCoroutine(TriggerHapticsRoutine(OVRInput.Controller.LTouch));
    }

    public IEnumerator TriggerHapticsRoutine(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0,0,controller);
    }
}
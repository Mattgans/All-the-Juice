using UnityEngine;

public class AxeBehavior : MonoBehaviour
{
    public float velocityThreshold = 1.2f; 
    public string treeTag = "Tree";
    private Rigidbody rb;
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
                    Debug.Log("Meta Axe hit!");
                }
            }
        }
    }
}
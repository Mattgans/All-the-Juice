using UnityEngine;
using System.Collections;

public class ResourceTree : MonoBehaviour
{
    public enum TreeType { Oak, Maple }

    [Header("Tree Setup")]
    public TreeType typeOfTree; // Select Oak or Maple in the Inspector
    public int yieldAmount = 10;
    public float respawnTime = 10f;
    public float hitCooldown = 0.5f;

    [Header("Visuals")]
    public GameObject fullTreeModel;
    public GameObject cutStumpModel; 
    public Animator treeAnimator; 

    private int hitsRemaining;
    private bool isAvailable = true;
    private bool canBeHit = true;
    private Collider treeCollider;

    void Start()
    {
        treeCollider = GetComponent<Collider>();
        if (treeAnimator == null) treeAnimator = GetComponentInChildren<Animator>();
        ResetTree();
    }

    public void GetHit()
    {
        if (!isAvailable || !canBeHit) return;

        hitsRemaining--;
        
        if (treeAnimator != null)
            treeAnimator.Play("tree_wig", 0, 0f); 

        if (hitsRemaining <= 0)
            StartCoroutine(HandleHarvest());
        else
            StartCoroutine(CooldownTimer());
    }

    IEnumerator CooldownTimer()
    {
        canBeHit = false;
        yield return new WaitForSeconds(hitCooldown);
        canBeHit = true;
    }

    IEnumerator HandleHarvest()
    {
        isAvailable = false;
        
        // Check the Enum to decide which method to call
        if (ResourceManager.Instance != null)
        {
            if (typeOfTree == TreeType.Oak)
                ResourceManager.Instance.AddOak(yieldAmount);
            else if (typeOfTree == TreeType.Maple)
                ResourceManager.Instance.AddMaple(yieldAmount);
        }

        fullTreeModel.SetActive(false);
        cutStumpModel.SetActive(true);
        if (treeCollider != null) treeCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);
        ResetTree();
    }

    void ResetTree()
    {
        hitsRemaining = Random.Range(3, 6);
        isAvailable = true;
        canBeHit = true;
        fullTreeModel.SetActive(true);
        cutStumpModel.SetActive(false);
        if (treeCollider != null) treeCollider.enabled = true;
    }
}
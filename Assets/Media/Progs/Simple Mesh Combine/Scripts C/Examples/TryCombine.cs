using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryCombine : MonoBehaviour
{
    private SimpleMeshCombine simpleMeshCombine;
    private bool isCombined;

    public void Awake()
    {
        simpleMeshCombine = GetComponent<SimpleMeshCombine>();
        //simpleMeshCombine.CombineMeshes();
    }

    private void Update()
    {
        if (!isCombined)
        {
            simpleMeshCombine = GetComponent<SimpleMeshCombine>();
            isCombined = true;

            Combine();
        }
    }

    public void Start()
    {

        if (simpleMeshCombine == null)
        {
            Debug.Log("Couldn't find SMC, aborting");
            return;
        }
        
    }

    public void Combine()
    {
        simpleMeshCombine.CombineMeshes();
        Debug.Log("Combined");
    }

    public void Release()
    {
        simpleMeshCombine.EnableRenderers(true);
        if (simpleMeshCombine.combined == null) return;
        Destroy(simpleMeshCombine.combined);
        simpleMeshCombine.combinedGameOjects = null;
        Debug.Log("Released");
    }
}

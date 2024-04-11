using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerOptions : MonoBehaviour
{
    [SerializeField] private LineRenderer[] lines;
    [SerializeField] private GameObject nonDeleter;
    [SerializeField] private GameObject deleter;

    private GameManager gm;
    private BlockManager bm;

    private bool isBuildingNotDeleting;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        bm = gm.BlockManager;

        isBuildingNotDeleting = !bm.IsBuildingBlocks;

        for (int i = 0; i < lines.Length; i++)
        {
            if (Globals.IsMobile)
            {
                lines[i].startWidth = 1.2f;
                lines[i].endWidth = 1.2f;
                lines[i].textureScale = new Vector2(0.3f, 1);
            }
            else
            {
                lines[i].startWidth = 0.75f;
                lines[i].endWidth = 0.75f;
                lines[i].textureScale = new Vector2(0.5f, 1);
            }
        }
    }

    private void Update()
    {
        if (isBuildingNotDeleting != bm.IsBuildingBlocks)
        {
            if (bm.IsBuildingBlocks || bm.IsChoosingBlocks)
            {
                isBuildingNotDeleting = true;
                makeYellow();
                nonDeleter.SetActive(true);
                deleter.SetActive(false);
            }
            else if (bm.IsDestroingBlocks)
            {
                isBuildingNotDeleting = false;
                makeRed();
                nonDeleter.SetActive(false);
                deleter.SetActive(true);
            }
        }
    }

    private void makeRed()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].startColor = Color.red;
            lines[i].endColor = Color.red;
        }
    }

    private void makeYellow()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].startColor = Color.yellow;
            lines[i].endColor = Color.yellow;
        }
    }
}

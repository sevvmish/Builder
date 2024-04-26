using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSides : MonoBehaviour
{
    [SerializeField] private Transform[] sides;

    private Block block;
    private bool isFinalized;
    private float delta;
    private GameManager gm;
    private BlockManager bm;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance == null)
        {
            this.enabled = false;
            return;
        }

        gm = GameManager.Instance;
        bm = gm.BlockManager;
        block = GetComponent<Block>();
        delta = block.Volume.x;
    }

    private void Update()
    {

        if (!isFinalized && block.IsFinalized)
        {
            isFinalized = true;
            CheckBorders();
        }
    }

    public void CheckBorders()
    {
        if (sides.Length <= 0 || sides == null) return;

        List<Block> blocks = new List<Block>();

        for (int i = 0; i < bm.ReadyBlocks.Count; i++)
        {
            if (bm.ReadyBlocks[i].BlockType == BlockTypes.garden_ground && (bm.ReadyBlocks[i].transform.position - transform.position).magnitude == delta)
            {
                blocks.Add(bm.ReadyBlocks[i]);
            }
        }

        if (blocks.Count > 0)
        {
            for (int i = 0; i < sides.Length; i++)
            {
                if (!sides[i].gameObject.activeSelf) continue;

                for (int j = 0; j < blocks.Count; j++)
                {
                    if (blocks[j].TryGetComponent(out BlockSides b))
                    {
                        Vector3 another = blocks[j].transform.position;
                        if ((sides[i].transform.position - new Vector3(another.x + delta / 2f, another.y, another.z)).magnitude <= 0.4f)
                        {
                            sides[i].gameObject.SetActive(false);
                            b.CheckBorders();
                        }
                        if ((sides[i].transform.position - new Vector3(another.x - delta / 2f, another.y, another.z)).magnitude <= 0.4f)
                        {
                            sides[i].gameObject.SetActive(false);
                            b.CheckBorders();
                        }
                        if ((sides[i].transform.position - new Vector3(another.x, another.y, another.z + delta / 2f)).magnitude <= 0.4f)
                        {
                            sides[i].gameObject.SetActive(false);
                            b.CheckBorders();
                        }
                        if ((sides[i].transform.position - new Vector3(another.x, another.y, another.z - delta / 2f)).magnitude <= 0.4f)
                        {
                            sides[i].gameObject.SetActive(false);
                            b.CheckBorders();
                        }
                    }
                    
                }

                
            }
        }
    }

}

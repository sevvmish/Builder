using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage
{
    public Dictionary<int, int> BlocksByTypes;
    public List<Block> Blocks;

    public Stage()
    {
        Blocks = new List<Block> ();
        BlocksByTypes = new Dictionary<int, int> ();
    }

    public bool IsStageDone()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            if (!Blocks[i].IsFinalized)
            {
                return false;
            }
        }

        return true;
    }

    public void Assess()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            if (!BlocksByTypes.ContainsKey(Blocks[i].ID.ID))
            {
                BlocksByTypes.Add(Blocks[i].ID.ID, 1);
            }
            else
            {
                BlocksByTypes[Blocks[i].ID.ID]++;
            }
        }
    }
}

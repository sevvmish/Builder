using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActInRuntime : MonoBehaviour
{
    [SerializeField] private GameObject[] toEnable;
    [SerializeField] private GameObject[] toDisable;
    [SerializeField] private MeshRenderer[] toHide;

    private void Awake()
    {
        if (toEnable.Length > 0)
        {
            for (int i = 0; i < toEnable.Length; i++)
            {
                toEnable[i].SetActive(true);
            }
        }

        if (toDisable.Length > 0)
        {
            for (int i = 0; i < toDisable.Length; i++)
            {
                toDisable[i].SetActive(false);
            }
        }

        if (toHide.Length > 0)
        {
            for (int i = 0; i < toHide.Length; i++)
            {
                toHide[i].enabled = false;
            }
        }
    }
}

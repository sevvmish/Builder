using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Anchors : MonoBehaviour
{
    public Transform[] AnchorsPoints { get => anchors; }
    [SerializeField] private Transform[] anchors;

    private void OnEnable()
    {
        List<Transform> a = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.layer == 8)
            {
                a.Add(transform.GetChild(i));
            }
        }

        anchors = a.ToArray();
    }
}

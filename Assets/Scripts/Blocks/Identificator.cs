using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identificator : MonoBehaviour
{
    [SerializeField] private int id;
    private int uniqueID;

    public int ID { get => id; }
    public int UniqueID { get => uniqueID; }

    private void OnEnable()
    {
        uniqueID = Random.Range(0, 1000000);
    }

}

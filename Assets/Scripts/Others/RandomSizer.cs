using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSizer : MonoBehaviour
{
    [SerializeField] private float deltaX;
    [SerializeField] private float deltaY;
    [SerializeField] private float deltaZ;

    private void OnEnable()
    {
        float x = 0, y = 0, z = 0;

        if (deltaX != 0)
        {
            x = UnityEngine.Random.Range(-deltaX, deltaX);
        }

        if (deltaY != 0)
        {
            y = UnityEngine.Random.Range(-deltaY, deltaY);
        }

        if (deltaZ != 0)
        {
            z = UnityEngine.Random.Range(-deltaZ, deltaZ);
        }

        Vector3 vec = transform.localScale;
        transform.localScale = new Vector3(transform.localScale.x + x, transform.localScale.y + y, transform.localScale.z + z);
    }

}

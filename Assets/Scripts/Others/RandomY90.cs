using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomY90 : MonoBehaviour
{
    private void OnEnable()
    {
        int rnd = UnityEngine.Random.Range(0, 4);

        switch(rnd)
        {
            case 0:
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
                break;

            case 1:
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 90, transform.localEulerAngles.z);
                break;

            case 2:
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180, transform.localEulerAngles.z);
                break;

            case 3:
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 270, transform.localEulerAngles.z);
                break;
        }
    }
}

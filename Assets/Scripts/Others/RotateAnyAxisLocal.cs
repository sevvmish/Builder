using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnyAxisLocal : MonoBehaviour
{
    [SerializeField] private Axises axisToRotate = Axises.X_axis;
    [SerializeField] private float speed = 10f;

    float ax;
    Vector3 initAxises;

    private void Start()
    {
        initAxises = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        ax += speed * Time.deltaTime;

        switch(axisToRotate)
        {
            case Axises.X_axis:
                transform.localEulerAngles = new Vector3(ax, initAxises.y, initAxises.z);

                break;

            case Axises.Y_axis:
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, ax, transform.localEulerAngles.z);
                break;

            case Axises.Z_axis:
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, ax);
                break;
        }
    }
}

public enum Axises
{
    X_axis,
    Y_axis,
    Z_axis
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeShow : MonoBehaviour
{
    [SerializeField] private Transform location;
    private float speed = 25f;

    float ax;
    int index = 1;

    // Start is called before the first frame update
    void Start()
    {
        location.GetChild(1).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        ax += -speed * Time.deltaTime;
        transform.localEulerAngles = new Vector3(0, ax, 0);

        if (Input.GetKeyDown(KeyCode.V))
        {
            ax = 30;
            resetAll();
            index++;
            location.GetChild(index).gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ax = 30;
            /*
            resetAll();
            index--;
            location.GetChild(index).gameObject.SetActive(true);
            */
        }
    }

    private void resetAll()
    {
        for (int i = 0; i < location.childCount; i++)
        {
            location.GetChild(i).gameObject.SetActive(false);
        }
    }
}

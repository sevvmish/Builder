using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkImageUI : MonoBehaviour
{
    [SerializeField] private float delta = 0.1f;
    [SerializeField] private float delay = 0.1f;
    [SerializeField] private float upValue = 1f;
    [SerializeField] private float downValue = 0;

    private Image image;
    private Vector3 baseColor;
    private WaitForSeconds waiter;

    private void Start()
    {
        waiter = new WaitForSeconds(delay);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        image = GetComponent<Image>();
        baseColor = new Vector3(image.color.r, image.color.g, image.color.b);
        StartCoroutine(playT());
    }

    private IEnumerator playT()
    {
        float alpha = upValue;

        while (true)
        {            
            image.color = new Color(baseColor.x, baseColor.y, baseColor.z, alpha);
            yield return waiter;
            alpha -= delta;
            if (alpha < downValue)
            {
                alpha = upValue;
            }
        }
    }
}

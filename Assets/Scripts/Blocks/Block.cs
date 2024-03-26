using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockTypes BlockType { get => blockType; }
    public Sprite BlockIcon { get => blockIcon; }
    public bool IsGoodToFinalize => (prototypeViewGood.activeSelf && !prototypeViewBad.activeSelf);

    [SerializeField] private BlockTypes blockType;
    [SerializeField] private Sprite blockIcon;
    [SerializeField] private GameObject realView;
    [SerializeField] private GameObject prototypeViewGood;
    [SerializeField] private GameObject prototypeViewBad;
    [SerializeField] private GameObject buildVFX;

    private Transform _transform;

    
    private void OnEnable()
    {
        _transform = transform;
    }

    public void MakeColorGood()
    {
        hideAll();
        prototypeViewGood.SetActive(true);
    }

    public void MakeColorBad()
    {
        hideAll();
        prototypeViewBad.SetActive(true);
    }

    public void MakeFinalView()
    {        
        hideAll();
        realView.SetActive(true);
    }

    private void hideAll()
    {
        realView.SetActive(false);
        prototypeViewGood.SetActive(false);
        prototypeViewBad.SetActive(false);
        buildVFX.SetActive(false);
    }

    public void SetPosition(Vector3 markerPoint)
    {
        assessRightPosition(markerPoint);

        Collider[] colliders = Physics.OverlapBox(_transform.position, getBoxForBlockCheck(), _transform.rotation);

        bool isBad = false;
        
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {

                if (colliders[i].gameObject.layer == 3 || colliders[i].gameObject.layer == 7)
                {                    
                    isBad = true;
                    break;
                }
            }
        }
        else
        {
            isBad = true;
        }

        if (isBad)
        {
            MakeColorBad();
        }
        else
        {
            MakeColorGood();
        }
    }

    private void assessRightPosition(Vector3 markerPoint)
    {
        switch(BlockType)
        {
            case BlockTypes.floor:
                //_transform.position = new Vector3(getNearest05(markerPoint.x), getNearest05(markerPoint.y), getNearest05(markerPoint.z));
                _transform.position = new Vector3(Mathf.Round(markerPoint.x), Mathf.Round(markerPoint.y), Mathf.Round(markerPoint.z));
                break;
        }
    }

    private float getNearest05(float val)
    {
        val = System.MathF.Round(val, 1);

        if (val % 0.5f == 0) return val;

        for (int i = 1; i < 10; i++)
        {
            float newVal1 = val + 0.1f * i;
            float newVal2 = val - 0.1f * i;

            if (newVal1 % 0.5f == 0) return newVal1;
            if (newVal2 % 0.5f == 0) return newVal2;
        }

        return 0;
    }

    private Vector3 getBoxForBlockCheck()
    {
        switch(BlockType)
        {
            case BlockTypes.floor:
                return new Vector3(2, 0.1f, 2) * 0.95f / 2f;
                
        }

        return Vector3.zero;
    }
}

public enum BlockTypes
{
    none,
    floor,
    wall,
    roof
}

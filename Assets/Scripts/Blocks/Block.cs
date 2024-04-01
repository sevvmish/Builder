using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockTypes BlockType { get => blockType; }
    public Identificator ID { get => id; }
    public Sprite BlockIcon { get => blockIcon; }
    public bool IsRotatable { get => isRotatable; }
    public bool IsGoodToFinalize => (prototypeViewGood.activeSelf && !prototypeViewBad.activeSelf);
    public bool IsFinalized => realView.activeSelf;
    public DeltaDimentions DeltaStep { get => deltaStep; }
    public float RotationAngle { get => rotationAngle; }

    [SerializeField] private BlockTypes blockType;

    [SerializeField] private Sprite blockIcon;
    [SerializeField] private GameObject realView;
    [SerializeField] private GameObject prototypeViewGood;
    [SerializeField] private GameObject prototypeViewBad;
    [SerializeField] private GameObject buildVFX;

    [SerializeField] private DeltaDimentions deltaStep = DeltaDimentions.all_1;
    [SerializeField] private float rotationAngle = 90;
    [SerializeField] private bool isRotatable;

    private Transform _transform;
    private GameManager gm;
    private BlockManager blockManager;
    private Identificator id;

    private Vector3 lastMarkerPosition;
    
    private void OnEnable()
    {
        _transform = transform;
        gm = GameManager.Instance;
        blockManager = gm.BlockManager;
        id = GetComponent<Identificator>();
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

    public void MakeColorBadForDelete(bool isActive)
    {        
        prototypeViewBad.SetActive(isActive);
    }

    public void MakeFinalView()
    {        
        hideAll();
        realView.SetActive(true);
    }

    public bool Rotate(ref Vector3 endRotationVector)
    {
        if (!IsRotatable) return false;

        Vector3 newVector = _transform.eulerAngles + new Vector3(0, rotationAngle, 0);

        _transform.DORotate(newVector, 0.1f).SetEase(Ease.Linear);
        endRotationVector = newVector;
        return true;
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
        if (IsFinalized) return;

        if (lastMarkerPosition == markerPoint) return;

        assessRightPositionVector(markerPoint);

        switch(BlockType)
        {
            case BlockTypes.floor:
                assessBlockStatusFloor();
                break;

            case BlockTypes.wall:
                assessBlockStatusWall();
                break;
        }

        lastMarkerPosition = markerPoint;
    }

    private void assessBlockStatusFloor()
    {
        Collider[] colliders = Physics.OverlapBox(_transform.position, getBoxForBlockCheck(), _transform.rotation);

        bool isBad = false;

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {                
                if (colliders[i].gameObject.layer == 3)
                {
                    isBad = true;
                    gm.GetUI.PlayerCrossNewBlockError();
                    break;
                }

                if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this) && b.blockType == BlockTypes.floor)
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

    private void assessBlockStatusWall()
    {
        Collider[] colliders = Physics.OverlapBox(_transform.position + Vector3.up, getBoxForBlockCheck(), _transform.rotation);

        bool isBad = false;

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {

                if (colliders[i].gameObject.layer == 3)
                {
                    isBad = true;
                    gm.GetUI.PlayerCrossNewBlockError();
                    break;
                }

                if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this) && b.blockType == BlockTypes.wall)
                {
                    isBad = true;
                    break;
                }
            }
        }
        else
        {
            isBad = false;
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

    private void assessRightPositionVector(Vector3 markerPoint)
    {        
        switch (deltaStep)
        {
            case DeltaDimentions.all_05:
                _transform.position = new Vector3(getNearest05(markerPoint.x), getNearest05(markerPoint.y), getNearest05(markerPoint.z));
                break;

            case DeltaDimentions.all_1:
                _transform.position = new Vector3(Mathf.Round(markerPoint.x), Mathf.Round(markerPoint.y), Mathf.Round(markerPoint.z));
                break;

            case DeltaDimentions.all_15:
                _transform.position = new Vector3(getNearest15(markerPoint.x), getNearest15(markerPoint.y), getNearest15(markerPoint.z));
                break;

            case DeltaDimentions.all_2:
                _transform.position = new Vector3(getNearest2(markerPoint.x), getNearest2(markerPoint.y), getNearest2(markerPoint.z));
                break;
        }


        /*
        switch(BlockType)
        {
            case BlockTypes.floor:
                //_transform.position = new Vector3(getNearest05(markerPoint.x), getNearest05(markerPoint.y), getNearest05(markerPoint.z));
                _transform.position = new Vector3(Mathf.Round(markerPoint.x), Mathf.Round(markerPoint.y), Mathf.Round(markerPoint.z));
                break;

            case BlockTypes.wall:
                //_transform.position = new Vector3(getNearest05(markerPoint.x), getNearest05(markerPoint.y), getNearest05(markerPoint.z));
                _transform.position = new Vector3(Mathf.Round(markerPoint.x), Mathf.Round(markerPoint.y), Mathf.Round(markerPoint.z));
                break;
        }*/
    }

    private Vector3 getBoxForBlockCheck()
    {
        switch (BlockType)
        {
            case BlockTypes.floor:
                return new Vector3(1.5f, 0.1f, 1.5f) / 2f;

            case BlockTypes.wall:
                return new Vector3(1.5f, 1.5f, 0.2f) / 2f;

        }

        return Vector3.zero;
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

    private float getNearest15(float val)
    {
        val = System.MathF.Round(val, 1);

        if (val % 1.5f == 0) return val;

        for (int i = 1; i < 10; i++)
        {
            float newVal1 = val + 0.1f * i;
            float newVal2 = val - 0.1f * i;

            if (newVal1 % 1.5f == 0) return newVal1;
            if (newVal2 % 1.5f == 0) return newVal2;
        }

        return 0;
    }

    private float getNearest2(float val)
    {
        val = System.MathF.Round(val, 1);

        if (val % 2f == 0) return val;

        for (int i = 1; i < 10; i++)
        {
            float newVal1 = val + 0.1f * i;
            float newVal2 = val - 0.1f * i;

            if (newVal1 % 2f == 0) return newVal1;
            if (newVal2 % 2f == 0) return newVal2;
        }

        return 0;
    }


}

public enum DeltaDimentions
{
    all_05,
    all_1,
    all_15,
    all_2
}
public enum BlockTypes
{
    none,
    floor,
    wall,
    roof
}

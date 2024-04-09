using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Anchors Anchors { get => anchors; }

    public BlockSizes BlockSize { get => blockSize; }
    public MaterialTypes MaterialType { get => materialType; }

    [SerializeField] private BlockTypes blockType;

    [SerializeField] private Sprite blockIcon;
    [SerializeField] private GameObject realView;
    [SerializeField] private GameObject prototypeViewGood;
    [SerializeField] private GameObject prototypeViewBad;
    [SerializeField] private GameObject buildVFX;
    [SerializeField] private Vector3 sizeCheker = Vector3.one;
    [SerializeField] private Vector3 deltaCheker = Vector3.zero;

    [SerializeField] private DeltaDimentions deltaStep = DeltaDimentions.all_1;
    [SerializeField] private float rotationAngle = 90;
    [SerializeField] private bool isRotatable;
    [SerializeField] private BoxCollider[] colliders;

    [SerializeField] private BlockSizes blockSize;
    [SerializeField] private MaterialTypes materialType;

    private Transform _transform;
    private GameManager gm;
    private BlockManager blockManager;
    private Identificator id;    
    private Anchors anchors;
    private LayerMask anchorMask;

    private Vector3 lastMarkerPosition;
    
    private void OnEnable()
    {
        _transform = transform;
        gm = GameManager.Instance;
        blockManager = gm.BlockManager;
        id = GetComponent<Identificator>();
        SetColliders(true);
        anchors = GetComponent<Anchors>();
        anchorMask = LayerMask.GetMask(new string[] { "anchor" });
    }

    private void SetColliders(bool isActive)
    {
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].enabled != isActive) colliders[i].enabled = isActive;
            }
        }
    }

    public void MakeColorGood()
    {        
        hideAll();
        SetColliders(false);
        prototypeViewGood.SetActive(true);
    }

    public void MakeColorBad()
    {
        hideAll();
        SetColliders(false);
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
        SetColliders(true);
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

        switch (BlockType)
        {
            case BlockTypes.floor:
                assessBlockStatusFloor();
                break;

            case BlockTypes.wall:
                assessBlockStatusWall();
                break;

            case BlockTypes.roof:
                assessBlockStatusRoof();
                break;

            case BlockTypes.stair:
                assessBlockStatusStair();
                break;

            case BlockTypes.fence:
                assessBlockStatusFence();
                break;

            case BlockTypes.beam:
                assessBlockStatusBeam();
                break;
        }

        lastMarkerPosition = markerPoint;
    }

    private void assessBlockStatusBeam()
    {
        Collider[] colliders = Physics.OverlapBox(_transform.position + Vector3.up * 2 + deltaCheker, getBoxForBlockCheck(), _transform.rotation);

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
                else if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this) && BlockType == b.blockType)
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

    private void assessBlockStatusFence()
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
                else if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this) && BlockType == b.blockType)
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

    private void assessBlockStatusStair()
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
                else if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this) && BlockType == b.blockType)
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

    private void chechAnchors()
    {        
        float minDist = 1000;
        Transform anotherAnchor = default;
        Transform myAnchor = default;

        for (int t = 0; t < anchors.AnchorsPoints.Length; t++)
        {
            Collider[] colliders = Physics.OverlapSphere(anchors.AnchorsPoints[t].position, 2, anchorMask);

            if (colliders.Length > 0)
            {                
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (anchors.AnchorsPoints.Contains(colliders[i].transform)) continue;

                    float distance = (anchors.AnchorsPoints[t].position - colliders[i].transform.position).magnitude;
                    if (distance <= 1.5f && distance < minDist)
                    {
                        myAnchor = anchors.AnchorsPoints[t];
                        minDist = distance;
                        anotherAnchor = colliders[i].transform;
                    }                    
                }

                if (anotherAnchor != null)
                {
                    _transform.position = anotherAnchor.position + (_transform.position - myAnchor.position);
                }
            }
        }        
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
                else if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) )
                {                    
                    if (b.blockType == BlockTypes.wall)
                    {                        
                        Vector3 dir = Vector3.zero;
                        int sign = 0;

                        if (b.transform.eulerAngles.y == 0 || b.transform.eulerAngles.y == 180 || b.transform.eulerAngles.y == 360)
                        {
                            dir = new Vector3(0, 0, 1);
                            sign = (gm.GetMainPlayerTransform().position.z - _transform.position.z) > 0 ? 1 : -1;
                        }
                        else
                        {
                            dir = new Vector3(1, 0, 0);
                            sign = (gm.GetMainPlayerTransform().position.x - _transform.position.x) > 0 ? 1 : -1;
                        }


                        bool result = pushBlockToGoodPlace(b, dir, sign);
                        
                        if (result)
                        {
                            colliders = Physics.OverlapBox(_transform.position, getBoxForBlockCheck(), _transform.rotation);
                            if (!isCertainBlockTypeInArray(colliders, BlockTypes.wall) && !isCertainBlockTypeInArray(colliders, BlockTypes.floor))
                            {
                                isBad = false;
                                break;
                            }
                            else
                            {
                                isBad = true;
                                break;
                            }

                            /*
                            colliders = Physics.OverlapBox(_transform.position, getBoxForBlockCheck(), _transform.rotation);
                            
                            if (colliders.Length == 0)
                            {
                                isBad = false;
                                break;
                            }*/
                                
                        }
                        else
                        {
                            isBad = true;
                            break;
                        }
                    }
                    else if (b.blockType == BlockTypes.floor && !b.Equals(this))
                    {
                        Vector3 dir = Vector3.zero;
                        int sign = 0;

                        if (Mathf.Abs(_transform.position.x - b.transform.position.x) > Mathf.Abs(_transform.position.z - b.transform.position.z))                            
                        {
                            dir = new Vector3(1, 0, 0);
                            sign = (_transform.position.x - b.transform.position.x) > 0 ? 1 : -1;
                        }
                        else
                        {
                            dir = new Vector3(0, 0, 1);
                            sign = (_transform.position.z - b.transform.position.z) > 0 ? 1 : -1;
                        }


                        bool result = pushBlockToGoodPlace(b, dir, sign);

                        if (result)
                        {
                            
                            colliders = Physics.OverlapBox(_transform.position, getBoxForBlockCheck(), _transform.rotation);
                            if (!isCertainBlockTypeInArray(colliders, BlockTypes.wall) 
                                && !isCertainBlockTypeInArray(colliders, BlockTypes.floor))
                            {
                                isBad = false;
                                break;
                            }
                            else
                            {
                                isBad = true;
                                break;
                            }
                        }
                        else
                        {
                            isBad = true;
                            break;
                        }

                    }
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

    private bool pushBlockToGoodPlace(Block b, Vector3 direction, int delta)
    {
        Vector3 initPos = _transform.position;

        int sign = delta;


        for (int i = 0; i < 5; i++)
        {
            _transform.position += direction * sign;

            Collider[] colliders = Physics.OverlapBox(_transform.position, getBoxForBlockCheck(), _transform.rotation);

            bool isIn = false;

            if (colliders.Length > 0)
            {
                for (int j = 0; j < colliders.Length; j++)
                {                    
                    if (colliders[j].gameObject.Equals(b.gameObject))
                    {
                        isIn = true;
                        break;
                    }
                }
            }
            

            if (!isIn)
            {
                return true;
            }
        }

        _transform.position = initPos;
        return false;
    }


    private void assessBlockStatusWall()
    {
        Collider[] colliders = Physics.OverlapBox(_transform.position + Vector3.up*2, getBoxForBlockCheck(), _transform.rotation);

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
                    Vector3 dir = Vector3.zero;
                    int sign = 0;
                                        

                    if (b.transform.eulerAngles.y == 0 || b.transform.eulerAngles.y == 180 || b.transform.eulerAngles.y == 360)
                    {
                        

                        if (Mathf.Abs(_transform.position.y - b.transform.position.y) > Mathf.Abs(_transform.position.x - b.transform.position.x))
                        {
                            _transform.position = new Vector3(_transform.position.x, b.transform.position.y + 4, _transform.position.z);
                            colliders = Physics.OverlapBox(_transform.position + Vector3.up*2, getBoxForBlockCheck(), _transform.rotation);
                            if (!isCertainBlockTypeInArray(colliders, BlockTypes.wall))
                            {
                                isBad = false;
                                break;
                            }
                            else
                            {
                                isBad = true;
                                break;
                            }
                        }
                        else if (_transform.position.x > b.transform.position.x)
                        {
                            dir = new Vector3(1, 0, 0);
                            sign = 1;
                        }
                        else if (_transform.position.x <= b.transform.position.x)
                        {
                            dir = new Vector3(1, 0, 0);
                            sign = -1;
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(_transform.position.y - b.transform.position.y) > Mathf.Abs(_transform.position.z - b.transform.position.z))
                        {
                            _transform.position = new Vector3(_transform.position.x, b.transform.position.y + 4, _transform.position.z);
                            colliders = Physics.OverlapBox(_transform.position + Vector3.up*2, getBoxForBlockCheck(), _transform.rotation);
                            if (!isCertainBlockTypeInArray(colliders, BlockTypes.wall))
                            {
                                isBad = false;
                                break;
                            }
                            else
                            {
                                isBad = true;
                                break;
                            }

                        }
                        else if (_transform.position.z > b.transform.position.z)
                        {
                            dir = new Vector3(0, 0, 1);
                            sign = 1;
                        }
                        else if (_transform.position.z <= b.transform.position.z)
                        {
                            dir = new Vector3(0, 0, 1);
                            sign = -1;
                        }
                    }

                    bool result = pushBlockToGoodPlace(b, dir, sign);

                    if (result)
                    {
                        colliders = Physics.OverlapBox(_transform.position + Vector3.up*2, getBoxForBlockCheck(), _transform.rotation);
                        if (!isCertainBlockTypeInArray(colliders, BlockTypes.wall))
                        {
                            isBad = false;
                            break;
                        }
                        else
                        {
                            isBad = true;
                            break;
                        }
                    }
                    else
                    {
                        isBad = true;
                        break;
                    }
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

    private bool isCertainBlockTypeInArray(Collider[] colliders, BlockTypes _type)
    {
        if (colliders.Length == 0)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent(out Block b) && b.BlockType == _type)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void assessBlockStatusRoof()
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

                if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this))
                {
                    if (b.blockType == BlockTypes.roof)
                    {
                        Vector3 dir = Vector3.zero;
                        int sign = 0;

                        if (b.transform.eulerAngles.y == 0 || b.transform.eulerAngles.y == 180 || b.transform.eulerAngles.y == 360)
                        {
                            dir = new Vector3(1, 0, 0);

                            if (_transform.position.x > b.transform.position.x)
                            {
                                sign = 1;
                            }
                            else
                            {
                                sign = -1;
                            }
                        }
                        else
                        {
                            dir = new Vector3(0, 0, 1);

                            if (_transform.position.z > b.transform.position.z)
                            {
                                sign = 1;
                            }
                            else
                            {
                                sign = -1;
                            }
                        }

                        bool result = pushBlockToGoodPlace(b, dir, sign);

                        if (result)
                        {
                            colliders = Physics.OverlapBox(_transform.position + Vector3.up, getBoxForBlockCheck(), _transform.rotation);
                            if (colliders.Length == 0)
                            {
                                isBad = false;
                                break;
                            }
                            else
                            {
                                isBad = true;
                                break;
                            }
                        }
                        else
                        {
                            isBad = true;
                            break;
                        }

                    }
                    else if (b.blockType == BlockTypes.wall)
                    {
                        Vector3 shift = Vector3.zero;
                                                                        
                        _transform.position = new Vector3(_transform.position.x, b.transform.position.y + 4, _transform.position.z);
                        colliders = Physics.OverlapBox(_transform.position + Vector3.up, getBoxForBlockCheck(), _transform.rotation);
                        if (colliders.Length == 0)
                        {
                            isBad = false;
                            break;
                        }
                        else
                        {
                            isBad = true;
                            break;
                        }
                    }

                    
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
                _transform.position = new Vector3(getNearest15(markerPoint.x), getNearest2(markerPoint.y)/*getNearest15(markerPoint.y)*/, getNearest15(markerPoint.z));
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
        return sizeCheker / 2f;
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
    roof,
    beam,
    stair,
    fence
}

public enum BlockSizes
{    
    small,
    medium,
    large
}

public enum MaterialTypes
{
    none,
    wood,
    stone,
    concrete
}



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
    public bool IsFinalized { get; private set; }
    public DeltaDimentions DeltaStep { get => deltaStep; }
    public float RotationAngle { get => rotationAngle; }
    public Vector3 Volume { get => volume; }

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
    [SerializeField] private Vector3 volume = Vector3.one;

    [SerializeField] private MeshRenderer[] visualRenderers;
    [SerializeField] private Material visualMaterial;
    private Dictionary<MeshRenderer, Material> rendererMaterials = new Dictionary<MeshRenderer, Material>();

    private Sequence sequence;
    private Transform _transform;
    private bool isVisualOn;
    private GameManager gm;
    private BlockManager blockManager;
    private Identificator id;    
    private Anchors anchors;
    private LayerMask anchorMask;

    private Vector3 baseScale;
    private Vector3 lastMarkerPosition;


    private void OnEnable()
    {
        float strengthOfShake = 0.3f;
        if (Globals.IsMobile) strengthOfShake = 0.6f;

        if (volume.x <=2 && volume.y <= 2 && volume.z <= 2)
        {
            strengthOfShake = 1f;
            if (Globals.IsMobile) strengthOfShake = 1.5f;
        }

        baseScale = transform.localScale;
        if (sequence != null) sequence.Kill();
        sequence = DOTween.Sequence();
        sequence.SetLoops(-1, LoopType.Restart);
        sequence.Append(_transform.DOShakeScale(0.7f, strengthOfShake, 10).SetEase(Ease.OutSine));
        sequence.AppendInterval(0.5f);
        sequence.Pause();

        buildVFX.SetActive(false);
        _transform = transform;

        if (GameManager.Instance == null)
        {
            this.enabled = false;
            return;
        }
        gm = GameManager.Instance;
        blockManager = gm.BlockManager;
        id = GetComponent<Identificator>();
        SetColliders(true);
        anchors = GetComponent<Anchors>();
        anchorMask = LayerMask.GetMask(new string[] { "anchor" });

        if (visualRenderers.Length > 0)
        {
            for (int i = 0; i < visualRenderers.Length; i++)
            {
                if (!rendererMaterials.ContainsKey(visualRenderers[i])) rendererMaterials.Add(visualRenderers[i], visualRenderers[i].material);                
            }
        }
        else
        {
            print("renderers empty in " + gameObject.name);
        }

        if (visualMaterial == null) visualMaterial = gm.Assets.VisualizationMaterial;
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

    public void SetShakeEffect(bool isActive)
    {
        if (isActive && !isVisualOn)
        {
            isVisualOn = true;
            sequence.Play();
        }
        else if (!isActive && isVisualOn)
        {
            isVisualOn = false;
            sequence.Pause();
            _transform.localScale = baseScale;
        }
    }

    public void SetVisualization(bool isActive)
    {
        if (isActive)
        {
            
            //realView.SetActive(true);
            for (int i = 0; i < visualRenderers.Length; i++)
            {
                visualRenderers[i].sharedMaterial = visualMaterial;
            }

            
        }
        else
        {
            

            for (int i = 0; i < visualRenderers.Length; i++)
            {                
                if (rendererMaterials[visualRenderers[i]].name.Contains("main"))
                {
                    visualRenderers[i].sharedMaterial = gm.Assets.MainAtlas;
                }
                else if (rendererMaterials[visualRenderers[i]].name.Contains("black"))
                {
                    visualRenderers[i].sharedMaterial = gm.Assets.Black;
                }
                else if (rendererMaterials[visualRenderers[i]].name.Contains("brown"))
                {
                    visualRenderers[i].sharedMaterial = gm.Assets.Brown;
                }
                else if (rendererMaterials[visualRenderers[i]].name.Contains("carpet"))
                {
                    visualRenderers[i].sharedMaterial = gm.Assets.Carpet;
                }
                else
                {
                    visualRenderers[i].sharedMaterial = rendererMaterials[visualRenderers[i]];
                }

                //
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

    public void MakeNoColor()
    {
        hideAll();
        SetColliders(false);
        prototypeViewGood.SetActive(false);
        prototypeViewBad.SetActive(false);
    }

    public void MakeColorBadForDelete(bool isActive)
    {        
        prototypeViewBad.SetActive(isActive);
    }

    public void MakeFinalView()
    {        
        hideAll();
        realView.SetActive(true);
        IsFinalized = true;
        SetVisualization(false);
        SetColliders(true);
        StartCoroutine(playVFX());
        sequence.Kill();
        _transform.localScale = baseScale;
    }
    private IEnumerator playVFX()
    {
        buildVFX.SetActive(false);
        buildVFX.SetActive(true);
        yield return new WaitForSeconds(1);
        buildVFX.SetActive(false);
    }

    public void ShowDestroyVFX()
    {
        for (int i = 0; i < visualRenderers.Length; i++)
        {
            visualRenderers[i].sharedMaterial = gm.Assets.Destro;
        }
        StartCoroutine(playVFX());
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

        if (gm.IsWalkthroughGame)
        {

        }
        else
        {
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

                case BlockTypes.garden_ground:
                    assessBlockStatusGardenGround();
                    break;

                case BlockTypes.furniture:
                    assessBlockStatusFurniture();
                    break;

                default:
                    assessBlockStatusStandartSurface();
                    break;
            }

            lastMarkerPosition = markerPoint;
        }

        
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

    private void assessBlockStatusGardenGround()
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
                else if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this))
                {                    

                    if (b.blockType == BlockTypes.garden_ground)
                    {
                        Vector3 dir = (_transform.position - b.transform.position);                        
                        int sign = 0;

                        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z) && Mathf.Abs(dir.x) > 0)
                        {
                            if (dir.x > 0)
                            {
                                sign = 1;
                                dir = new Vector3(1, 0, 0);
                            }
                            else
                            {
                                sign = -1;
                                dir = new Vector3(1, 0, 0);

                            }
                        }
                        else
                        {
                            if (dir.z > 0)
                            {
                                sign = 1;
                                dir = new Vector3(0, 0, 1);
                            }
                            else
                            {
                                sign = -1;
                                dir = new Vector3(0, 0, 1);

                            }
                        }
                                                
                        bool result = pushBlockToGoodPlace(b, dir, sign);

                        if (result)
                        {

                            colliders = Physics.OverlapBox(_transform.position, getBoxForBlockCheck(), _transform.rotation);
                            if (!isCertainBlockTypeInArray(colliders, BlockTypes.garden_ground)
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

    private void assessBlockStatusFence()
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
                else if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this) && BlockType == b.blockType)
                {
                    Vector3 dir = (_transform.position - b.transform.position);
                    int sign = 0;

                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z) && Mathf.Abs(dir.x) > 0)
                    {
                        if (dir.x > 0)
                        {
                            sign = 1;
                            dir = new Vector3(1, 0, 0);
                        }
                        else
                        {
                            sign = -1;
                            dir = new Vector3(1, 0, 0);

                        }
                    }
                    else
                    {
                        if (dir.z > 0)
                        {
                            sign = 1;
                            dir = new Vector3(0, 0, 1);
                        }
                        else
                        {
                            sign = -1;
                            dir = new Vector3(0, 0, 1);

                        }
                    }

                    bool result = pushBlockToGoodPlace(b, dir, sign);

                    if (result)
                    {

                        colliders = Physics.OverlapBox(_transform.position, getBoxForBlockCheck(), _transform.rotation);
                        if (!isCertainBlockTypeInArray(colliders, BlockTypes.fence))
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
            //isBad = true;
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

    private void assessBlockStatusStandartSurface()
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
                else if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this) && BlockType == b.blockType && this.ID.ID == b.ID.ID)
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

    private void assessBlockStatusFurniture()
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
                else if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this))
                {
                    if (b.BlockType == BlockTypes.furniture && b.ID.ID == ID.ID)
                    {
                        isBad = true;
                        break;
                    }
                    else if (b.BlockType == BlockTypes.floor || b.BlockType == BlockTypes.furniture)
                    {
                        isBad = false;
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
        Collider[] colliders = Physics.OverlapBox(_transform.position + deltaCheker - _transform.right, getBoxForBlockCheck(), _transform.rotation);

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
                else if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this) )
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
                        if ((_transform.position.y - b.transform.position.y) >=2)
                        {
                            _transform.position = new Vector3(_transform.position.x, b.transform.position.y + b.volume.y, _transform.position.z);
                        }
                        else
                        {
                            _transform.position = new Vector3(_transform.position.x, b.transform.position.y, _transform.position.z);
                        }


                        
                        Vector3 dir = Vector3.zero;
                        int sign = 0;
                        int Yangle = (int)Mathf.Abs(b.transform.eulerAngles.y);

                        if (Yangle == 0 || Yangle == 180 || Yangle == 360)
                        {
                            dir = new Vector3(0, 0, 1);
                            sign = (gm.GetMainPlayerTransform().position.z - b.transform.position.z) > 0 ? 1 : -1;                            
                        }
                        else
                        {
                            dir = new Vector3(1, 0, 0);
                            sign = (gm.GetMainPlayerTransform().position.x - b.transform.position.x) > 0 ? 1 : -1;
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

                        _transform.position = new Vector3(_transform.position.x, b.transform.position.y, _transform.position.z);
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
                    else if (b.blockType == BlockTypes.stair)
                    {
                        Vector3 dir = Vector3.zero;
                        int sign = 0;
                        float xVolume = b.Volume.x;
                        int Yangle = (int)Mathf.Abs(b.transform.eulerAngles.y);

                        

                        if (Yangle == 0 || Yangle == 180 || Yangle == 360)
                        {
                            if (_transform.position.x > b.transform.position.x + xVolume / 2f)
                            {
                                dir = new Vector3(1, 0, 0);
                                sign = 1;
                                _transform.position = new Vector3(_transform.position.x, b.transform.position.y + b.Volume.y, _transform.position.z);
                            }
                            else
                            {
                                dir = new Vector3(1, 0, 0);
                                sign = -1;
                                _transform.position = new Vector3(_transform.position.x, b.transform.position.y + b.Volume.y, _transform.position.z);
                            }
                        }
                        else
                        {
                            if (_transform.position.z > b.transform.position.z + xVolume / 2f)
                            {
                                dir = new Vector3(0, 0, 1);
                                sign = 1;
                                _transform.position = new Vector3(_transform.position.x, b.transform.position.y + b.Volume.y, _transform.position.z);
                            }
                            else
                            {
                                dir = new Vector3(0, 0, 1);
                                sign = -1;
                                _transform.position = new Vector3(_transform.position.x, b.transform.position.y + b.Volume.y, _transform.position.z);
                            }
                        }

                        bool result = pushBlockToGoodPlace(b, dir, sign);

                        if (result)
                        {

                            colliders = Physics.OverlapBox(_transform.position, getBoxForBlockCheck(), _transform.rotation);
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
                    else if (b.blockType == BlockTypes.beam)
                    {

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

                if (colliders[i].gameObject.layer == 7 && colliders[i].TryGetComponent(out Block b) && !b.Equals(this))
                {                    
                    if (b.blockType == BlockTypes.wall)
                    {
                        if ((_transform.position.y - b.transform.position.y) >= 2.5f) 
                        {
                            _transform.position = b.transform.position + Vector3.up * b.volume.y;
                            colliders = Physics.OverlapBox(_transform.position + Vector3.up * 2, getBoxForBlockCheck() * 0.9f, _transform.rotation);

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

                        int Yangle = (int)Mathf.Abs(b.transform.eulerAngles.y);

                        if (Yangle == 0 || Yangle == 180 || Yangle == 360)
                        {
                            if (_transform.position.x > b.transform.position.x)
                            {
                                _transform.position = b.transform.position + new Vector3(b.volume.x / 2 + volume.x / 2, 0, 0);
                            }
                            else if (_transform.position.x <= b.transform.position.x)
                            {
                                _transform.position = b.transform.position + new Vector3(-b.volume.x / 2 - volume.x / 2, 0, 0);
                            }
                        }
                        else
                        {
                            if (_transform.position.z > b.transform.position.z)
                            {
                                _transform.position = b.transform.position + new Vector3(0, 0, b.volume.x / 2 + volume.x / 2);
                            }
                            else if (_transform.position.z <= b.transform.position.z)
                            {
                                _transform.position = b.transform.position + new Vector3(0, 0, -b.volume.x / 2 - volume.x / 2);
                            }
                        }

                        colliders = Physics.OverlapBox(_transform.position + Vector3.up * 2, getBoxForBlockCheck() * 0.9f, _transform.rotation);

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


                        /*
                        if ((_transform.position.y - b.transform.position.y) >= 3f
                            )
                        {
                            print("11111111");


                            Vector3 prev = _transform.position;
                            _transform.position = new Vector3(_transform.position.x, b.transform.position.y + b.volume.y, _transform.position.z); //b.transform.position + Vector3.up * b.volume.y;
                         
                        }
                        else
                        {
                            _transform.position = new Vector3(_transform.position.x, b.transform.position.y, _transform.position.z);                            
                        }

                        print("222222 " + b.gameObject.name);

                        //_transform.position = new Vector3(_transform.position.x, b.transform.position.y, _transform.position.z);

                        Vector3 dir = Vector3.zero;
                        int sign = 0;
                        int Yangle = (int)Mathf.Abs(b.transform.eulerAngles.y);

                        if (Yangle == 0 || Yangle == 180 || Yangle == 360)
                        {
                            if (_transform.position.x > b.transform.position.x)
                            {
                                _transform.position += new Vector3(b.volume.x / 2 + volume.x/2, 0, 0);
                                dir = new Vector3(1, 0, 0);
                                sign = 1;
                            }
                            else if (_transform.position.x <= b.transform.position.x)
                            {
                                _transform.position += new Vector3(-b.volume.x / 2 - volume.x / 2, 0, 0);
                                dir = new Vector3(1, 0, 0);
                                sign = -1;
                            }
                        }
                        else
                        {
                            if (_transform.position.z > b.transform.position.z)
                            {
                                _transform.position += new Vector3(0, 0, b.volume.x / 2 + volume.x / 2);
                                dir = new Vector3(0, 0, 1);
                                sign = 1;
                            }
                            else if (_transform.position.z <= b.transform.position.z)
                            {
                                _transform.position += new Vector3(0, 0, -b.volume.x / 2 - volume.x / 2);
                                dir = new Vector3(0, 0, 1);
                                sign = -1;
                            }
                        }

                        
                        //bool result = pushBlockToGoodPlace(b, dir, sign);

                        if (isSameBlocks(this))
                        {
                            isBad = true;
                            break;
                        }
                        else
                        {
                            isBad = false;
                            break;
                        }
                        */

                    }
                    else if (b.blockType == BlockTypes.floor)
                    {                        
                        _transform.position = new Vector3(_transform.position.x, b.transform.position.y + b.volume.y, _transform.position.z);

                        if (b.volume.x >= 3 && b.volume.z >= 3)
                        {                            
                            if ((b.transform.position - _transform.position).magnitude <= 1.5f)
                            {
                                _transform.position = b.transform.position + Vector3.up * b.volume.y;
                            }
                            else
                            {
                                int Yangle = (int)Mathf.Abs(b.transform.eulerAngles.y);

                                if (Yangle == 0 || Yangle == 180 || Yangle == 360)
                                {
                                    Vector3 another = b.transform.position + Vector3.up * b.volume.y;

                                    if (   (_transform.position - new Vector3(another.x + b.volume.x / 2f, another.y, another.z)).magnitude <= 1.5f)
                                    {
                                        _transform.position = new Vector3(another.x + b.volume.x / 2f, another.y, another.z);
                                    }
                                    else if ((_transform.position - new Vector3(another.x - b.volume.x / 2f, another.y, another.z)).magnitude <= 1.5f)
                                    {
                                        _transform.position = new Vector3(another.x - b.volume.x / 2f, another.y, another.z);
                                    }
                                    else if ((_transform.position - new Vector3(another.x, another.y, another.z + b.volume.z / 2f)).magnitude <= 1.5f)
                                    {
                                        _transform.position = new Vector3(another.x, another.y, another.z + b.volume.z / 2f);
                                    }
                                    else if ((_transform.position - new Vector3(another.x, another.y, another.z - b.volume.z / 2f)).magnitude <= 1.5f)
                                    {
                                        _transform.position = new Vector3(another.x, another.y, another.z - b.volume.z / 2f);
                                    }
                                }
                                else
                                {
                                    Vector3 another = b.transform.position + Vector3.up * b.volume.y;

                                    if ((_transform.position - new Vector3(another.x, another.y, another.z + b.volume.x / 2f)).magnitude <= 1.5f)
                                    {
                                        _transform.position = new Vector3(another.x, another.y, another.z + b.volume.x / 2f);
                                    }
                                    else if ((_transform.position - new Vector3(another.x, another.y, another.z - b.volume.x / 2f)).magnitude <= 1.5f)
                                    {
                                        _transform.position = new Vector3(another.x, another.y, another.z - b.volume.x / 2f);
                                    }
                                    else if ((_transform.position - new Vector3(another.x + b.volume.z / 2f, another.y, another.z)).magnitude <= 1.5f)
                                    {
                                        _transform.position = new Vector3(another.x + b.volume.z / 2f, another.y, another.z);
                                    }
                                    else if ((_transform.position - new Vector3(another.x - b.volume.z / 2f, another.y, another.z)).magnitude <= 1.5f)
                                    {
                                        _transform.position = new Vector3(another.x - b.volume.z / 2f, another.y, another.z);
                                    }
                                }
                            }
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

    private bool isSameBlocks(Block b)
    {
        for (int i = 0; i < blockManager.ReadyBlocks.Count; i++)
        {
            if (blockManager.ReadyBlocks[i].blockType == b.blockType && blockManager.ReadyBlocks[i].transform.position == b.transform.position)
            {
                return true;
            }
        }

        return false;
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
                        int Yangle = (int)Mathf.Abs(b.transform.eulerAngles.y);

                        if (Yangle == 0 || Yangle == 180 || Yangle == 360)
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
    fence,
    parts,
    others,
    garden_ground,
    furniture
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
    concrete,
    garden
}



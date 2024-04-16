using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public Block CurrentActiveBlock { get; private set; }
    public Block CurrentBlockToDelete { get; set; }
    public bool IsBuildingBlocks { get; private set; }
    public bool IsDestroingBlocks { get; private set; }
    public bool IsChoosingBlocks { get; private set; }

    public Action<int> OnChangeCurrentBlock;

    [SerializeField] private LineRenderer liner;

    private GameManager gm;
    private LevelControl lc;
    private SoundUI sounds;
    private AssetManager assets;
    private Transform playerTransform;
    
    public List<Block> ReadyBlocks { get; private set; }
    private Transform marker;
    //private Transform markerDestroyer;

    private float _timer;
    private int currentID;
    private Vector3 currentRotation = Vector3.zero;

    

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        lc = gm.LevelControl;
        sounds = SoundUI.Instance;
        assets = gm.Assets;
        playerTransform = gm.GetMainPlayerTransform();
        marker = assets.GetMarker;
        //markerDestroyer = assets.GetMarkerDestroyer;

        ReadyBlocks = new List<Block>();

        //actions        
        OnChangeCurrentBlock = changeCurrentBlockCall;

        //=
        if (gm.IsWalkthroughGame)
        {
            currentID = -1;
        }
        else
        {
            currentID = 1;
        }
        
        

        if (Globals.IsMobile)
        {
            liner.startWidth = 1;
            liner.endWidth = 1;
            liner.textureScale = new Vector2 (0.3f, 1);
        }
        else
        {
            liner.startWidth = 0.5f;
            liner.endWidth = 0.5f;
            liner.textureScale = new Vector2(0.5f, 1);
        }
    }
    
    

    // Update is called once per frame
    void Update()
    {        
        if (_timer > 0.1f)
        {
            _timer = 0;

            
            if (IsBuildingBlocks)
            {
                if (CurrentActiveBlock != null)
                {
                    if (gm.IsWalkthroughGame)
                    {
                        updateCurrentBlockPositionForVis();
                    }
                    else
                    {
                        updateCurrentBlockPosition();
                    }
                    
                    marker.position = gm.pointForMarker;
                }
            }
            else if (IsDestroingBlocks)
            {                
                if (CurrentBlockToDelete == null)
                {
                    marker.position = gm.pointForMarker;
                }
                else
                {
                    marker.position = CurrentBlockToDelete.gameObject.transform.position;
                }
            }
                        
            
        }
        else
        {
            _timer += Time.deltaTime;
        }        

        if (IsBuildingBlocks || IsDestroingBlocks)
        {
            liner.SetPosition(0, playerTransform.position + Vector3.up);
            liner.SetPosition(1, marker.position);

            /*
            if (IsBuildingBlocks)
            {
                liner.SetPosition(1, marker.position);
            }
            else if (IsDestroingBlocks)
            {
                liner.SetPosition(1, markerDestroyer.position);
            }
            */
        }
    }

    public void Rotate()
    {
        if (CurrentActiveBlock != null && CurrentActiveBlock.IsRotatable)
        {
            Vector3 newRotation = currentRotation;
            bool result = CurrentActiveBlock.Rotate(ref newRotation);
            if (result)
            {
                currentRotation = newRotation;
                sounds.PlayUISound(SoundsUI.click);
            }
            else
            {
                sounds.PlayUISound(SoundsUI.error1);
            }
        }
        else
        {
            sounds.PlayUISound(SoundsUI.error1);
        }
    }

    public void StopBuilding()
    {
        IsBuildingBlocks = false;
        IsDestroingBlocks = false;
        IsChoosingBlocks = false;

        if (CurrentBlockToDelete != null)
        {
            CurrentBlockToDelete.MakeColorBadForDelete(false);
            CurrentBlockToDelete = null;
        }

        if (CurrentActiveBlock != null)
        {
            CurrentActiveBlock.gameObject.SetActive(false);
            Destroy(CurrentActiveBlock.gameObject);
            CurrentActiveBlock = null;
        }

        //if (markerDestroyer.gameObject.activeSelf) markerDestroyer.gameObject.SetActive(false);
        if (marker.gameObject.activeSelf) marker.gameObject.SetActive(false);
        if (liner.gameObject.activeSelf) liner.gameObject.SetActive(false);
    }

    public void StartBuilding()
    {
        if (IsBuildingBlocks) return;

        if (CurrentBlockToDelete != null)
        {
            CurrentBlockToDelete.MakeColorBadForDelete(false);
            CurrentBlockToDelete = null;
        }

        IsBuildingBlocks = true;
        IsDestroingBlocks = false;
        IsChoosingBlocks = false;

        if (!marker.gameObject.activeSelf) marker.gameObject.SetActive(true);
        //if (markerDestroyer.gameObject.activeSelf) markerDestroyer.gameObject.SetActive(false);

        getNewBlock(currentID);
        if (!liner.gameObject.activeSelf) liner.gameObject.SetActive(true);
        liner.startColor = Color.yellow;
        liner.endColor = Color.yellow;
    }

    public void StartChoosing()
    {
        if (IsChoosingBlocks) return;

        if (CurrentBlockToDelete != null)
        {
            CurrentBlockToDelete.MakeColorBadForDelete(false);            
        }

        if (CurrentActiveBlock != null)
        {
            CurrentActiveBlock.gameObject.SetActive(false);
            Destroy(CurrentActiveBlock.gameObject);
            CurrentActiveBlock = null;
        }

        //if (markerDestroyer.gameObject.activeSelf) markerDestroyer.gameObject.SetActive(false);
        //marker.gameObject.SetActive(false);

        IsBuildingBlocks = false;
        IsDestroingBlocks = false;
        IsChoosingBlocks = true;
    }

    public void StartDestroying()
    {
        if (IsDestroingBlocks) return;

        if (CurrentActiveBlock != null)
        {
            CurrentActiveBlock.gameObject.SetActive(false);
            Destroy(CurrentActiveBlock.gameObject);
            CurrentActiveBlock = null;
        }

        IsBuildingBlocks = false;
        IsDestroingBlocks = true;
        IsChoosingBlocks = false;

        if (!marker.gameObject.activeSelf) marker.gameObject.SetActive(true);
        //if (!liner.gameObject.activeSelf) liner.gameObject.SetActive(true);
        liner.startColor = Color.red;
        liner.endColor = Color.red;
    }

    public void CancelLastBlock()
    {
        if (ReadyBlocks.Count == 0)
        {
            sounds.PlayUISound(SoundsUI.error1);
            return;
        }
                
        Block b = ReadyBlocks[ReadyBlocks.Count - 1];
        sounds.PlayDestroy(b);
        ReadyBlocks.Remove(b);
        b.gameObject.SetActive(false);
        Destroy(b);
    }

    public void BuildCurrentBlockCall()
    {
        if (gm.IsWalkthroughGame)
        {
            if (CurrentActiveBlock != null && gm.blockForMarker != null && CurrentActiveBlock.ID.ID == gm.blockForMarker.ID.ID)
            {
                gm.blockForMarker.MakeFinalView();
                Vector3 pos = CurrentActiveBlock.transform.position;
                sounds.PlayBuild(CurrentActiveBlock);
                Destroy(CurrentActiveBlock.gameObject);
                

                if (!lc.IsIDForBlockOK(currentID))
                {
                    int newBlockID = lc.GetFirstID();
                    if (currentID != newBlockID) currentID = newBlockID;
                }
                
                getNewBlock(currentID, pos);
                lc.UpdateProgress();
            }
            else
            {
                sounds.PlayUISound(SoundsUI.error1);
            }
        }
        else
        {
            if (CurrentActiveBlock != null && CurrentActiveBlock.IsGoodToFinalize)
            {
                sounds.PlayBuild(CurrentActiveBlock);
                CurrentActiveBlock.MakeFinalView();
                ReadyBlocks.Add(CurrentActiveBlock);
                getNewBlock(currentID);
            }
            else
            {
                sounds.PlayUISound(SoundsUI.error1);
            }
        }

        
    }

    public void DeleteCurrentBlock()
    {
        if (CurrentBlockToDelete != null)
        {
            if (ReadyBlocks.Count > 0)
            {
                for (int i = 0; i < ReadyBlocks.Count; i++)
                {
                    if (ReadyBlocks[i].Equals(CurrentBlockToDelete))
                    {
                        ReadyBlocks.Remove(CurrentBlockToDelete);
                    }
                }
            }

            sounds.PlayDestroy(CurrentBlockToDelete);
            CurrentBlockToDelete.gameObject.SetActive(false);
            Destroy(CurrentBlockToDelete.gameObject);
            CurrentBlockToDelete = null;
        }
        
    }

    private void changeCurrentBlockCall(int val)
    {        
        if (val == currentID)
        {
            gm.GetUI.HideBlocksPanel();
            return;
        }

        gm.GetUI.HideBlocksPanel();
        Destroy(CurrentActiveBlock.gameObject);
        currentID = val;
        getNewBlock(currentID);
    }

    private void updateCurrentBlockPositionForVis()
    {
        if (CurrentActiveBlock != null)
        {
            if (gm.blockForMarker != null && CurrentActiveBlock.ID.ID == gm.blockForMarker.ID.ID)
            {
                CurrentActiveBlock.transform.position = gm.blockForMarker.transform.position;
                CurrentActiveBlock.transform.eulerAngles = gm.blockForMarker.transform.eulerAngles;
                CurrentActiveBlock.MakeColorGood();
            }
            else
            {
                CurrentActiveBlock.transform.position = gm.pointForMarker;
                CurrentActiveBlock.transform.eulerAngles = Vector3.zero;
                CurrentActiveBlock.MakeColorBad();
            }
            
        }
    }

    private void updateCurrentBlockPosition()
    {
        if (CurrentActiveBlock != null)
        {
            CurrentActiveBlock.SetPosition(gm.pointForMarker);
        }
    }

    private void getNewBlock(int id)
    {
        getNewBlock(id, Vector3.zero);
    }

    private void getNewBlock(int id, Vector3 position)
    {        
        if (id == -1 && gm.IsWalkthroughGame)
        {
            id = lc.GetFirstID();
        }

        if (gm.IsWinWalkthroughGame) return;

        GameObject newBlock = assets.GetGameObjectByID(id);
        CurrentActiveBlock = newBlock.GetComponent<Block>();
        CurrentActiveBlock.gameObject.SetActive(true);
        CurrentActiveBlock.transform.position = position;

        if (CurrentActiveBlock.IsRotatable)
        {
            CurrentActiveBlock.transform.eulerAngles = currentRotation;
        }
        else
        {
            CurrentActiveBlock.transform.eulerAngles = Vector3.zero;
            currentRotation = Vector3.zero;
        }
        
        
        CurrentActiveBlock.transform.localScale = Vector3.one;

        CurrentActiveBlock.MakeColorBad();
        if (!gm.IsWalkthroughGame) updateCurrentBlockPosition();
        gm.GetUI.NewBlockChosen();
    }

}

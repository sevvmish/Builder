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

    private GameManager gm;
    private SoundUI sounds;
    private AssetManager assets;
    private Transform playerTransform;
    
    private List<Block> blocksForCancel = new List<Block>();
    //private Transform marker;
    private Transform markerDestroyer;

    private float _timer;
    private int currentID;
    private Vector3 currentRotation = Vector3.zero;

    

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        sounds = SoundUI.Instance;
        assets = gm.Assets;
        playerTransform = gm.GetMainPlayerTransform();
        //marker = assets.GetMarker;
        markerDestroyer = assets.GetMarkerDestroyer;

        //actions        
        OnChangeCurrentBlock = changeCurrentBlockCall;

        //=
        currentID = 1;
        //StartBuilding();
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
                    updateCurrentBlockPosition();
                    //marker.position = gm.pointForMarker;
                }
            }
            else if (IsDestroingBlocks)
            {
                if (!markerDestroyer.gameObject.activeSelf) markerDestroyer.gameObject.SetActive(true);

                if (CurrentBlockToDelete == null)
                {                    
                    markerDestroyer.position = gm.pointForMarker;
                }
                else
                {
                    markerDestroyer.position = CurrentBlockToDelete.gameObject.transform.position;
                }
            }
            
        }
        else
        {
            _timer += Time.deltaTime;
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
                sounds.PlayUISound(SoundsUI.error);
            }
        }
        else
        {
            sounds.PlayUISound(SoundsUI.error);
        }
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

        //marker.gameObject.SetActive(true);
        //markerDestroer.gameObject.SetActive(false);
        if (markerDestroyer.gameObject.activeSelf) markerDestroyer.gameObject.SetActive(false);

        getNewBlock(currentID);
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

        if (markerDestroyer.gameObject.activeSelf) markerDestroyer.gameObject.SetActive(false);

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

        //marker.gameObject.SetActive(false);
        //markerDestroer.gameObject.SetActive(true);
    }

    public void CancelLastBlock()
    {
        if (blocksForCancel.Count == 0)
        {
            sounds.PlayUISound(SoundsUI.error);
            return;
        }

        sounds.PlayUISound(SoundsUI.pop);
        Block b = blocksForCancel[blocksForCancel.Count - 1];
        blocksForCancel.Remove(b);
        b.gameObject.SetActive(false);
        Destroy(b);
    }

    public void BuildCurrentBlockCall()
    {
        if (CurrentActiveBlock != null && CurrentActiveBlock.IsGoodToFinalize)
        {            
            CurrentActiveBlock.MakeFinalView();
            blocksForCancel.Add(CurrentActiveBlock);
            getNewBlock(currentID);
            sounds.PlayUISound(SoundsUI.click);
        }
        else
        {
            sounds.PlayUISound(SoundsUI.error);
        }
    }

    public void DeleteCurrentBlock()
    {
        if (CurrentBlockToDelete != null)
        {
            if (blocksForCancel.Count > 0)
            {
                for (int i = 0; i < blocksForCancel.Count; i++)
                {
                    if (blocksForCancel[i].Equals(CurrentBlockToDelete))
                    {
                        blocksForCancel.Remove(CurrentBlockToDelete);
                    }
                }
            }

            CurrentBlockToDelete.gameObject.SetActive(false);
            Destroy(CurrentBlockToDelete.gameObject);
            CurrentBlockToDelete = null;
        }
        
    }

    private void changeCurrentBlockCall(int val)
    {
        if (val == currentID)
        {
            sounds.PlayUISound(SoundsUI.error);
            return;
        }

        gm.GetUI.HideBlocksPanel();
        Destroy(CurrentActiveBlock.gameObject);
        currentID = val;
        getNewBlock(currentID);
    }

    private void updateCurrentBlockPosition()
    {
        if (CurrentActiveBlock != null)
        {
            switch (CurrentActiveBlock.BlockType)
            {
                case BlockTypes.floor:
                    CurrentActiveBlock.SetPosition(gm.pointForMarker);
                    break;

                case BlockTypes.wall:
                    CurrentActiveBlock.SetPosition(gm.pointForMarker);
                    break;
            }
        }
    }


    private void getNewBlock(int id)
    {        
        GameObject newBlock = assets.GetGameObjectByID(id);
        CurrentActiveBlock = newBlock.GetComponent<Block>();
        CurrentActiveBlock.gameObject.SetActive(true);
        CurrentActiveBlock.transform.position = Vector3.zero;

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
        //marker.gameObject.SetActive(true);
        updateCurrentBlockPosition();
        gm.GetUI.NewBlockChosen();
    }

    private Vector3 lowerPlayerPoint => playerTransform.position + playerTransform.forward + Vector3.up * 0.1f;
    private Vector3 mediumPlayerPoint => playerTransform.position + playerTransform.forward + Vector3.up * 0.5f;
    private Vector3 highPlayerPoint => playerTransform.position + playerTransform.forward + Vector3.up;
}

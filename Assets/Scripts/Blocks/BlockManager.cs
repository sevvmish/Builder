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
        
    public Action<int> OnChangeCurrentBlock;

    private GameManager gm;
    private AssetManager assets;
    private Transform playerTransform;
    
    private Transform marker;
    private Transform markerDestroer;

    private float _timer;
    private int currentID;
    private Vector3 currentRotation = Vector3.zero;

    

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        assets = gm.Assets;
        playerTransform = gm.GetMainPlayerTransform();
        marker = assets.GetMarker;
        markerDestroer = assets.GetMarkerDestroer;

        //actions        
        OnChangeCurrentBlock = changeCurrentBlockCall;

        //=
        currentID = 1;
        StartBuilding();
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
                    marker.position = gm.pointForMarker;
                }
            }
            else if (IsDestroingBlocks)
            {
                markerDestroer.position = gm.pointForMarker;
            }
            
        }
        else
        {
            _timer += Time.deltaTime;
        }        
    }

    public void RotationMade(Vector3 newVector)
    {
        currentRotation = newVector;
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

        marker.gameObject.SetActive(true);
        markerDestroer.gameObject.SetActive(false);

        getNewBlock(currentID);
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

        marker.gameObject.SetActive(false);
        markerDestroer.gameObject.SetActive(true);
    }

    public void BuildCurrentBlockCall()
    {
        if (CurrentActiveBlock != null && CurrentActiveBlock.IsGoodToFinalize)
        {            
            CurrentActiveBlock.MakeFinalView();
            getNewBlock(currentID);
        }
        else
        {
            SoundUI.Instance.PlayUISound(SoundsUI.error);
        }
    }

    public void DeleteCurrentBlock()
    {
        if (CurrentBlockToDelete != null)
        {
            CurrentBlockToDelete.gameObject.SetActive(false);
            Destroy(CurrentBlockToDelete.gameObject);
            CurrentBlockToDelete = null;
        }
        
    }

    private void changeCurrentBlockCall(int val)
    {
        if (val == currentID)
        {
            SoundUI.Instance.PlayUISound(SoundsUI.error);
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
        marker.gameObject.SetActive(true);
        updateCurrentBlockPosition();
        gm.GetUI.NewBlockChosen();
    }

    private Vector3 lowerPlayerPoint => playerTransform.position + playerTransform.forward + Vector3.up * 0.1f;
    private Vector3 mediumPlayerPoint => playerTransform.position + playerTransform.forward + Vector3.up * 0.5f;
    private Vector3 highPlayerPoint => playerTransform.position + playerTransform.forward + Vector3.up;
}

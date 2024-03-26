using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public Block CurrentActiveBlock { get; private set; }
    
    public Action OnBuildCurrentBlock;
    public Action<int> OnChangeCurrentBlock;

    private GameManager gm;
    private AssetManager assets;
    private Transform playerTransform;
    private Transform marker;

    private float _timer;
    private int currentID;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        assets = gm.Assets;
        playerTransform = gm.GetMainPlayerTransform();
        marker = assets.GetMarker;

        //actions
        OnBuildCurrentBlock = buildCurrentBlockCall;
        OnChangeCurrentBlock = changeCurrentBlockCall;

        //=
        currentID = 1;
        getNewBlock(currentID);
    }
    

    // Update is called once per frame
    void Update()
    {        
        if (_timer > 0.1f)
        {
            _timer = 0;

            if (CurrentActiveBlock != null)
            {
                updateCurrentBlockPosition();                                
                marker.position = gm.pointForMarker;
            }
        }
        else
        {
            _timer += Time.deltaTime;
        }        
    }

    private void buildCurrentBlockCall()
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
            }
        }
    }

    private void getNewBlock(int id)
    {
        /*
        if (CurrentActiveBlock != null)
        {
            print("ERROR! Block is allready in use!");
            return;
        }*/

        GameObject newBlock = assets.GetGameObjectByID(id);
        CurrentActiveBlock = newBlock.GetComponent<Block>();
        CurrentActiveBlock.gameObject.SetActive(true);
        CurrentActiveBlock.transform.position = Vector3.zero;
        CurrentActiveBlock.transform.eulerAngles = Vector3.zero;
        CurrentActiveBlock.transform.localScale = Vector3.one;

        CurrentActiveBlock.MakeColorBad();
        marker.gameObject.SetActive(true);
        updateCurrentBlockPosition();
    }

    private Vector3 lowerPlayerPoint => playerTransform.position + playerTransform.forward + Vector3.up * 0.1f;
    private Vector3 mediumPlayerPoint => playerTransform.position + playerTransform.forward + Vector3.up * 0.5f;
    private Vector3 highPlayerPoint => playerTransform.position + playerTransform.forward + Vector3.up;
}

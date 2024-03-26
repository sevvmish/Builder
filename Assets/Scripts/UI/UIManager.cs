using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main buttons")]
    [SerializeField] private Button buildCurrentBlockButton;

    [Header("BlocksMenu")]
    [SerializeField] private BlockMenuUI blockMenuUI;


    private SoundUI sounds;
    private GameManager gm;
    private AssetManager assets;
    [SerializeField] private BlockManager blockManager;

    

    // Start is called before the first frame update
    void Start()
    {
        sounds = SoundUI.Instance;
        gm = GameManager.Instance;
        assets = gm.Assets;

        buildCurrentBlockButton.onClick.AddListener(buildCurrentBlock);        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildCurrentBlock();
        }
    }

    private void buildCurrentBlock()
    {
        sounds.PlayUISound(SoundsUI.click);
        blockManager.OnBuildCurrentBlock?.Invoke();
    }

    public void HideBlocksPanel() => blockMenuUI.HideAllPanel();
}

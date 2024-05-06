using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroUI : MonoBehaviour
{
    [SerializeField] private GameObject backPanel;
    [SerializeField] private GameObject iconPanel;
    [SerializeField] private GameObject textPanel;

    [SerializeField] private RectTransform backRect;
    [SerializeField] private RectTransform iconRect;
    [SerializeField] private RectTransform textRect;
    private Vector2 backBasePosition;
    private Vector2 iconBasePosition;
    private Vector2 textBasePosition;
    private float speedTime = 0.3f;

    [SerializeField] private TextMeshProUGUI missionName;
    [SerializeField] private TextMeshProUGUI missionWin;
    [SerializeField] private Image missionIcon;

    private GameManager gm;
    private LevelControl lc;

    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.Instance.IsWalkthroughGame) return;
        gm = GameManager.Instance;
        lc = gm.LevelControl;

        backBasePosition = new Vector2(0, 0);
        iconBasePosition = new Vector2(-503, 9);
        textBasePosition = new Vector2(-300, 9);

        backPanel.SetActive(false);
        iconPanel.SetActive(false);
        textPanel.SetActive(false);

        missionIcon.sprite = lc.GetLevelData.MissionIcon;
        missionName.text = getMissionName(Globals.CurrentLevel);

        if (missionName.text.Length <= 20)
        {
            missionName.fontSize = 60;
        }
        else
        {
            missionName.fontSize = 50;
        }

        missionWin.text = Globals.Language.BuildingCompleted;
    }

    public void ShowIntro(bool isWin)
    {
        StartCoroutine(playIntro(isWin));
    }
    private IEnumerator playIntro(bool isWin)
    {
        backPanel.SetActive(true);
        iconPanel.SetActive(true);
        textPanel.SetActive(true);

        if (isWin)
        {
            missionWin.gameObject.SetActive(true);
        }
        else
        {
            missionWin.gameObject.SetActive(false);
        }

        //before
        backRect.anchoredPosition = new Vector2 (-900, 0);
        iconRect.anchoredPosition = new Vector2 (1400, 0);
        textRect.anchoredPosition = new Vector2 (-2000, 0);

        //start
        SoundUI.Instance.PlayUISound(SoundsUI.success1);

        backRect.DOAnchorPos(backBasePosition, speedTime).SetEase(Ease.InOutBounce);
        yield return new WaitForSeconds(0.1f);
        iconRect.DOAnchorPos(iconBasePosition, speedTime).SetEase(Ease.InOutBounce);
        yield return new WaitForSeconds(0.1f);
        textRect.DOAnchorPos(textBasePosition, speedTime).SetEase(Ease.InOutBounce);
        
        yield return new WaitForSeconds(2f);


        //back
        backRect.DOAnchorPos(new Vector2(-900, 0), speedTime).SetEase(Ease.InOutBounce);
        yield return new WaitForSeconds(0.1f);
        iconRect.DOAnchorPos(new Vector2(1400, 0), speedTime).SetEase(Ease.InOutBounce);
        yield return new WaitForSeconds(0.1f);
        textRect.DOAnchorPos(new Vector2(-2000, 0), speedTime).SetEase(Ease.InOutBounce);
        
        yield return new WaitForSeconds(speedTime);
        
        backPanel.SetActive(false);
        iconPanel.SetActive(false);
        textPanel.SetActive(false);
    }

    private string getMissionName(int level)
    {
        return Globals.Language.MissionName[level];
    }
}

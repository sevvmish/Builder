using DG.Tweening;
using UnityEngine;

public class IslandFlying : MonoBehaviour
{
    public float upDownDistance = 2f;
    public float upDownSpeed = 3f;
    public float rotationSpeedDirection = 5f;
    public bool upFirst;

    private Transform island;

    // Start is called before the first frame update
    void Start()
    {
        island = GetComponent<Transform>();

        Sequence sequence = DOTween.Sequence();
        sequence.SetLoops(-1, LoopType.Restart);
        if (upFirst)
        {
            sequence.Append(island.DOLocalMoveY(island.localPosition.y + upDownDistance, upDownSpeed).SetEase(Ease.Linear));
            sequence.Append(island.DOLocalMoveY(island.localPosition.y, upDownSpeed).SetEase(Ease.Linear));
        }
        else
        {
            sequence.Append(island.DOLocalMoveY(island.localPosition.y - upDownDistance, upDownSpeed).SetEase(Ease.Linear));
            sequence.Append(island.DOLocalMoveY(island.localPosition.y, upDownSpeed).SetEase(Ease.Linear));
        }
        
    }

    private void Update()
    {
        if (rotationSpeedDirection != 0) island.localEulerAngles = new Vector3(0, island.localEulerAngles.y + rotationSpeedDirection * Time.deltaTime, 0);        
    }

}

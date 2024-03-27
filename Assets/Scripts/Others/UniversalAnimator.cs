using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UniversalAnimator : MonoBehaviour
{

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private LevelAnimatorType GameType;
    [SerializeField] private float DelayBeforeStart;
    [SerializeField] private Ease easeType = Ease.InOutSine;

    [SerializeField] private Axes PendulumAxis;
    [SerializeField] private float PendulumAngle;
    [SerializeField] private float PendulumSpeed;

    [SerializeField] private Axes RotationAxis;
    [SerializeField] private float RotationSpeed;

    [SerializeField] private Transform FromPoint, ToPoint;
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float delayBeforePoint;
    [SerializeField] private float delayBetweenPoints;

    public bool isUsePlatforming = false;

    private float _timer;
    private Vector3 prevPos;

    private float rotator;

    private void Start()
    {
        Invoke("Init", DelayBeforeStart);        
        prevPos = transform.position;
    }

    private void Init()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetLoops(-1, LoopType.Restart);
        sequence.SetUpdate(UpdateType.Fixed);

        switch (GameType)
        {
            case LevelAnimatorType.pendulum:
                switch (PendulumAxis)
                {
                    case Axes.axis_X:
                        _rigidbody.rotation = Quaternion.Euler(
                    -PendulumAngle,
                    _rigidbody.rotation.eulerAngles.y,
                    _rigidbody.rotation.eulerAngles.z);


                        sequence.Append(_rigidbody.DORotate(new Vector3(PendulumAngle,
                            _rigidbody.rotation.eulerAngles.y,
                            _rigidbody.rotation.eulerAngles.z), PendulumSpeed).SetEase(easeType));

                        sequence.Append(_rigidbody.DORotate(new Vector3(-PendulumAngle,
                            _rigidbody.rotation.eulerAngles.y,
                            _rigidbody.rotation.eulerAngles.z), PendulumSpeed).SetEase(easeType));

                        break;

                    case Axes.axis_Y:
                        _rigidbody.rotation = Quaternion.Euler(
                    _rigidbody.rotation.eulerAngles.x,
                    -PendulumAngle,
                    _rigidbody.rotation.eulerAngles.z);


                        sequence.Append(_rigidbody.DORotate(new Vector3(
                            _rigidbody.rotation.eulerAngles.x,
                            PendulumAngle,
                            _rigidbody.rotation.eulerAngles.z), PendulumSpeed).SetEase(easeType));

                        sequence.Append(_rigidbody.DORotate(new Vector3(
                            _rigidbody.rotation.eulerAngles.x,
                            -PendulumAngle,
                            _rigidbody.rotation.eulerAngles.z), PendulumSpeed).SetEase(easeType));

                        break;

                    case Axes.axis_Z:
                        _rigidbody.rotation = Quaternion.Euler(
                    _rigidbody.rotation.eulerAngles.x,
                    _rigidbody.rotation.eulerAngles.y,
                    -PendulumAngle);


                        sequence.Append(_rigidbody.DORotate(new Vector3(
                            _rigidbody.rotation.eulerAngles.x,
                            _rigidbody.rotation.eulerAngles.y,
                            PendulumAngle), PendulumSpeed).SetEase(easeType));

                        sequence.Append(_rigidbody.DORotate(new Vector3(
                            _rigidbody.rotation.eulerAngles.x,
                            _rigidbody.rotation.eulerAngles.y,
                            -PendulumAngle), PendulumSpeed).SetEase(easeType));

                        break;
                }



                break;

            case LevelAnimatorType.rotation:
                switch (RotationAxis)
                {
                    case Axes.axis_X:
                        rotator = _rigidbody.rotation.eulerAngles.x;
                        

                        break;

                    case Axes.axis_Y:
                        rotator = _rigidbody.rotation.eulerAngles.y;

                        break;

                    case Axes.axis_Z:
                        rotator = _rigidbody.rotation.eulerAngles.z;

                        break;
                }
                break;

            case LevelAnimatorType.movement:
                               
                if (isUsePlatforming)
                {
                    _rigidbody.position = FromPoint.position;
                    sequence.Append(_rigidbody.DOMove(ToPoint.position, MovementSpeed).SetEase(easeType));
                    sequence.AppendInterval(delayBetweenPoints);
                    sequence.Append(_rigidbody.DOMove(FromPoint.position, MovementSpeed).SetEase(easeType));
                    sequence.AppendInterval(delayBeforePoint);
                }
                else
                {
                    _rigidbody.transform.position = FromPoint.position;
                    sequence.Append(_rigidbody.transform.DOMove(ToPoint.position, MovementSpeed).SetEase(easeType));
                    sequence.AppendInterval(delayBetweenPoints);
                    sequence.Append(_rigidbody.transform.DOMove(FromPoint.position, MovementSpeed).SetEase(easeType));
                    sequence.AppendInterval(delayBeforePoint);
                }        
                
                break;
        }
    }

    

    private void FixedUpdate()
    {


        
        if (GameType == LevelAnimatorType.rotation)
        {            
                                           
                switch (RotationAxis)
                {
                    case Axes.axis_X:
                        
                        _rigidbody.DORotate(new Vector3(
                            _rigidbody.rotation.eulerAngles.x + RotationSpeed,
                            _rigidbody.rotation.eulerAngles.y,
                            _rigidbody.rotation.eulerAngles.z), 1).SetEase(Ease.Linear);

                        break;

                    case Axes.axis_Y:


                        _rigidbody.MoveRotation(Quaternion.Euler(
                            _rigidbody.rotation.eulerAngles.x,
                            _rigidbody.rotation.eulerAngles.y + RotationSpeed,
                            _rigidbody.rotation.eulerAngles.z));

                        /*
                        _rigidbody.DORotate(new Vector3(
                            _rigidbody.rotation.eulerAngles.x,
                            _rigidbody.rotation.eulerAngles.y + RotationSpeed,
                            _rigidbody.rotation.eulerAngles.z), 1).SetEase(Ease.Linear);
                        */
                        break;

                    case Axes.axis_Z:
                        _rigidbody.DORotate(new Vector3(
                            _rigidbody.rotation.eulerAngles.x,
                            _rigidbody.rotation.eulerAngles.y,
                            _rigidbody.rotation.eulerAngles.z + RotationSpeed), 1).SetEase(Ease.Linear);

                        break;
                }

                
            
        }

        
    }

}

public enum LevelAnimatorType
{
    pendulum,
    rotation,
    movement
}


public enum Axes
{
    none,
    axis_X,
    axis_Y,
    axis_Z
}


public static class RigidbodyShortcutExtensions
{
    public static Tweener DOMove(this Rigidbody rigidbody, Vector3 endValue, float duration)
    {
        return DOTween.To(() => rigidbody.position, rigidbody.MovePosition, endValue, duration).SetId(rigidbody);
    }

    public static Tweener DORotate(this Rigidbody rigidbody, Vector3 endValue, float duration)
    {
        return DOTween.To(() => rigidbody.rotation, rigidbody.MoveRotation , endValue, duration).SetId(rigidbody);
    }
}

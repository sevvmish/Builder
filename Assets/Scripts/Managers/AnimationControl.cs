using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public AnimationStates AnimationState { get; private set; }

    private Animator _animator;
    private PlayerControl pc;
    private GameManager gm;
    private float speedLimit = 1f;
    private float minSpeed = 0.1f;
    private float _timer;
    private readonly float coolDown = 0.1f;

    public void SetData(Animator a, PlayerControl p)
    {
        gm = GameManager.Instance;
        pc = p;
        _animator = a;
        AnimationState = AnimationStates.None;
        idle();
    }

    public void Run()
    {
        checkMovement();
    }

    public void Idle()
    {
        checkMovement();
    }

    public void JumpStart()
    {
        _animator.Play("JumpStart");
        AnimationState = AnimationStates.Fly;
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }

        _timer = coolDown;

        checkMovement();
    }

    private void checkMovement()
    {
        if (pc.IsGrounded)
        {
            float speed = pc.PlayerVelocity;

            if (speed < speedLimit && speed > minSpeed)
            {
                walk();
            }
            else if (speed > speedLimit)
            {
                run();
            }
            else if (speed <= minSpeed)
            {
                idle();
            }
        }
        else
        {
            /*
            if (IsJumping)
            {
                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpStart") && !_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpLoop")) _animator.Play("JumpStart");
            }
            else if (howLongNonGrounded > 0.1f)
            {
                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpLoop")) _animator.Play("JumpLoop");
            }*/

            fly();
        }
        
    }

    private void fly()
    {
        if (AnimationState == AnimationStates.Fly) return;
            AnimationState = AnimationStates.Fly;

        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpLoop")
                && !_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpStart")) _animator.Play("JumpLoop");
    }

    private void idle()
    {
        if (AnimationState == AnimationStates.Idle) return;
            AnimationState = AnimationStates.Idle;

        if (pc.IsGrounded && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {            
            _animator.Play("Idle");
        }
        else if (!pc.IsGrounded)
        {
            fly();
        }
    }

    private void run()
    {
        if (AnimationState == AnimationStates.Run) return;
            AnimationState = AnimationStates.Run;

        if (pc.IsGrounded && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            _animator.Play("Run");
        }
        else if (!pc.IsGrounded)
        {
            fly();
        }
    }

    private void walk()
    {
        if (AnimationState == AnimationStates.Walk) return;
            AnimationState = AnimationStates.Walk;

        if (pc.IsGrounded && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            _animator.Play("Walk");
        }
        else if (!pc.IsGrounded)
        {
            fly();
        }
    }


}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    public SkinControl SkinControl { get; private set; }

    [Header("Controls")]    
    private AnimationControl animationControl;
    private Transform playerLocation;
    private EffectsControl effectsControl;

    //INPUT
    public float angleYForMobile { get; private set; }
    public void SetHorizontal(float hor) => horizontal = hor;
    public void SetVertical(float ver) => vertical = ver;
    public void SetRotationAngle(float ang) => angleY = ang;
    public void SetJump() => isJump = true;
    public void SetJumpUP() => isJumpUp = true;
    public void SetJumpDOWN() => isJumpDown = true;
    public Rigidbody GetRigidbody => _rigidbody;
    public void SetForward(bool isOk) => isForward = isOk;
    private float horizontal;
    private float vertical;
    private float angleY;
    
    private bool isJump;
    private bool isJumpUp;
    private bool isJumpDown;
    private bool isForward;
    private bool isChekingCollision;
    private bool isCollisionON;
    private bool isSolidPlayerBodyON;


    //SPEED
    public float PlayerMaxSpeed { get; private set; }
    public float PlayerCurrentSpeed { get; private set; }
    public float PlayerVelocity { get; private set; }
    public float PlayerNonVerticalVelocity { get; private set; }
    public float PlayerVerticalVelocity { get; private set; }

    //CONDITIONS
    public bool IsGrounded { get; private set; } = true;
    public bool IsCanJump { get; private set; }
    public bool IsCanWalk { get; private set; }
    public bool IsSpeedChanged { get; private set; }
    public bool IsFinished { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsIdle { get; private set; }
    
    public bool IsFloating { get; private set; }
    public bool IsCanAct { get; private set; }
    public bool IsDead { get; private set; }

    private Rigidbody _rigidbody;
    private CapsuleCollider mainCollider;
    private Transform _transform;
    private GameManager gm;

    private float jumpCooldown;
    private float howLongNonGrounded;
    private float howLongMoving;
    private float checkGroundTimer;

    private LayerMask ignoreMask;

    private WaitForSeconds ZeroOne = new WaitForSeconds(0.1f);
    private WaitForSeconds FixedOne = new WaitForSeconds(0.02f);

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        playerLocation = gm.GetPlayersLocation();

        ignoreMask = LayerMask.GetMask(new string[] { "trigger", "player"});

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.mass = PhysicsCustomizing.GetData(PhysicObjects.Player).Mass;
        _rigidbody.drag = PhysicsCustomizing.GetData(PhysicObjects.Player).Drag;
        _rigidbody.angularDrag = PhysicsCustomizing.GetData(PhysicObjects.Player).AngularDrag;

        _transform = GetComponent<Transform>();
        mainCollider = GetComponent<CapsuleCollider>();
        PlayerMaxSpeed = Globals.BASE_SPEED;
        PlayerCurrentSpeed = PlayerMaxSpeed;
        IsCanAct = true;
        IsCanJump = true;
        IsCanWalk = true;

        if (gm.IsWalkthroughGame)
        {
            StartCoroutine(playerPosition());
        }
    }
    private IEnumerator playerPosition()
    {
        yield return new WaitForSeconds(0.1f);
        _rigidbody.MovePosition(gm.LevelControl.GetLevelData.Position);
        _rigidbody.MoveRotation(Quaternion.Euler(gm.LevelControl.GetLevelData.Rotation));
    }

    public void CheckUnstuck()
    {
        StartCoroutine(checkingUnstuck());
    }
    private IEnumerator checkingUnstuck()
    {
        print("started checking");
        isChekingCollision = true;
        
        yield return FixedOne;

        if (isCollisionON)
        {
            for (int i = 0; i < 8; i++)
            {
                if (!isCollisionON) break;
                _rigidbody.MovePosition(_transform.position + Vector3.up);
                yield return FixedOne;
            }
        }

        isChekingCollision = false;
        isCollisionON = false;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!isChekingCollision) return;

        if (collision.gameObject.TryGetComponent(out Block b))
        {
            isCollisionON = true;
            print("yesss!!!!!!!!!");
        }
        else
        {
            isCollisionON = false;
            print("noooooo!!!!!!!!!");
        }
    }


    public void SetEffectControl(EffectsControl ef) => effectsControl = ef;

    public void SetSkinData(Animator anim)
    {        
        anim.gameObject.AddComponent<AnimationControl>();
        animationControl = anim.GetComponent<AnimationControl>();
        animationControl.SetData(anim, this);
        SkinControl = anim.GetComponent<SkinControl>();
    }

    
    public void StopJumpPermission(float seconds)
    {
        StartCoroutine(jumpPermission(seconds));
    }
    private IEnumerator jumpPermission(float seconds)
    {
        IsCanJump = false;

        for (float i = 0; i < seconds; i += 0.1f)
        {
            yield return ZeroOne;
            if (IsDead) break;
        }

        IsCanJump = true;
    }

    
    public void StopWalkPermission(float seconds)
    {
        StartCoroutine(walkPermission(seconds));
    }
    private IEnumerator walkPermission(float seconds)
    {
        IsCanWalk = false;

        for (float i = 0; i < seconds; i += 0.1f)
        {
            yield return ZeroOne;
            if (IsDead) break;
        }

        IsCanWalk = true;
    }

    public void StopActPermission(float seconds)
    {
        StartCoroutine(actPermission(seconds));
    }
    private IEnumerator actPermission(float seconds)
    {
        IsCanAct = false;

        for (float i = 0; i < seconds; i += 0.1f)
        {
            yield return ZeroOne;
            if (IsDead) break;
        }

        IsCanAct = true;
    }

    public void ChangeSpeed(float multiplier, float seconds)
    {        
        StartCoroutine(changeSpeed(multiplier, seconds));
    }
    private IEnumerator changeSpeed(float multiplier, float seconds)
    {
        PlayerCurrentSpeed *= multiplier;
        IsSpeedChanged = true;
                
        for (float i = 0; i < seconds; i+=0.1f)
        {            
            yield return ZeroOne;
            if (IsDead) break;
        }

        IsSpeedChanged = false;
        PlayerCurrentSpeed /= multiplier;
    }

    private void Update()
    {
        if (!gm.IsGameStarted || IsFinished)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }

        if (gm.IsWalkthroughGame && gm.IsBuildMode && !isSolidPlayerBodyON)
        {
            isSolidPlayerBodyON = true;
            GetComponent<CapsuleCollider>().isTrigger = true;
            GetComponent<SphereCollider>().isTrigger = true;
        }
        else if (gm.IsWalkthroughGame && !gm.IsBuildMode && isSolidPlayerBodyON)
        {
            isSolidPlayerBodyON = false;
            GetComponent<CapsuleCollider>().isTrigger = false;
            GetComponent<SphereCollider>().isTrigger = false;
            CheckUnstuck();
        }

        if (jumpCooldown > 0) jumpCooldown -= Time.deltaTime;

        if (isForward && IsCanWalk)
        {
            movement(true);
        }
        else
        {
            movement(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gm.IsGameStarted || IsFinished) return;

        if (checkGroundTimer >= Time.fixedDeltaTime * 2)
        {
            checkGroundTimer = 0;
            IsGrounded = checkGround();
        }
        else
        {
            checkGroundTimer += Time.fixedDeltaTime;
        }

        effectsControl.WalkSmoke(IsGrounded);

        PlayerVelocity = _rigidbody.velocity.magnitude;
        PlayerNonVerticalVelocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z).magnitude;
        PlayerVerticalVelocity = new Vector3(0, _rigidbody.velocity.y, 0).magnitude;

        if (!IsGrounded)
        {
            howLongNonGrounded += Time.deltaTime;

            GravityScale(_rigidbody);
        }
        else
        {
            howLongNonGrounded = 0;

            if (_rigidbody.drag != PhysicsCustomizing.GetData(PhysicObjects.Player).Drag) _rigidbody.drag = PhysicsCustomizing.GetData(PhysicObjects.Player).Drag;
        }

        if (isJump) makeJump();
        if (isJumpUp) makeJumpUp();
        if (isJumpDown) makeJumpDown();
    }

    private void makeJumpUp()
    {
        isJumpUp = false;
        if (!IsCanAct) return;

        if (gm.IsBuildMode)
        {
            if (jumpCooldown <= 0)
            {
                _rigidbody.velocity = Vector3.zero;
                effectsControl.MakeJumpFX();
                animationControl.JumpStart();

                //_transform.position += Vector3.up * 1.5f;
                _rigidbody.AddForce(Vector3.up * 20, ForceMode.Impulse);
                IsJumping = true;
                //jumpCooldown = 0.2f;
            }
            return;
        }
    }

    private void makeJumpDown()
    {
        isJumpDown = false;

        if (IsGrounded) return;

        if (!IsCanAct) return;

        if (gm.IsBuildMode)
        {
            if (jumpCooldown <= 0)
            {
                _rigidbody.velocity = Vector3.zero;
                effectsControl.MakeJumpFX();
                animationControl.JumpStart();

                //_transform.position += Vector3.down * 1.5f;
                _rigidbody.AddForce(Vector3.down * 20, ForceMode.Impulse);
                IsJumping = true;
                //jumpCooldown = 0.2f;
            }
            return;
        }
    }

    private void makeJump()
    {        
        isJump = false;
        if (!IsCanAct) return;
        
        if (gm.IsBuildMode)
        {
            if (jumpCooldown <= 0)
            {                
                _rigidbody.velocity = Vector3.zero;
                effectsControl.MakeJumpFX();
                animationControl.JumpStart();
                                
                _transform.position += Vector3.up * 2;
                IsJumping = true;
                jumpCooldown = 0.2f;
            }
            return;
        }

        if (IsGrounded && jumpCooldown <= 0 && !IsJumping && IsCanJump)
        {
            float addKoef = 1;
           
            _rigidbody.velocity = Vector3.zero;
            effectsControl.MakeJumpFX();
            animationControl.JumpStart();

            _rigidbody.AddRelativeForce(Vector3.up * Globals.JUMP_POWER * addKoef, ForceMode.Impulse);            
            IsJumping = true;
            jumpCooldown = 0.2f;
        }
    }

    private bool checkGround()
    {
        bool result = Physics.CheckBox(_transform.position + Vector3.down * 0f, new Vector3(0.25f, 0.05f, 0.25f), _transform.rotation, ~ignoreMask, QueryTriggerInteraction.Ignore);

        if (!IsGrounded && result)
        {
            effectsControl.MakeLandEffect();
        }

        if (!IsGrounded && result)
        {            
            IsRunning = false;
            IsIdle = true;
        }

        if (result)
        {   
            if (jumpCooldown <= 0) IsJumping = false;
            IsFloating = false;
        }

        return result;
    }


    private void movement(bool forward)
    {        
        if (!IsCanAct || !IsCanWalk)
        {
            return;
        }
            

        if (Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0 || forward || Mathf.Abs(angleY) > 0)
        {
            float turnKoeff = PlayerCurrentSpeed * 0.03f;
                        
            if ((Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0))
            {                    
                if (Mathf.Abs(angleY) > 0)
                {
                    angleYForMobile += angleY;                    
                }

                float angle = Mathf.Atan2(horizontal, vertical) * 180 / Mathf.PI;

                if (Globals.IsMobile)
                {
                    _rigidbody.DORotate(new Vector3(_transform.eulerAngles.x, angleYForMobile + angle, _transform.eulerAngles.z), Time.deltaTime * 10);
                }
                else
                {
                    _rigidbody.MoveRotation(Quaternion.Euler(new Vector3(_transform.eulerAngles.x, angleYForMobile + angle, _transform.eulerAngles.z)));
                }

            }
            else if (horizontal == 0 && vertical == 0 && Mathf.Abs(angleY) > 0)
            {
                angleYForMobile += angleY;
                                

                if (Globals.IsMobile)
                {
                    _rigidbody.DORotate(new Vector3(_transform.eulerAngles.x, angleYForMobile, _transform.eulerAngles.z), Time.deltaTime * 10);
                }
                else
                {
                    _rigidbody.MoveRotation(Quaternion.Euler(new Vector3(_transform.eulerAngles.x, angleYForMobile, _transform.eulerAngles.z)));
                }
                
            }

            angleY = 0;
                        
            if (forward)
            {
                forward = false;
                vertical = 1;
            }

            float addSpeedKoeff = 1f;

            if (Globals.IsMobile)
            {
                addSpeedKoeff += Globals.SPEED_INC_IN_NONGROUND_MOBILE;
            }
            else
            {
                addSpeedKoeff += Globals.SPEED_INC_IN_NONGROUND_PC;
            }


            if ((PlayerVelocity < PlayerCurrentSpeed && IsGrounded) || (PlayerVelocity < PlayerCurrentSpeed * addSpeedKoeff && !IsGrounded))
            {
                float koeff = 0;
                float addKoeff = IsGrounded ? 1 : addSpeedKoeff;

                koeff = PlayerCurrentSpeed * addKoeff * new Vector2(horizontal, vertical).magnitude - PlayerVelocity;

                koeff = koeff > 0 ? koeff : 0;

                _rigidbody.velocity += (_transform.forward) * koeff;

            }


            howLongMoving = 0;
            horizontal = 0;
            vertical = 0;
        }
        else
        {
            

            if (howLongMoving < 2 && IsGrounded)
            {                
                howLongMoving++;                
                _rigidbody.velocity = Vector3.zero;
            }

        }

        if (isSolidPlayerBodyON && _transform.position.y < 0.1f)
        {
            _rigidbody.MovePosition(new Vector3(_transform.position.x, 0.15f, _transform.position.z));
        }
    }


    private void GravityScale(Rigidbody r)
    {
        if (gm.IsBuildMode)
        {
            r.useGravity = false;
            return;
        }
        else if (!r.useGravity)
        {
            r.useGravity = true;
        }

        

        float fallingKoeff = 1;

        if (r.velocity.y >= 0)
        {
            r.AddForce(Physics.gravity * Globals.GRAVITY_KOEFF * r.mass);
        }
        else if (r.velocity.y < 0)
        {
            if (!IsFloating)
            {
                if (r.drag != 2) r.drag = 2;
                if (fallingKoeff < 8) fallingKoeff *= 1.2f;
                r.AddForce(Physics.gravity * r.mass * Globals.GRAVITY_KOEFF * fallingKoeff);
            }
            else
            {
                //if (r.drag != 3) r.drag = 3;
                r.AddRelativeForce(Vector3.forward * 20, ForceMode.Force);
                r.AddForce(Physics.gravity * r.mass * Globals.GRAVITY_KOEFF * 0.1f);
            }            
        }                
        
    }
    
}

public struct PhysicsCustomizing
{
    public float Mass;
    public float Drag;
    public float AngularDrag;

    public static PhysicsCustomizing GetData(PhysicObjects _type)
    {
        PhysicsCustomizing result = new PhysicsCustomizing();
        switch (_type)
        {
            case PhysicObjects.Player:
                result.Mass = Globals.MASS;
                result.Drag = Globals.DRAG;
                result.AngularDrag = Globals.ANGULAR_DRAG;
                break;

        }

        return result;
    }

    

}

public enum PhysicObjects
{
    Player,
    Ragdoll
}

public enum ApplyForceType
{
    Punch_easy,
    Punch_medium,
    Punch_large
}

public enum AnimationStates
{
    None,
    Idle,
    Run,
    Fly,
    Walk
}

using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerControl))]
public class InputControl : MonoBehaviour
{
    private Joystick joystick;
    private PlayerControl playerControl;
    private CameraControl cameraControl;
    private PointerDownOnly jump;
    private PointerDownOnly jumpUp;
    private PointerDownOnly jumpDown;
    private PointerMoveOnly mover;  
    private readonly float XLimit = 10;
    
    private GameManager gm;
    private BlockManager blockManager;
    private LevelControl lc;
    
    private Ray ray;
    private RaycastHit hit;
    private Camera _camera;
    private float cameraRayCast = 50f;

    private Vector3 markerPosition;
    private Block markerAim;
    public Vector3 GetMarkerPosition => markerPosition;
    public Block GetMarkerAim => markerAim;
    private Transform mainPlayer;
    

    private LayerMask ignoreMask;
    private LayerMask blockMask;


    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        blockManager = gm.BlockManager;
        lc = gm.LevelControl;
        _camera = gm.GetCamera();
        
        cameraControl = GameManager.Instance.GetCameraControl();
        playerControl = gameObject.GetComponent<PlayerControl>();

        ignoreMask = LayerMask.GetMask(new string[] { "player" });
        blockMask = LayerMask.GetMask(new string[] { "block" });

        if (!Globals.IsMobile)
        {
            Cursor.lockState = CursorLockMode.Locked;       
            Cursor.visible = false;
            gm.GetUI.Mover.SetActive(false);
            Globals.WORKING_DISTANCE = 30;
        }
        else
        {
            joystick = gm.GetUI.Joystick.GetComponent<Joystick>();
            jump = gm.GetUI.JumpMain.GetComponent<PointerDownOnly>();
            jumpUp = gm.GetUI.JumpUP.GetComponent<PointerDownOnly>();
            jumpDown = gm.GetUI.JumpDOWN.GetComponent<PointerDownOnly>();
            mover = gm.GetUI.Mover.GetComponent<PointerMoveOnly>();
            Globals.WORKING_DISTANCE = 20;
        }

        mainPlayer = gm.GetMainPlayerTransform();
        
    }

    private void getCloseBlockByType()
    {

        if (blockManager.CurrentActiveBlock == null) return;
        Stage s = lc.GetCurrentStage();
        if (s == null) return;

        float minDistance = 1000;
        Block b = default;

        for (int i = 0; i < s.Blocks.Count; i++)
        {
            if (blockManager.CurrentActiveBlock.BlockType == s.Blocks[i].BlockType && !s.Blocks[i].IsFinalized)
            {
                float distance = (markerPosition - s.Blocks[i].transform.position).magnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    b = s.Blocks[i];
                }

            }
        }

        if (b != null)
        {
            switch(b.BlockSize)
            {
                case BlockSizes.small:
                    if (minDistance <= 5) //5
                    {
                        markerAim = b;
                    }
                    else
                    {
                        markerAim = null;
                    }
                    break;

                case BlockSizes.medium:
                    if (minDistance <= 5) //4
                    {
                        markerAim = b;
                    }
                    else
                    {
                        markerAim = null;
                    }
                    break;

                case BlockSizes.large:
                    if (minDistance <= 5) //3 
                    {
                        markerAim = b;
                    }
                    else
                    {
                        markerAim = null;
                    }
                    break;
            }

            //print(b.gameObject.name + " = " + minDistance);
        }
        else
        {
            markerAim = null;
        }
    }

    // Update is called once per frame
    void Update()
    {        
        if (!gm.IsGameStarted || Globals.IsOptions) return;

        

        if (Globals.IsMobile)
        {
            forMobile();
        }
        else
        {
            forPC();
        }


        if (gm.IsBuildMode)
        {
            if (blockManager.IsBuildingBlocks && gm.IsWalkthroughGame)
            {
                if (Physics.Raycast(mainPlayer.position + Vector3.up * 2, _camera.transform.forward, out hit, Globals.WORKING_DISTANCE, blockMask)) //Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, 50f, ~ignoreMask, QueryTriggerInteraction.Ignore
                {
                    if (hit.collider.TryGetComponent(out Block bl) && !bl.IsFinalized)
                    {
                        markerAim = bl;
                    }
                    else
                    {
                        getCloseBlockByType();
                        //markerAim = null;
                    }
                }
                else
                {
                    getCloseBlockByType();
                    //markerAim = null;
                }
            }


            if (blockManager.IsBuildingBlocks)
            {
                if (Physics.Raycast(mainPlayer.position + Vector3.up * 2, _camera.transform.forward, out hit, Globals.WORKING_DISTANCE, ~ignoreMask, QueryTriggerInteraction.Ignore)) //Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, 50f, ~ignoreMask, QueryTriggerInteraction.Ignore
                {
                    markerPosition = hit.point;
                }
            }
            else if (blockManager.IsDestroingBlocks)
            {
                if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, Globals.WORKING_DISTANCE, blockMask)) //_camera.transform.position, _camera.transform.forward, out hit, 50f, blockMask
                {
                    if (hit.collider.TryGetComponent(out Block b) && b.IsFinalized)
                    {
                        if (blockManager.CurrentBlockToDelete != null && blockManager.CurrentBlockToDelete.Equals(b))
                        {
                            //
                        }
                        else
                        {
                            if (blockManager.CurrentBlockToDelete != null)
                            {
                                blockManager.CurrentBlockToDelete.MakeColorBadForDelete(false);
                            }

                            blockManager.CurrentBlockToDelete = b;
                            b.MakeColorBadForDelete(true);
                        }
                    }
                }
                else
                {
                    if (blockManager.CurrentBlockToDelete != null)
                    {
                        blockManager.CurrentBlockToDelete.MakeColorBadForDelete(false);
                        blockManager.CurrentBlockToDelete = null;
                    }

                    if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, Globals.WORKING_DISTANCE, ~ignoreMask, QueryTriggerInteraction.Ignore))
                    {
                        markerPosition = hit.point;
                    }
                }


            }
        }
                
    }



    private void forMobile()
    {        

        playerControl.SetHorizontal(joystick.Horizontal);
        playerControl.SetVertical(joystick.Vertical);

        //TODEL        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            playerControl.SetHorizontal(horizontal);
            playerControl.SetVertical(vertical);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gm.IsBuildMode)
            {
                playerControl.SetJumpUP();
            }
            else
            {
                playerControl.SetJump();
            }

        }

        if (jump.IsPressed)
        {
            playerControl.SetJump();
        }

        if (jumpUp.IsPressed)
        {
            playerControl.SetJumpUP();
        }

        if (jumpDown.IsPressed)
        {
            playerControl.SetJumpDOWN();
        }

        Vector2 delta2 = mover.DeltaPosition;
        Vector2 delta = delta2.normalized;

        if (gm.IsBuildMode && blockManager.IsChoosingBlocks) return;
        
        if (delta2.x > 0 || delta2.x < 0)
        {

            int sign = delta2.x > 0 ? 1 : -1;
            playerControl.SetRotationAngle(4 * sign/*200 * sign * Time.deltaTime*/);

        }
        else if (delta2.x == 0)
        {
            playerControl.SetRotationAngle(0);
        }

        if (Mathf.Abs(delta.y) > 0)
        {
            cameraControl.ChangeCameraAngleX(delta.y * -70 * Time.deltaTime);
        }        
    }

    private void forPC()
    {        
        if (Input.mouseScrollDelta.magnitude > 0)
        {            
            cameraControl.ChangeZoom(Input.mouseScrollDelta.y);
        }

        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            playerControl.SetHorizontal(horizontal);
            playerControl.SetVertical(vertical);
        }

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gm.IsBuildMode)
            {
                playerControl.SetJumpUP();
            }
            else
            {
                playerControl.SetJump();
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            playerControl.SetJumpDOWN();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            gm.SetBuildingMode(!gm.IsBuildMode);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (gm.IsBuildMode && !gm.IsWalkthroughGame)
            {
                if (!blockManager.IsDestroingBlocks)
                {
                    blockManager.StartDestroying();
                }
                else if (blockManager.IsDestroingBlocks)
                {
                    blockManager.StartBuilding();
                }
            }            
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (gm.IsBuildMode && !gm.IsWalkthroughGame)
            {
                if (blockManager.IsBuildingBlocks)
                {
                    blockManager.Rotate();
                }                
            }
        }
        else if (Input.GetMouseButtonDown(1) && gm.PointerClickedCount <= 0 && !gm.IsWalkthroughGame)
        {
            if (gm.IsBuildMode)
            {
                blockManager.CancelLastBlock();
            }
        }

        if (Input.GetMouseButtonDown(0) && gm.PointerClickedCount <= 0)
        {
            if (gm.IsBuildMode)
            {
                if (blockManager.IsBuildingBlocks)
                {
                    blockManager.BuildCurrentBlockCall();
                    gm.PointerClickedCount = 0.1f;
                }
                else if (blockManager.IsDestroingBlocks)
                {
                    blockManager.DeleteCurrentBlock();
                    gm.PointerClickedCount = 0.1f;
                }                
            }            
        }
        

        if (gm.IsBuildMode && blockManager.IsChoosingBlocks) return;

        Vector3 mouseDelta = new Vector3(
            Input.GetAxis("Mouse X") * Globals.MOUSE_X_SENS, 
            Input.GetAxis("Mouse Y") * Globals.MOUSE_Y_SENS, 0);

                
        if ((mouseDelta.x > 0) || (mouseDelta.x < 0))
        {
            float koeff = mouseDelta.x * 20 * Time.deltaTime;

            if (koeff > XLimit)
            {
                koeff = XLimit;
            }
            else if (koeff < -XLimit)
            {
                koeff = -XLimit;
            }

            
            playerControl.SetRotationAngle(koeff);
        }        
        else
        {
            playerControl.SetRotationAngle(0);
        }

        
        if (Mathf.Abs(mouseDelta.y) > 0)
        {            
            cameraControl.ChangeCameraAngleX(mouseDelta.y * -7 * Time.deltaTime);
        }
    }

}

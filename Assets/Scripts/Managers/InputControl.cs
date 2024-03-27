using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerControl))]
public class InputControl : MonoBehaviour
{
    private Joystick joystick;
    private PlayerControl playerControl;
    private CameraControl cameraControl;
    private Transform playerTransform;
    private PointerDownOnly jump;
    private PointerMoveOnly mover;  
    private readonly float XLimit = 10;
    
    private GameManager gm;
    private BlockManager blockManager;
    
    private Ray ray;
    private RaycastHit hit;
    private Camera _camera;
    private float cameraRayCast = 50f;

    private Vector3 marker;
    public Vector3 GetMarker => marker;
    private Transform mainPlayer;

    private LayerMask ignoreMask;


    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        blockManager = gm.BlockManager;
        _camera = gm.GetCamera();
        joystick = GameManager.Instance.GetJoystick();
        cameraControl = GameManager.Instance.GetCameraControl();
        playerControl = gameObject.GetComponent<PlayerControl>();
        playerTransform = playerControl.transform;

        ignoreMask = LayerMask.GetMask(new string[] { "player" });

        if (!Globals.IsMobile)
        {
            Cursor.lockState = CursorLockMode.Locked;       
            Cursor.visible = false;
            GameObject.Find("Screen mover").SetActive(false);
        }
        else
        {
            jump = GameObject.Find("JumpButton").GetComponent<PointerDownOnly>();
            mover = GameObject.Find("Screen mover").GetComponent<PointerMoveOnly>();            
        }

        mainPlayer = gm.GetMainPlayerTransform();
    }

    // Update is called once per frame
    void Update()
    {        
        if (!gm.IsGameStarted || Globals.IsOptions) return;
            
        /*
        if (blockManager.IsBuildingBlocks && blockManager.CurrentBlockToDelete != null)
        {
            blockManager.CurrentBlockToDelete.MakeColorBadForDelete(false);
            blockManager.CurrentBlockToDelete = null;
        }
        */

        if (Globals.IsMobile)
        {
            forMobile();           
        }
        else
        {
            forPC();
        }

        if (!Globals.IsMobile)
        {
            if (Input.GetMouseButtonDown(0) && gm.PointerClickedCount <= 0)
            {
                if (gm.IsBuildMode)
                {
                    gm.GetUI.BuildCurrentBlock();
                    gm.PointerClickedCount = 0.1f;
                }
            }
            else if (Input.GetMouseButtonDown(1) && gm.PointerClickedCount <= 0)
            {
                if (gm.IsBuildMode)
                {
                    gm.GetUI.ShowBlocksPanel();
                    gm.PointerClickedCount = 0.1f;
                }
            }
        }
        
        /*
        if (Input.GetMouseButton(0) && gm.PointerClickedCount <= 0)
        {            
            ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, cameraRayCast))
            {
                if (hit.collider != null)
                {
                    //gm.Texter.text = hit.collider.name + "\n" + Input.mousePosition;
                    //gm.Marker.position = hit.point;
                }
            }
        }*/

        if (gm.IsBuildMode)
        {
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, 50f, ~ignoreMask))
            {                
                if (blockManager.IsBuildingBlocks)
                {
                    marker = hit.point;
                }
                else if (blockManager.IsDestroingBlocks)
                {
                    print(hit.collider.gameObject.name + " = " + hit.collider.TryGetComponent(out Block b1));

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
                    else
                    {
                        if (blockManager.CurrentBlockToDelete != null)
                        {
                            blockManager.CurrentBlockToDelete.MakeColorBadForDelete(false);
                            blockManager.CurrentBlockToDelete = null;
                        }
                    }

                    if (!Globals.IsMobile && Input.GetMouseButton(0) && blockManager.CurrentBlockToDelete != null)
                    {
                        blockManager.DeleteCurrentBlock();
                    }
                }

                

                /*
                Vector3 hitP = hit.point;
                float distance = (hitP - playerTransform.position).magnitude;
                float distanceLimit = 5f;

                if (distance <= distanceLimit)
                {
                    marker = new Vector3(hitP.x, playerTransform.position.y, hitP.z);
                }
                else
                {
                    Vector3 newp = playerTransform.position + playerTransform.forward * distanceLimit;
                    marker = new Vector3(newp.x, playerTransform.position.y + 1f, newp.z);
                }
                */
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
            playerControl.SetJump();
        }
        
        if (jump.IsPressed)
        {
            playerControl.SetJump();
        }
        
        Vector2 delta2 = mover.DeltaPosition;
        Vector2 delta = delta2.normalized;


        if (delta2.x > 0 || delta2.x < 0)
        {
            
            int sign = delta2.x > 0 ? 1 : -1;
            playerControl.SetRotationAngle(200 * sign * Time.deltaTime * 1);

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
            cameraControl.ChangeZoom(Input.mouseScrollDelta.y, true);
        }

        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            playerControl.SetHorizontal(horizontal);
            playerControl.SetVertical(vertical);
        }

        
        if (Input.GetKeyDown(KeyCode.Space)/* || Input.GetMouseButtonDown(1)*/)
        {
            playerControl.SetJump();
        }

        
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

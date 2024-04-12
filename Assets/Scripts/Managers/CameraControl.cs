using DG.Tweening;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Material shadowedMaterial;

    private Transform mainPlayer;
    private PlayerControl playerControl;
    private Transform mainCamera;
    private Transform mainCamTransformForRaycast;
    private Transform outerCamera;
    private Camera _camera;
    private Vector3 outerCameraShiftVector = Vector3.zero;
    private Vector3 baseCameraBodyPosition = Vector3.zero;

    private float currentZoom;
    private float zoomTimer;
    private bool isZooming;

    private bool isUpdate = true;
    private float _timer;
    private float _timerCooldown;
    private LayerMask ignoreMask;
    private Ray ray;
    private RaycastHit hit;

    private float xLimitUp = 40;
    private float xLimitDown = 270;

    private bool isCameraIn;


    private Dictionary<MeshRenderer, Material> changedMeshRenderers = new Dictionary<MeshRenderer, Material>();
    private HashSet<MeshRenderer> renderers = new HashSet<MeshRenderer>();

    private HashSet<MeshRenderer> renderersToReturn = new HashSet<MeshRenderer>();

    private GameManager gm;

    private float defaultCameraDistance;
    private float currentCameraDistance;
    private WaitForSeconds fixedDelta = new WaitForSeconds(0.02f);
    private float previousDistance;

    private bool isBuildRegimeCorrected;
    private bool isNonBuildRegimeCorrected;

    public void SetData(Transform player, Transform cam, Transform mainCamTransform)
    {
        gm = GameManager.Instance;
        mainCamTransformForRaycast = mainCamTransform;
        _camera = mainCamTransform.GetComponent<Camera>();
        mainPlayer = player;
        playerControl = mainPlayer.GetComponent<PlayerControl>();
        mainCamera = cam;
        outerCamera = mainCamera.parent;
        mainCamera.localPosition = Globals.BasePosition;
        mainCamera.localEulerAngles = Globals.BaseRotation;
        baseCameraBodyPosition = Globals.BasePosition;

        ignoreMask = LayerMask.GetMask(new string[] { "trigger", "player", "ragdoll", "danger" });

        currentZoom = Globals.MainPlayerData.Zoom;
        Zoom(Globals.MainPlayerData.Zoom);

        outerCamera.eulerAngles += new Vector3(-25, 0, 0);
        defaultCameraDistance = (mainPlayerPoint - mainCamTransformForRaycast.position).magnitude;
    }

    public void SwapControlBody(Transform newTransform)
    {
        mainPlayer = newTransform;
        isUpdate = false;
        StartCoroutine(playSwap());
    }
    private IEnumerator playSwap()
    {
        outerCamera.DOMove(mainPlayer.position/* + basePosition*/, 0.1f);
        yield return new WaitForSeconds(0.1f);
        isUpdate = true;
    }

    public void ChangeZoom(float koeff)
    {
        if (koeff < 0 && Globals.MainPlayerData.Zoom <= Globals.ZOOM_LIMIT_HIGH)
        {
            Globals.MainPlayerData.Zoom += Globals.ZOOM_DELTA;
        }
        else if (koeff > 0 && Globals.MainPlayerData.Zoom > -Globals.ZOOM_LIMIT_LOW)
        {
            Globals.MainPlayerData.Zoom -= Globals.ZOOM_DELTA;
        }

        checkCorrectZoom();
    }

    private void checkCorrectZoom()
    {
        if (Globals.MainPlayerData.Zoom > Globals.ZOOM_LIMIT_HIGH)
        {
            Globals.MainPlayerData.Zoom = Globals.ZOOM_LIMIT_HIGH;
            Zoom(Globals.MainPlayerData.Zoom);
        }
        else if (Globals.MainPlayerData.Zoom < Globals.ZOOM_LIMIT_LOW)
        {
            Globals.MainPlayerData.Zoom = Globals.ZOOM_LIMIT_LOW;
            Zoom(Globals.MainPlayerData.Zoom);
        }
        else
        {
            Zoom(Globals.MainPlayerData.Zoom);
        }
    }

    public void Zoom(float koeff)
    {
        _camera.fieldOfView = koeff;
    }

    public void RotateCameraOnVector(Vector3 vec, float _time)
    {
        outerCamera.DORotate(outerCamera.eulerAngles + vec, _time).SetEase(Ease.Linear);
    }

    public void ResetCameraOnRespawn()
    {
        outerCamera.eulerAngles = Vector3.zero;
    }

    public void ResetCameraOnRespawn(Vector3 vec)
    {
        outerCamera.eulerAngles = new Vector3(/*outerCamera.localEulerAngles.x + */outerCameraShiftVector.x, vec.y, 0);
    }

    public void ChangeCameraAngleY(float angleY)
    {

        outerCamera.eulerAngles = new Vector3(outerCamera.eulerAngles.x, playerControl.angleYForMobile + angleY, outerCamera.eulerAngles.z);
    }

    public void ChangeCameraAngleX(float angleX)
    {
        if (!Globals.IsMobile && Mathf.Abs(angleX) > 5) return;


        if (angleX > 0 && outerCamera.localEulerAngles.x > (xLimitUp - 10) && outerCamera.localEulerAngles.x < xLimitUp) return;
        if (angleX < 0 && outerCamera.localEulerAngles.x < (xLimitDown + 10) && outerCamera.localEulerAngles.x > xLimitDown) return;


        outerCamera.localEulerAngles = new Vector3(outerCamera.localEulerAngles.x + angleX * 2, outerCamera.localEulerAngles.y, outerCamera.localEulerAngles.z);

    }

    private void changeCurrentCameraDistance(float val)        
    {
        currentCameraDistance += val;
        if (currentCameraDistance > 0.9f)
        {
            currentCameraDistance = 1;
        }
        else if (currentCameraDistance < 0)
        {
            currentCameraDistance = 0;
        }

        mainCamera.localPosition = Vector3.Lerp(baseCameraBodyPosition, new Vector3(0, 1f, 0.7f), currentCameraDistance);
    }

    private void setCurrentCameraDistance(float val)
    {
        currentCameraDistance = val;
        if (currentCameraDistance > 1)
        {
            currentCameraDistance = 1;
        }
        else if (currentCameraDistance <= 0.1f)
        {
            currentCameraDistance = 0;
        }

        Vector3 newVector = Vector3.Lerp(new Vector3(0, 1f, 0.7f), baseCameraBodyPosition, currentCameraDistance);
        mainCamera.DOLocalMove(newVector, 0.05f).SetEase(Ease.Linear);
    }

    // Update is called once per frame
    void Update()
    {   
        
        if (gm.IsBuildMode && !isBuildRegimeCorrected)
        {
            isBuildRegimeCorrected = true;
            isNonBuildRegimeCorrected = false;

            mainCamera.localPosition = Globals.BasePosition + new Vector3(2, 0, 1);
            baseCameraBodyPosition = Globals.BasePosition + new Vector3(2, 0, 1);

        }
        else if (!gm.IsBuildMode && !isNonBuildRegimeCorrected)
        {
            isBuildRegimeCorrected = false;
            isNonBuildRegimeCorrected = true;

            mainCamera.localPosition = Globals.BasePosition;
            baseCameraBodyPosition = Globals.BasePosition;
        }
        

        if (zoomTimer > 10)
        {
            zoomTimer = 0;

            if (currentZoom != Globals.MainPlayerData.Zoom)
            {
                currentZoom = Globals.MainPlayerData.Zoom;
                SaveLoadManager.Save();
            }
        }
        else
        {
            zoomTimer += Time.deltaTime;
        }


        outerCamera.position = mainPlayer.position;
        outerCamera.eulerAngles = new Vector3(outerCamera.eulerAngles.x, playerControl.angleYForMobile, outerCamera.eulerAngles.z);


        if (!isUpdate || !gm.IsGameStarted) return;

        /*
        if (Input.GetKeyDown(KeyCode.T))
        {
            changeCurrentCameraDistance(0.075f);
            print(currentCameraDistance);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            changeCurrentCameraDistance(-0.075f);
            print(currentCameraDistance);
        }*/
    }

    private void FixedUpdate()
    {
        
        if (_timer > _timerCooldown)
        {            
            _timer = 0;
            _timerCooldown = 0.02f;
            newSystem();
        }
        else
        {
            _timer += Time.deltaTime;
        }
    }

    private Vector3 mainPlayerPoint => mainPlayer.position + Vector3.up * 1.2f;


    private void newSystem()
    {
        if (isZooming) return;

        Vector3 playerPoint = mainPlayerPoint;

        if (Physics.Raycast(playerPoint, (mainCamTransformForRaycast.position - playerPoint).normalized, out hit, defaultCameraDistance, ~ignoreMask, QueryTriggerInteraction.Ignore))
        {
            float distToBarrier = (playerPoint - hit.point).magnitude;
            if (MathF.Abs(distToBarrier - previousDistance) < 1 && distToBarrier > 2) return;

            if (distToBarrier < defaultCameraDistance * 0.9f) distToBarrier *= 0.9f;

            float distKoeff = distToBarrier / defaultCameraDistance;

            if ((currentCameraDistance <= 0.1f && distKoeff <= 0.1f) || (currentCameraDistance - distKoeff) < -0.1f) return;
            
            setCurrentCameraDistance(distKoeff);
            previousDistance = distToBarrier;
        }
        else
        {
            if (currentCameraDistance < 1) setCurrentCameraDistance(1);
        }

    }
    private IEnumerator playZoom(int koeff)
    {
        isZooming = true;

        for (int i = 0; i < 200; i++)
        {            
            ChangeZoom(koeff);
            yield return fixedDelta;

            if (koeff > 0)
            {
                if (Physics.Raycast(mainPlayerPoint, (mainCamTransformForRaycast.position - mainPlayerPoint).normalized, out hit, defaultCameraDistance, ~ignoreMask, QueryTriggerInteraction.Ignore))
                {
                    //
                }
                else
                {
                    break;
                }
            }
            else
            {
                float currentDistance = (mainPlayerPoint - mainCamTransformForRaycast.position).magnitude;
                if ((defaultCameraDistance - currentDistance) <= 0.5f)
                {
                    break;
                }

                if (Physics.Raycast(mainPlayerPoint, (mainCamTransformForRaycast.position - mainPlayerPoint).normalized, out hit, defaultCameraDistance, ~ignoreMask, QueryTriggerInteraction.Ignore))
                {
                    break;
                }                
            }
        }

        isZooming = false;
        _timerCooldown = 0.5f;
    }


    /*
    private void oldSystem()
    {
        _timer = 0;

        renderers.Clear();
        renderersToReturn.Clear();

        float distance = (mainPlayer.position + Vector3.up - mainCamTransformForRaycast.position).magnitude;

        if (Physics.Raycast(mainCamTransformForRaycast.position, (mainPlayer.position + Vector3.up - mainCamTransformForRaycast.position).normalized, out hit, distance, ~ignoreMask))
        {
            if (hit.collider.TryGetComponent(out MeshRenderer mr) && !hit.collider.gameObject.isStatic)
            {
                renderers.Add(mr);

                if (!changedMeshRenderers.ContainsKey(mr))
                {
                    changedMeshRenderers.Add(mr, mr.material);
                    mr.material = shadowedMaterial;
                }
            }

            foreach (MeshRenderer item in changedMeshRenderers.Keys)
            {
                if (!renderers.Contains(item))
                {
                    renderersToReturn.Add(item);
                }
            }

            if (renderersToReturn.Count > 0)
            {
                foreach (var item in renderersToReturn)
                {
                    if (item.material != changedMeshRenderers[item])
                    {
                        item.material = changedMeshRenderers[item];
                    }
                    renderers.Remove(item);
                    changedMeshRenderers.Remove(item);
                }
            }

        }
        else
        {
            _timer += Time.deltaTime;
        }
    }*/
}

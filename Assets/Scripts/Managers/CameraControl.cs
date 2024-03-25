using DG.Tweening;
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
    private Vector3 outerCameraShiftVector = Vector3.zero;

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
    private float xLimitDown = 300;


    private Dictionary<MeshRenderer, Material> changedMeshRenderers = new Dictionary<MeshRenderer, Material>();
    private HashSet<MeshRenderer> renderers = new HashSet<MeshRenderer>();

    private HashSet<MeshRenderer> renderersToReturn = new HashSet<MeshRenderer>();

    private GameManager gm;

    private float defaultCameraDistance;
    private WaitForSeconds fixedDelta = new WaitForSeconds(0.02f);

    public void SetData(Transform player, Transform _camera, Transform mainCamTransform)
    {
        gm = GameManager.Instance;
        mainCamTransformForRaycast = mainCamTransform;
        mainPlayer = player;
        playerControl = mainPlayer.GetComponent<PlayerControl>();
        mainCamera = _camera;
        outerCamera = mainCamera.parent;
        mainCamera.localPosition = Globals.BasePosition;
        mainCamera.localEulerAngles = Globals.BaseRotation;

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

    public void ChangeZoom(float koeff, bool isSaving)
    {
        if (koeff > 0 && Globals.MainPlayerData.Zoom < Globals.ZOOM_LIMIT)
        {
            float add = Globals.ZOOM_DELTA;
            mainCamera.position += mainCamera.forward * add;
            Globals.MainPlayerData.Zoom += add;

        }
        else if (koeff < 0 && Globals.MainPlayerData.Zoom > -Globals.ZOOM_LIMIT)
        {
            float add = -Globals.ZOOM_DELTA;
            mainCamera.position += mainCamera.forward * add;
            Globals.MainPlayerData.Zoom += add;
        }

        if (isSaving)
        {
            defaultCameraDistance = (mainPlayerPoint - mainCamTransformForRaycast.position).magnitude;
        }

        checkCorrectZoom(isSaving);
    }

    private void checkCorrectZoom(bool isSaving)
    {
        if (Globals.MainPlayerData.Zoom > Globals.ZOOM_LIMIT)
        {
            Globals.MainPlayerData.Zoom = Globals.ZOOM_LIMIT;
            if (isSaving) defaultCameraDistance = (mainPlayerPoint - mainCamTransformForRaycast.position).magnitude;
            Zoom(Globals.MainPlayerData.Zoom);
        }
        else if (Globals.MainPlayerData.Zoom < -Globals.ZOOM_LIMIT)
        {
            Globals.MainPlayerData.Zoom = -Globals.ZOOM_LIMIT;
            if (isSaving) defaultCameraDistance = (mainPlayerPoint - mainCamTransformForRaycast.position).magnitude;
            Zoom(Globals.MainPlayerData.Zoom);
        }
    }

    public void Zoom(float koeff)
    {
        mainCamera.localPosition = Globals.BasePosition;
        mainCamera.position += mainCamera.forward * koeff;
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

    // Update is called once per frame
    void Update()
    {        
        if (zoomTimer > 30)
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

        if (_timer > _timerCooldown)
        {
            //oldSystem();
            _timer = 0;
            _timerCooldown = 0.2f;
            newSystem();


        }
        else
        {
            _timer += Time.deltaTime;
        }
    }

    private Vector3 mainPlayerPoint => mainPlayer.position + Vector3.up * 0.5f;

    private void newSystem()
    {
        if (isZooming) return;

        Vector3 playerPoint = mainPlayerPoint;
        

        if (Physics.Raycast(playerPoint, (mainCamTransformForRaycast.position - playerPoint).normalized, out hit, defaultCameraDistance, ~ignoreMask))
        {            
            StartCoroutine(playZoom(1));
        }
        else
        {
            float currentDistance = (playerPoint - mainCamTransformForRaycast.position).magnitude;
            if ((defaultCameraDistance - currentDistance) > 0.5f)
            {
                StartCoroutine(playZoom(-1));
            }
        }

    }
    private IEnumerator playZoom(int koeff)
    {
        isZooming = true;

        for (int i = 0; i < 100; i++)
        {
            ChangeZoom(koeff, false);
            yield return fixedDelta;

            if (koeff > 0)
            {
                if (Physics.Raycast(mainPlayerPoint, (mainCamTransformForRaycast.position - mainPlayerPoint).normalized, out hit, defaultCameraDistance, ~ignoreMask))
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

                if (Physics.Raycast(mainPlayerPoint, (mainCamTransformForRaycast.position - mainPlayerPoint).normalized, out hit, defaultCameraDistance, ~ignoreMask))
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

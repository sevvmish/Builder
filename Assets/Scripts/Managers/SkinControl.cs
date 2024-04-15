using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinControl : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] skins;
    [SerializeField] private MeshRenderer[] meshes;
    private bool isActive;

    private void Start()
    {
        this.isActive = true;
        for (int i = 0; i < skins.Length; i++)
        {
            skins[i].enabled = true;
        }
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = true;
        }
    }

    public void SetSkin(bool isActive)
    {
        if (skins.Length == 0) return;

        if (this.isActive && isActive) return;
        if (!this.isActive && !isActive) return;

        if (isActive)
        {
            this.isActive = true;
            for (int i = 0; i < skins.Length; i++)
            {
                skins[i].enabled = true;
            }
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].enabled = true;
            }
        }
        else
        {
            this.isActive = false;
            for (int i = 0; i < skins.Length; i++)
            {
                skins[i].enabled = false;
            }
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].enabled = false;
            }
        }
    }

    public bool GetSkinStatus { get => isActive; }
}

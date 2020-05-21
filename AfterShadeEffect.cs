using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterShade : MonoBehaviour
{
    public Material shadeMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, shadeMaterial);
    }
}

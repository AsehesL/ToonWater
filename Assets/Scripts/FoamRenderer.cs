﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoamRenderer : MonoBehaviour
{

    private bool m_IsInitialized;
    private Bounds m_Bounds;

    private FoamRenderCamera m_Camera;

	void Start ()
	{
	    m_IsInitialized = Init();
	}
	
	void Update () {
		
	}

    private bool Init()
    {
        MeshFilter meshfilter = GetComponent<MeshFilter>();
        if (!meshfilter || !meshfilter.sharedMesh)
            return false;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer)
            return false;
        m_Bounds = meshRenderer.bounds;
        Vector3 size = Vector3.Max( m_Bounds.size, Vector3.one);
        m_Bounds.size = size;

        m_Camera = CreateCamera();

        return true;
    }

    private FoamRenderCamera CreateCamera()
    {
        GameObject camGo = new GameObject("[Foam Render Camera]");
        camGo.transform.SetParent(transform);
        camGo.transform.position = m_Bounds.center;
        camGo.transform.rotation = Quaternion.Euler(90, 0, 0);

        Camera cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.aspect = m_Bounds.size.x / m_Bounds.size.z;
        cam.orthographicSize = m_Bounds.size.z * 0.5f;
        cam.nearClipPlane = -m_Bounds.size.y * 0.5f;
        cam.farClipPlane = m_Bounds.size.y * 0.5f;
        cam.cullingMask = 0;
        cam.allowHDR = false;
        cam.backgroundColor = Color.black;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.depthTextureMode |= DepthTextureMode.Depth;

        return camGo.AddComponent<FoamRenderCamera>();
    }

    void OnDrawGizmosSelected()
    {
        if (m_IsInitialized)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(m_Bounds.center, m_Bounds.size);
        }
    }
}
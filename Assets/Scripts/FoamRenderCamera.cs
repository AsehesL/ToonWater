using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class FoamRenderCamera : MonoBehaviour
{
    private Camera m_Camera;

    private RenderTexture m_InterectTexture;
    private RenderTexture m_FoamTexture;

    private CommandBuffer m_InterectCommandBuffer;
    private CommandBuffer m_FoamCommandBuffer;

    void Start()
    {
        m_Camera = gameObject.GetComponent<Camera>();
    }

    public void Render()
    {

    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class FoamRenderCamera : MonoBehaviour
{
    private Camera m_Camera;

    private RenderTexture m_InterectTexture;
    private RenderTexture m_FoamTexture;
	public RenderTexture m_DepthTexture;

    private CommandBuffer m_InterectCommandBuffer;
	private CommandBuffer m_DepthCommandBuffer;
    private CommandBuffer m_FoamCommandBuffer;

	private Material m_DepthMaterial;

	private void Awake()
	{
		m_Camera = gameObject.GetComponent<Camera>();

		m_DepthMaterial = new Material(Shader.Find("Hidden/Depth"));

		m_DepthTexture = new RenderTexture(1024, 1024, 24);
	}

	private void OnEnable()
	{
		if (m_DepthCommandBuffer == null)
		{
			m_DepthCommandBuffer = new CommandBuffer();
			m_DepthCommandBuffer.name = "[FoamDepth]";
		}

		m_Camera.AddCommandBuffer(CameraEvent.AfterImageEffects, m_DepthCommandBuffer);
	}

	private void OnDisable()
	{
		if (m_DepthCommandBuffer != null)
			m_Camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, m_DepthCommandBuffer);
	}

	private void OnDestroy()
	{
		if (m_DepthCommandBuffer != null)
			m_DepthCommandBuffer.Release();
		if (m_DepthTexture)
			Destroy(m_DepthTexture);
		if (m_DepthMaterial)
			Destroy(m_DepthMaterial);
	}

	private void OnPostRender()
	{
		m_DepthCommandBuffer.Clear();

		m_DepthCommandBuffer.SetRenderTarget(m_DepthTexture);
		m_DepthCommandBuffer.ClearRenderTarget(true, true, Color.black);
	}

	public void RenderDepth(Renderer renderer)
	{
		m_DepthCommandBuffer.DrawRenderer(renderer, m_DepthMaterial);
	}

	public void Render()
    {

    }

	private void OnGUI()
	{
		if (m_DepthTexture)
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_DepthTexture);
	}
}

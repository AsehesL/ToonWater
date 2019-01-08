using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using RenderTexture = UnityEngine.RenderTexture;

public class FoamRenderCamera : MonoBehaviour
{
    public Texture2D tex;

    private Camera m_Camera;

    private RenderTexture m_InterectTexture;
    private RenderTexture m_FoamTexture;
    private RenderTexture m_FoamPreTexture;
	public RenderTexture m_DepthTexture;

    private CommandBuffer m_InteractCommandBuffer;
	private CommandBuffer m_DepthCommandBuffer;
    private CommandBuffer m_FoamCommandBuffer;

	private Material m_DepthMaterial;
    private Material m_InteracMaterial;
    private Material m_FoamMaterial;

    private bool m_IsFoamRenderBegin;

	private void Awake()
	{
		m_Camera = gameObject.GetComponent<Camera>();

		m_DepthMaterial = new Material(Shader.Find("Hidden/Depth"));
        m_InteracMaterial = new Material(Shader.Find("Hidden/Interact"));
	    m_FoamMaterial = new Material(Shader.Find("Hidden/Foam"));

        m_DepthTexture = new RenderTexture(1024, 1024, 24);
        m_DepthTexture.DiscardContents(true, true);
	    m_InterectTexture = new RenderTexture(1024, 1024, 24);
	    m_InterectTexture.DiscardContents(true, true);
        m_FoamTexture = new RenderTexture(1024, 1024, 16);
	    m_FoamTexture.DiscardContents(true, true);
        m_FoamPreTexture = new RenderTexture(1024, 1024, 24);
        m_FoamPreTexture.DiscardContents(true, true);

        m_Camera.targetTexture = m_FoamTexture;
	}

	private void OnEnable()
	{
		if (m_DepthCommandBuffer == null)
		{
			m_DepthCommandBuffer = new CommandBuffer();
			m_DepthCommandBuffer.name = "[FoamDepth]";
		}

	    if (m_InteractCommandBuffer == null)
	    {
	        m_InteractCommandBuffer = new CommandBuffer();
	        m_InteractCommandBuffer.name = "[FoamInteract]";
	    }

        if (m_FoamCommandBuffer == null)
        {
            m_FoamCommandBuffer = new CommandBuffer();
            m_FoamCommandBuffer.name = "[FoamRenderBuffer]";
        }

        m_Camera.AddCommandBuffer(CameraEvent.AfterImageEffects, m_DepthCommandBuffer);
        m_Camera.AddCommandBuffer(CameraEvent.AfterImageEffects, m_InteractCommandBuffer);
	    m_Camera.AddCommandBuffer(CameraEvent.AfterImageEffects, m_FoamCommandBuffer);
    }

	private void OnDisable()
	{
		if (m_DepthCommandBuffer != null)
			m_Camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, m_DepthCommandBuffer);
	    if (m_InteractCommandBuffer != null)
	        m_Camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, m_InteractCommandBuffer);
	    if (m_FoamCommandBuffer != null)
	        m_Camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, m_FoamCommandBuffer);
    }

	private void OnDestroy()
	{
		if (m_DepthCommandBuffer != null)
			m_DepthCommandBuffer.Release();
	    if (m_InteractCommandBuffer != null)
	        m_InteractCommandBuffer.Release();
	    if (m_FoamCommandBuffer != null)
	        m_FoamCommandBuffer.Release();
        if (m_DepthTexture)
			Destroy(m_DepthTexture);
		if (m_DepthMaterial)
			Destroy(m_DepthMaterial);
	    if (m_InterectTexture)
	        Destroy(m_InterectTexture);
	    if (m_InteracMaterial)
	        Destroy(m_InteracMaterial);
        if(m_FoamMaterial)
            Destroy(m_FoamMaterial);
        if(m_FoamTexture)
            Destroy(m_FoamTexture);
        if (m_FoamPreTexture)
            Destroy(m_FoamPreTexture);
    }

	private void OnPostRender()
	{
		m_DepthCommandBuffer.Clear();

		m_DepthCommandBuffer.SetRenderTarget(m_DepthTexture);
		m_DepthCommandBuffer.ClearRenderTarget(true, true, Color.black);

        m_InteractCommandBuffer.Clear();
	    m_InteractCommandBuffer.SetRenderTarget(m_InterectTexture);
	    m_InteractCommandBuffer.ClearRenderTarget(true, true, Color.black);

        m_FoamCommandBuffer.Clear();
        m_FoamCommandBuffer.SetRenderTarget(m_FoamTexture);
        m_FoamCommandBuffer.ClearRenderTarget(true, true, Color.black);
    }

	public void RenderDepth(Renderer renderer)
	{
		m_DepthCommandBuffer.DrawRenderer(renderer, m_DepthMaterial);
	}

	public void RenderInteract(Renderer renderer)
    {
        m_InteracMaterial.SetTexture("_DepthTexture", m_DepthTexture);
        m_InteractCommandBuffer.DrawRenderer(renderer, m_InteracMaterial);
    }

    public void RenderFoam(Renderer renderer, float flowSpeed, float offsetSpeed)
    {
        //RenderTexture pre = null;
        

        m_FoamMaterial.SetVector("_Speed", new Vector4(flowSpeed, offsetSpeed));

        //m_FoamMaterial.SetPass(0);
        //Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
        ////if (m_IsFoamRenderBegin)
        ////{
        //    //    Graphics.Blit(m_FoamTexture, m_FoamPreTexture);

        ////}

        //m_FoamMaterial.SetTexture("_Interact", m_InterectTexture);
        //m_FoamMaterial.SetVector("_Speed", new Vector4(flowSpeed, offsetSpeed));

        ////m_FoamCommandBuffer.Blit(m_FoamTexture, m_FoamPreTexture);

        m_FoamMaterial.SetTexture("_Interact", m_InterectTexture);

        if(m_IsFoamRenderBegin)
            m_FoamMaterial.SetTexture("_Pre", m_FoamPreTexture);


        m_FoamCommandBuffer.DrawRenderer(renderer, m_FoamMaterial);

        if (m_IsFoamRenderBegin)
        {
            m_FoamCommandBuffer.Blit(m_FoamTexture, m_FoamPreTexture);
            
        }

        //m_FoamMaterial.SetTexture("_Pre", m_FoamTexture);

        //m_IsFoamRenderBegin = true;
        m_IsFoamRenderBegin = true;

        //if(pre)
        //    RenderTexture.ReleaseTemporary(pre);
    }

    //void OnRenderImage(RenderTexture src, RenderTexture dst)
    //{
    //    if (!m_FoamPreTexture)
    //    {
    //        m_FoamPreTexture = new RenderTexture(1024, 1024, 24);
    //    }
    //    Graphics.Blit(m_FoamTexture, m_FoamPreTexture);
    //    m_FoamMaterial.SetTexture("_Pre", m_FoamPreTexture);
    //    Graphics.Blit(src, dst);
    //}

	private void OnGUI()
	{
		if (m_FoamTexture)
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_FoamTexture);
	}
}

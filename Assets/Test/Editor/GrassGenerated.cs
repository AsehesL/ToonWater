using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GrassGenerated : EditorWindow
{
    private static System.Reflection.MethodInfo intersectRayMesh
    {
        get
        {
            if (m_IntersectRayMesh == null)
            {
                var tp = typeof(HandleUtility);
                m_IntersectRayMesh = tp.GetMethod("IntersectRayMesh",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            }
            return m_IntersectRayMesh;
        }
    }
    private static System.Reflection.MethodInfo m_IntersectRayMesh;

    private MeshFilter m_MeshFilter;

    private int m_Count;

    private Texture2D m_Mask;
    private Texture2D m_DirTex;
    private Texture2D m_SizeTex;
    private Texture2D m_ColorTex;
    private float m_BeginSize;
    private float m_DirOffset;

    [MenuItem("Tools/GrassGenerated")]
    static void Init()
    {
        GrassGenerated win = GrassGenerated.GetWindow<GrassGenerated>();
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneview)
    {

    }

    void OnGUI()
    {
        m_MeshFilter = EditorGUILayout.ObjectField("Mesh", m_MeshFilter, typeof(MeshFilter), true) as MeshFilter;
        m_Count = EditorGUILayout.IntField("Count", m_Count);
        m_Mask = EditorGUILayout.ObjectField("Mask", m_Mask, typeof(Texture2D), false) as Texture2D;
        m_DirTex = EditorGUILayout.ObjectField("Dir", m_DirTex, typeof(Texture2D), false) as Texture2D;
        m_SizeTex = EditorGUILayout.ObjectField("SizeTex", m_SizeTex, typeof(Texture2D), false) as Texture2D;
        m_ColorTex = EditorGUILayout.ObjectField("ColorTex", m_ColorTex, typeof(Texture2D), false) as Texture2D;
        m_BeginSize = EditorGUILayout.FloatField("BeginSize", m_BeginSize);
        m_DirOffset = EditorGUILayout.FloatField("DirOffset", m_DirOffset);

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
    }

    private void Generate()
    {
        if(m_MeshFilter == null || m_MeshFilter.sharedMesh == null)
            return;
        string path = EditorUtility.SaveFilePanel("", "", "", "asset");
        if(string.IsNullOrEmpty(path))
            return;
        path = FileUtil.GetProjectRelativePath(path);
        if(string.IsNullOrEmpty(path))
            return;

        Bounds bounds = default(Bounds);
        Vector3[] vertices = m_MeshFilter.sharedMesh.vertices;
        Vector3 max = Vector3.one*-Mathf.Infinity;
        Vector3 min = Vector3.one * Mathf.Infinity;
        for (int i = 0; i < vertices.Length; i++)
        {
            max = Vector3.Max(m_MeshFilter.transform.localToWorldMatrix.MultiplyPoint(vertices[i]), max);
            min = Vector3.Min(m_MeshFilter.transform.localToWorldMatrix.MultiplyPoint(vertices[i]), min);
        }

        Vector3 size = max - min;
        bounds = new Bounds(min + size * 0.5f, size);

        Mesh mesh = new Mesh();

        List<Vector3> vlist = new List<Vector3>();
        List<Vector2> ulist = new List<Vector2>();
        List<Color> clist = new List<Color>();
        List<int> ilist = new List<int>();

        for (int i = 0; i < m_Count; i++)
        {
            Vector3 origin = new Vector3(Random.Range(-0.5f, 0.5f) * bounds.size.x, bounds.size.y*3,
                                 Random.Range(-0.5f, 0.5f) * bounds.size.z) + bounds.center;
            Ray ray = new Ray(origin, Vector3.down);

            RaycastHit hit;
            if(RaycastMeshFilter(ray, m_MeshFilter, out hit) == false)
                continue;

            float uvx = 1.0f - (hit.point.x - bounds.min.x) / bounds.size.x;
            float uvy = 1.0f - (hit.point.z - bounds.min.z) / bounds.size.z;

            float maskV = m_Mask.GetPixel((int)(uvx * m_Mask.width), (int)(uvy * m_Mask.height)).r;
            float s = m_BeginSize + (1.0f - m_BeginSize) * m_SizeTex.GetPixel((int)(uvx * m_SizeTex.width), (int)(uvy * m_SizeTex.height)).r;
            Color dir = m_DirTex.GetPixel((int)(uvx * m_DirTex.width), (int)(uvy * m_DirTex.height));
            Color col = m_ColorTex.GetPixel((int) (uvx * m_ColorTex.width), (int) (uvy * m_ColorTex.height));
            float randPriority = Random.Range(0.0f, 1.0f);
            if (randPriority < maskV)
            {
                Vector3 p = hit.point;
                Vector2 u = new Vector2((dir.r * 2 - 1) * m_DirOffset, (dir.g * 2 - 1) * m_DirOffset);
                
                ilist.Add(vlist.Count);
                vlist.Add(p);
                ulist.Add(u);
                clist.Add(new Color(col.r, col.g, col.b, s));
            }
        }
        mesh.SetVertices(vlist);
        mesh.SetUVs(0, ulist);
        mesh.SetColors(clist);
        mesh.SetIndices(ilist.ToArray(), MeshTopology.Points, 0);

        AssetDatabase.CreateAsset(mesh, path);
    }

    bool RaycastMeshFilter(Ray ray, MeshFilter meshFilter, out RaycastHit hit)
    {
        var parameters = new object[] {ray, meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, null};
        bool result = (bool) intersectRayMesh.Invoke(null, parameters);
        hit = (RaycastHit) parameters[3];
        return result;
    }
}

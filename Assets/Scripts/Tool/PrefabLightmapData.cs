using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabLightmapData : MonoBehaviour
{
    [System.Serializable]
    struct RendererInfo
    {
        public Renderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
    }

    [SerializeField]
    RendererInfo[] m_RendererInfo;

    void Awake()
    {
        if (m_RendererInfo == null || m_RendererInfo.Length == 0)
            return;

        ApplyRendererInfo();
    }
    private void ApplyRendererInfo()
    {
        for (int i = 0; i < m_RendererInfo.Length; i++)
        {
            var info = m_RendererInfo[i];
            info.renderer.lightmapIndex = info.lightmapIndex;
            info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Bake Prefab Lightmaps")]
    static void GenerateLightmapInfo()
    {
        if (UnityEditor.Lightmapping.giWorkflowMode != UnityEditor.Lightmapping.GIWorkflowMode.OnDemand)
        {
            Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
            return;
        }
        UnityEditor.Lightmapping.Bake();

        PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();

        foreach (var instance in prefabs)
        {
            var gameObject = instance.gameObject;
            var rendererInfos = new List<RendererInfo>();

            GenerateLightmapInfo(gameObject, rendererInfos);

            instance.m_RendererInfo = rendererInfos.ToArray();

            var targetPrefab = UnityEditor.PrefabUtility.GetPrefabParent(gameObject) as GameObject;
            if (targetPrefab != null)
            {
                //UnityEditor.Prefab
                UnityEditor.PrefabUtility.ReplacePrefab(gameObject, targetPrefab);
            }
        }
    }

    static void GenerateLightmapInfo(GameObject root, List<RendererInfo> rendererInfos)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.lightmapIndex != -1)
            {
                RendererInfo info = new RendererInfo();
                info.renderer = renderer;
                info.lightmapIndex = renderer.lightmapIndex;
                info.lightmapOffsetScale = renderer.lightmapScaleOffset;

                //Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapFar;
                Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;

                rendererInfos.Add(info);
            }
        }
    }
#endif

}
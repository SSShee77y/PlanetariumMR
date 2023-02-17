using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[ExecuteInEditMode]
public class ExtractFalloff : MonoBehaviour
{
    public void OnEnable()
    {
        Lightmapping.RequestLightsDelegate testDel = (Light[] requests, Unity.Collections.NativeArray<LightDataGI> lightsOutput) =>
        {
            DirectionalLight dLight = new DirectionalLight();
            PointLight point = new PointLight();
            SpotLight spot = new SpotLight();
            RectangleLight rect = new RectangleLight();
            DiscLight disc = new DiscLight();
            Cookie cookie = new Cookie();
            LightDataGI ld = new LightDataGI();
            
            for (int i = 0; i < requests.Length; i++)
            {
                Light l = requests[i];
                switch (l.type)
                {
                    case UnityEngine.LightType.Directional: LightmapperUtils.Extract(l, ref dLight); LightmapperUtils.Extract(l, out cookie); ld.Init(ref dLight, ref cookie); break;
                    case UnityEngine.LightType.Point: LightmapperUtils.Extract(l, ref point); LightmapperUtils.Extract(l, out cookie); ld.Init(ref point, ref cookie); break;
                    case UnityEngine.LightType.Spot: LightmapperUtils.Extract(l, ref spot); LightmapperUtils.Extract(l, out cookie); ld.Init(ref spot, ref cookie); break;
                    case UnityEngine.LightType.Area: LightmapperUtils.Extract(l, ref rect); LightmapperUtils.Extract(l, out cookie); ld.Init(ref rect, ref cookie); break;
                    case UnityEngine.LightType.Disc: LightmapperUtils.Extract(l, ref disc); LightmapperUtils.Extract(l, out cookie); ld.Init(ref disc, ref cookie); break;
                    default: ld.InitNoBake(l.GetInstanceID()); break;
                }
                ld.cookieID = l.cookie?.GetInstanceID() ?? 0;
                ld.falloff = FalloffType.Linear;
                lightsOutput[i] = ld;
            }
        };
        Lightmapping.SetDelegate(testDel);
    }
    
    void OnDisable()
    {
        Lightmapping.ResetDelegate();
    }
}
using System.Collections.Generic;
using UnityEngine;

public class FogSystemManager : MonoBehaviour
{
    public static FogSystemManager Instance { get; private set; }
    
    [SerializeField] private List<FogLayerController> fogLayers = new();
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    public void ClearFog(float duration = 2f)
    {
        foreach (var layer in fogLayers)
        {
            if (layer != null)
                layer.FadeTo(0f, duration);
        }
    }
    
    public void IntensifyFog(float duration = 1f)
    {
        foreach (var layer in fogLayers)
        {
            if (layer != null)
                layer.FadeTo(0.9f, duration);
        }
    }
}
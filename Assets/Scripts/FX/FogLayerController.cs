using UnityEngine;
using System.Collections;

public class FogLayerController : MonoBehaviour
{
    private static readonly int ScrollX = Shader.PropertyToID("_ScrollX");
    private static readonly int ScrollY = Shader.PropertyToID("_ScrollY");
    private static readonly int Scale = Shader.PropertyToID("_Scale");
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");
    [SerializeField] private Material fogMaterial;
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.05f, 0.02f);
    [SerializeField] [Range(0f, 1f)] private float opacity = 0.6f;
    [SerializeField] private float breathingSpeed = 1f;
    [SerializeField] private float noiseScale = 2.0f; 
    
    private SpriteRenderer spriteRenderer;
    private Material instanceMaterial;
    private float baseOpacity;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseOpacity = opacity;
        
        if (fogMaterial != null)
        {
            instanceMaterial = new Material(fogMaterial);
            spriteRenderer.material = instanceMaterial;
        }
    }
    
    private void Update()
    {
        if (instanceMaterial == null) return;

        
        instanceMaterial.SetFloat(ScrollX, scrollSpeed.x);
        instanceMaterial.SetFloat(ScrollY, scrollSpeed.y);
        instanceMaterial.SetFloat(Scale, noiseScale);

        
        var breathing = Mathf.Sin(Time.time * breathingSpeed * 0.5f) * 0.05f + 1f;
        instanceMaterial.SetFloat(Alpha, opacity * breathing);
    }
    
    public void FadeTo(float targetOpacity, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(targetOpacity, duration));
    }
    
    private IEnumerator FadeRoutine(float target, float duration)
    {
        var startOpacity = opacity;
        var elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            opacity = Mathf.Lerp(startOpacity, target, elapsed / duration);
            yield return null;
        }
        
        opacity = target;
    }
}
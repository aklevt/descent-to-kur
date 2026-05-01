using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    //public bool IsDead => entity != null && entity.Stats.IsDead;
    public bool IsDead => entity != null && entity.Health <= 0;

    private bool isDying = false;

    private Entity entity;
    
    public event Action<GameObject> OnDeath;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        entity = GetComponent<Entity>();
    }

    public void TakeDamage(int damage)
    {
        if (isDying || IsDead || !entity) return;


        entity.Health = Mathf.Max(0, entity.Health - damage);
        
        Debug.Log($"{gameObject.name} получил урон: {damage}. ХП: {entity.Health}/{entity.MaxHealth}");

        //if (entity.Stats.IsDead)
        if (IsDead)
        {
            StartCoroutine(FlashRed());
            Die();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;
        
        CameraFollow.Instance?.ShakeMedium();
        OnDeath?.Invoke(gameObject);

        var cell = entity.CurrentCell;
        GridManager.Instance.UnregisterEntity(cell);

        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
    }

    private IEnumerator FadeOutAndDestroy()
    {
        var elapsed = 0f;
        var color = spriteRenderer.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            spriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}
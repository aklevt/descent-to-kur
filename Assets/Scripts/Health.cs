using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    public bool IsDead => entity != null && entity.Stats.IsDead;
    
    private bool isDying = false;

    private BaseEntity entity;
    
    public event Action<GameObject> OnDeath;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        entity = GetComponent<BaseEntity>();
    }

    public void TakeDamage(int damage)
    {
        if (isDying || IsDead || !entity) return;


        entity.Stats.ApplyDamage(damage);
        
        Debug.Log($"{gameObject.name} получил урон: {damage}. ХП: {entity.Stats.Health}/{entity.Stats.MaxHealth}");

        if (entity.Stats.IsDead)
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
        if (entity != null)
        {
            entity.UpdateVisualStatus();
        }
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
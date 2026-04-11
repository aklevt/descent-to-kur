using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    public bool IsDead { get; private set; } = false;

    private BaseEntity entity;
    
    public event Action<GameObject> OnDeath;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        entity = GetComponent<BaseEntity>();
    }
    
    private void Start()
    {
        if (entity != null)
        {
            currentHealth = entity.MaxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} получил урон: {damage}. ХП: {currentHealth}/{entity?.MaxHealth}");

        if (currentHealth <= 0) 
            Die();
        else StartCoroutine(FlashRed());
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        
        OnDeath?.Invoke(gameObject);
    
        var cell = GridManager.Instance.WorldToCell(transform.position);
        GridManager.Instance.UnregisterEntity(cell);

        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
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
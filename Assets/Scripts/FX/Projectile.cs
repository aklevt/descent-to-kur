using System;
using System.Collections;
using UnityEngine;

namespace FX
{
    /// <summary>
    /// Снаряд, летящий от источника к цели
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed = 10f;
        
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem trailEffect;
        
        [Header("Hit Effect")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private ParticleSystem hitParticles;

        private Vector3 targetPosition;
        private Action onHitCallback;
        private float animationSpeedMultiplier = 1f;

        /// <summary>
        /// Запустить снаряд к цели
        /// </summary>
        public void Launch(Vector3 from, Vector3 to, float speedMultiplier, Action onHit = null)
        {
            transform.position = from;
            targetPosition = to;
            onHitCallback = onHit;
            animationSpeedMultiplier = speedMultiplier;

            // Направить спрайт в сторону полета
            var direction = (to - from).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            StartCoroutine(FlyToTarget());
        }

        private IEnumerator FlyToTarget()
        {
            var scaledSpeed = speed * animationSpeedMultiplier;
            
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetPosition, 
                    scaledSpeed * Time.deltaTime
                );
                yield return null;
            }

            // Попадание
            transform.position = targetPosition;
            OnHit();
        }

        private void OnHit()
        {
            onHitCallback?.Invoke();
            
            if (hitParticles != null)
            {
                hitParticles.transform.SetParent(null);
                hitParticles.Play();
                Destroy(hitParticles.gameObject, 2f);
            }

            if (hitEffectPrefab != null)
            {
                var effect = Instantiate(hitEffectPrefab, targetPosition, Quaternion.identity);
                Destroy(effect, 1f);
            }

            Destroy(gameObject);
        }
    }
}
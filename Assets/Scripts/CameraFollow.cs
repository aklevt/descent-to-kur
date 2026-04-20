using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] private Transform playerTarget;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Shake Settings")] [SerializeField]
    private float defaultShakeDuration = 0.15f;

    [SerializeField] private float defaultShakeIntensity = 0.1f;

    [Header("Follow Settings")] [SerializeField]
    private bool smoothFollow = true;

    [SerializeField] private float followSpeed = 8f;

    private Vector3 focusOffset;

    private Transform currentTarget;
    private Vector3 shakeOffset;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetToPlayer();
    }

    private void LateUpdate()
    {
        if (currentTarget == null) return;
        var targetPos = currentTarget.position + offset + focusOffset;
        if (smoothFollow)
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        else
            transform.position = targetPos;

        transform.position += shakeOffset;
    }

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    public void ResetToPlayer()
    {
        currentTarget = playerTarget;
    }

    public void ShiftTowards(Vector3 worldPos, float strength = 0.25f)
    {
        focusOffset = (worldPos - playerTarget.position) * strength;
    }

    public void ResetFocus()
    {
        focusOffset = Vector3.zero;
    }

    public void ShakeLight() => StartCoroutine(ShakeRoutine(defaultShakeIntensity * 0.5f, defaultShakeDuration));
    public void ShakeMedium() => StartCoroutine(ShakeRoutine(defaultShakeIntensity, defaultShakeDuration));
    public void ShakeHeavy() => StartCoroutine(ShakeRoutine(defaultShakeIntensity * 2f, defaultShakeDuration * 1.5f));

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;
            shakeOffset = Random.insideUnitSphere * Mathf.Lerp(intensity, 0f, t);
            shakeOffset.z = 0;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
}
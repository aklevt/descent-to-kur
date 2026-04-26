using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] private Transform playerTarget;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Shake Settings")]
    [SerializeField] private float defaultShakeDuration = 0.15f;
    [SerializeField] private float defaultShakeIntensity = 0.1f;

    [Header("Follow Settings")]
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private float followSpeed = 8f;

    private Vector3 focusOffset;
    private Transform currentTarget;
    private Vector3 shakeOffset;
    
    private bool isDetached;
    private Vector3 freeLookTarget;
    private Vector2 dragStartMousePos;
    private Vector3 dragStartFreeLookTarget;
    
    private Camera mainCamera;
    private bool isPlayerTurn;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetToPlayer();
        mainCamera = Camera.main;
        
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnStateChanged += OnTurnStateChanged;
    }
    
    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnStateChanged -= OnTurnStateChanged;
    }
    
    private void OnTurnStateChanged(TurnState state)
    {
        isPlayerTurn = (state == TurnState.PlayerTurn);
        if (!isPlayerTurn)
        {
            ResetToPlayer();
        }
    }

    private void LateUpdate()
    {
        if (currentTarget == null)
            return;
        
        Vector3 targetPos;

        if (isDetached)
        {
            targetPos = freeLookTarget + offset;
        }
        else
        {
            targetPos = currentTarget.position + offset + focusOffset;
        }

        if (smoothFollow)
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        else
            transform.position = targetPos;

        transform.position += shakeOffset;
    }
    
    public void ResetFocus() 
    {
        isDetached = false;
        focusOffset = Vector3.zero;
    }

    public void StartDrag(Vector2 mousePos)
    {
        dragStartMousePos = mousePos;
        
        if (!isDetached)
        {
            isDetached = true;
            freeLookTarget = currentTarget.position + focusOffset;
        }
        dragStartFreeLookTarget = freeLookTarget;
    }

    public void UpdateDrag(Vector2 currentMousePos)
    {
        var mouseDelta = currentMousePos - dragStartMousePos;
        var worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(mouseDelta.x, mouseDelta.y, 0)) 
                         - mainCamera.ScreenToWorldPoint(Vector3.zero);
        
        freeLookTarget = dragStartFreeLookTarget - worldDelta;
    }

    public void EndDrag()
    {
    }


    public void ResetToPlayer() 
    {
        if (playerTarget != null)
        {
            currentTarget = playerTarget;
            ResetFocus();
        }
    }

    public void ShiftTowards(Vector3 worldPos, float strength = 0.25f)
    {
        focusOffset = (worldPos - playerTarget.position) * strength;
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
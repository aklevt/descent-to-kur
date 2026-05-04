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
    
    [Header("Free Look Settings")]
    [SerializeField] private float freeLookSpeed = 5f;
    
    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 8f;
    [SerializeField] private float zoomSpeed = 1f;

    private Vector3 focusOffset;
    private Transform currentTarget;
    private Vector3 shakeOffset;
    
    private bool isDetached;
    private Vector3 freeLookTarget;
    private Vector2 dragStartMousePos;
    private Vector3 dragStartFreeLookTarget;
    
    private bool hasBounds;
    private Vector3 minBounds;
    private Vector3 maxBounds;
    private Bounds tilemapBounds;
    
    private Camera mainCamera;
    private bool isPlayerTurn;
    
    private Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.LogError("[CameraFollow] MainCamera не найдена");
                }
            }
            return mainCamera;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("[CameraFollow] MainCamera не найдена в Awake");
        }
    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnStateChanged += OnTurnStateChanged;
    }
    
    /// <summary>
    /// Установить цель для камеры
    /// </summary>
    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
        currentTarget = target;
        Debug.Log($"<color=cyan>[CameraFollow]</color> Цель установлена: {target?.name}");
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
        
        if (hasBounds)
        {
            targetPos = ClampToBounds(targetPos);
        }

        if (smoothFollow)
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        else
            transform.position = targetPos;

        transform.position += shakeOffset;
    }
    
    /// <summary>
    /// Установить границы перемещения камеры на основе тайлмапа
    /// </summary>
    public void SetCameraBounds(Bounds bounds)
    {
        tilemapBounds = bounds;
        UpdateBoundsWithCurrentZoom(bounds);
    }

    /// <summary>
    /// Убрать ограничения камеры
    /// </summary>
    public void ClearBounds()
    {
        hasBounds = false;
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(position.y, minBounds.y, maxBounds.y),
            position.z
        );
    }
    
    public void ResetFocus() 
    {
        isDetached = false;
        focusOffset = Vector3.zero;
    }
    
    /// <summary>
    /// Переместить камеру в нужном направлении (для управления по WASD)
    /// </summary>
    public void MoveFreeLook(Vector2 direction)
    {
        if (!isDetached)
        {
            isDetached = true;
            freeLookTarget = currentTarget.position + focusOffset;
        }

        var movement = new Vector3(direction.x, direction.y, 0f) * freeLookSpeed * Time.deltaTime;
        freeLookTarget += movement;

        if (hasBounds)
        {
            var clampedPos = ClampToBounds(freeLookTarget + offset);
            freeLookTarget = clampedPos - offset;
        }
    }

    // public void StartDrag(Vector2 mousePos)
    // {
    //     dragStartMousePos = mousePos;
    //     
    //     if (!isDetached)
    //     {
    //         isDetached = true;
    //         freeLookTarget = currentTarget.position + focusOffset;
    //     }
    //     dragStartFreeLookTarget = freeLookTarget;
    // }
    //
    // public void UpdateDrag(Vector2 currentMousePos)
    // {
    //     var mouseDelta = currentMousePos - dragStartMousePos;
    //     var worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(mouseDelta.x, mouseDelta.y, 0)) 
    //                      - mainCamera.ScreenToWorldPoint(Vector3.zero);
    //     
    //     freeLookTarget = dragStartFreeLookTarget - worldDelta;
    // }
    //
    // public void EndDrag()
    // {
    // }


    public void ResetToPlayer() 
    {
        if (playerTarget != null)
        {
            currentTarget = playerTarget;
            ResetFocus();
        }
        else
        {
            Debug.LogWarning("[CameraFollow] playerTarget == null, используется PlayerMovement.Instance");
            if (Entities.PlayerMovement.Instance != null)
            {
                playerTarget = Entities.PlayerMovement.Instance.transform;
                currentTarget = playerTarget;
                ResetFocus();
            }
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
    
    /// <summary>
    /// Изменить зум камеры
    /// </summary>
    public void Zoom(float delta)
    {
        if (MainCamera == null) return;
        mainCamera.orthographicSize = Mathf.Clamp(
            mainCamera.orthographicSize - delta * zoomSpeed,
            minZoom,
            maxZoom
        );
        UpdateBoundsWithCurrentZoom(tilemapBounds);
    }
    
    /// <summary>
    /// Обновить границы с учётом текущего зума
    /// </summary>
    private void UpdateBoundsWithCurrentZoom(Bounds tilemapBounds)
    {
        var cameraHeight = mainCamera.orthographicSize * 2f;
        var cameraWidth = cameraHeight * mainCamera.aspect;

        minBounds = new Vector3(
            tilemapBounds.min.x + cameraWidth / 2f,
            tilemapBounds.min.y + cameraHeight / 2f,
            offset.z
        );

        maxBounds = new Vector3(
            tilemapBounds.max.x - cameraWidth / 2f,
            tilemapBounds.max.y - cameraHeight / 2f,
            offset.z
        );

        if (minBounds.x > maxBounds.x)
        {
            var center = (tilemapBounds.min.x + tilemapBounds.max.x) / 2f;
            minBounds.x = maxBounds.x = center;
        }

        if (minBounds.y > maxBounds.y)
        {
            var center = (tilemapBounds.min.y + tilemapBounds.max.y) / 2f;
            minBounds.y = maxBounds.y = center;
        }

        hasBounds = true;
    }

}
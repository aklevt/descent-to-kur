using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Sprites
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Tilemap obstaclesTilemap;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float stepDelay = 0.2f;

        private SpriteRenderer spriteRenderer;
        private Vector3Int logicalCellPos;
        private Vector3 targetWorldPos;
        private Vector2 moveInput;
        private float lastStepTime;
        private bool isKeyHeld;
        private bool isMoving;
        
        private bool isPlayerTurn = true;
        
        private Camera mainCamera;
        
        public Vector3Int CurrentCell => logicalCellPos;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            mainCamera = Camera.main;
        }
        
        private void OnEnable()
        {
            if (TurnManager.Instance != null)
                TurnManager.Instance.OnStateChanged += HandleTurnChanged;
        }
        
        private void OnDisable()
        {
            if (TurnManager.Instance != null)
                TurnManager.Instance.OnStateChanged -= HandleTurnChanged;
        }

        private void HandleTurnChanged(TurnState newState)
        {
            isPlayerTurn = (newState == TurnState.PlayerTurn);
        }

        private void Start()
        {
            logicalCellPos = obstaclesTilemap.WorldToCell(transform.position);
            targetWorldPos = obstaclesTilemap.GetCellCenterWorld(logicalCellPos);
            targetWorldPos.z = transform.position.z;
            transform.position = targetWorldPos;
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        private void Update()
        {
            MoveSmoothly();

            if (!isPlayerTurn || isMoving) 
                return;

            CheckInput();
        }
        
        private void MoveSmoothly()
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
            isMoving = Vector3.Distance(transform.position, targetWorldPos) > 0.01f;
        }

        private void CheckInput()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) 
                HandleMouseClick();

            if (moveInput != Vector2.zero && Time.time - lastStepTime >= stepDelay)
            {
                ProcessKeyboardStep();
            }
        }

        private void HandleMouseClick()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var worldPoint = mainCamera.ScreenToWorldPoint(mousePosition);
            var clickedCell = obstaclesTilemap.WorldToCell(worldPoint);

            if (clickedCell == logicalCellPos || obstaclesTilemap.HasTile(clickedCell)) return;

            var directionX = clickedCell.x - logicalCellPos.x;
            UpdateSpriteFlip(directionX);

            SetNewTarget(clickedCell);
        }

        private void ProcessKeyboardStep()
        {
            var direction = Vector3Int.zero;
            
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                direction.x = moveInput.x > 0 ? 1 : -1;
            else if (Mathf.Abs(moveInput.y) > 0)
                direction.y = moveInput.y > 0 ? 1 : -1;

            if (direction == Vector3Int.zero) return;

            if (direction.x != 0)
            {
                UpdateSpriteFlip(direction.x);
            }

            TryMove(direction);
        }


        private void UpdateSpriteFlip(int horizontalDirection)
        {
            if (horizontalDirection > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (horizontalDirection < 0)
            {
                spriteRenderer.flipX = true;
            }
        }

        private void TryMove(Vector3Int direction)
        {
            var nextCell = logicalCellPos + direction;

            if (!obstaclesTilemap.HasTile(nextCell))
            {
                SetNewTarget(nextCell);
            }
            else
            {
                lastStepTime = Time.time - stepDelay;
            }
        }

        private void SetNewTarget(Vector3Int targetCell)
        {
            logicalCellPos = targetCell;
            targetWorldPos = obstaclesTilemap.GetCellCenterWorld(logicalCellPos);
            targetWorldPos.z = transform.position.z;
            lastStepTime = Time.time;
        }
    }
}
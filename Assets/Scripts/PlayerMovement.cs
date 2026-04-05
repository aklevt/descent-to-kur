using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Sprites
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private float moveSpeed = 2f;
        // [SerializeField] private float stepDelay = 0.2f;
        [SerializeField] private int moveRange = 1;

        private SpriteRenderer spriteRenderer;
        private Vector3Int logicalCellPos;
        private Vector3 targetWorldPos;
        private Vector2 moveInput;
        private float lastStepTime;
        private bool isMoving;
        private bool isPlayerTurn = true;
        private Camera mainCamera;
        
        public static PlayerMovement Instance { get; private set; }

        public Vector3Int CurrentCell => logicalCellPos;

        private List<Vector3Int> availableMoves = new();

        public bool IsMoving => isMoving;

        private void Awake()
        {
            Instance = this;
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
            // Debug.Log(isPlayerTurn);
            if (isPlayerTurn)
            {
                UpdateAvailableMoves();
            }
            else
            {
                GridHighlighter.Instance.Clear();
            }
        }

        private void Start()
        {
            var startCell = GridManager.Instance.WorldToCell(transform.position);
            SetLogicalPosition(startCell);
            
            UpdateTargetPosition(logicalCellPos);
            transform.position = targetWorldPos;

            if (isPlayerTurn) UpdateAvailableMoves();
            
            if (TurnManager.Instance != null)
            {
                HandleTurnChanged(TurnManager.Instance.CurrentState);
            }
        }

        public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

        private void Update()
        {
            var wasMoving = isMoving;
            MoveSmoothly();

            if (wasMoving && !isMoving && isPlayerTurn)
            {
                UpdateAvailableMoves();
            }

            if (!isPlayerTurn || isMoving)
                return;

            CheckInput();
        }
        
        private void SetLogicalPosition(Vector3Int newCell)
        {
            GridManager.Instance.MoveEntity(logicalCellPos, newCell, gameObject);
    
            logicalCellPos = newCell;
        }

        private void MoveSmoothly()
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
            isMoving = Vector3.Distance(transform.position, targetWorldPos) > 0.01f;
        }

        private void CheckInput()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
                HandleMouseClick();

            // Временно отключено передвижение по WASD
            // if (moveInput != Vector2.zero && Time.time - lastStepTime >= stepDelay)
            //     ProcessKeyboardStep();
        }

        private void HandleMouseClick()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var worldPoint = mainCamera.ScreenToWorldPoint(mousePosition);
            var clickedCell = GridManager.Instance.WorldToCell(worldPoint);

            if (!availableMoves.Contains(clickedCell) || !GridManager.Instance.IsCellWalkable(clickedCell, gameObject))
            {
                UpdateAvailableMoves();
                return; 
            }

            var directionX = clickedCell.x - logicalCellPos.x;
            UpdateSpriteFlip(directionX);

            SetNewTarget(clickedCell);
        }

        // Временно отключено передвижение по WASD
        // private void ProcessKeyboardStep()
        // {
        //     var direction = Vector3Int.zero;
        //
        //     if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        //         direction.x = moveInput.x > 0 ? 1 : -1;
        //     else if (Mathf.Abs(moveInput.y) > 0)
        //         direction.y = moveInput.y > 0 ? 1 : -1;
        //
        //     if (direction == Vector3Int.zero) return;
        //
        //     UpdateSpriteFlip(direction.x);
        //     TryMove(direction);
        // }
        // private void TryMove(Vector3Int direction)
        // {
        //     var nextCell = logicalCellPos + direction;
        //
        //
        //     if (GridManager.Instance.IsCellWalkable(nextCell))
        //     {
        //         SetNewTarget(nextCell);
        //     }
        //     else
        //     {
        //         lastStepTime = Time.time - stepDelay;
        //     }
        // }

        private void UpdateSpriteFlip(int horizontalDirection)
        {
            if (horizontalDirection != 0)
                spriteRenderer.flipX = horizontalDirection < 0;
        }

        private void SetNewTarget(Vector3Int targetCell)
        {
            GridHighlighter.Instance.Clear();

            SetLogicalPosition(targetCell);
            UpdateTargetPosition(logicalCellPos);
            lastStepTime = Time.time;
        }

        private void UpdateTargetPosition(Vector3Int cell)
        {
            targetWorldPos = GridManager.Instance.GetCellCenterWorld(cell);
            targetWorldPos.z = transform.position.z;
        }

        private void UpdateAvailableMoves()
        {
            availableMoves.Clear();
            availableMoves = GridManager.Instance.GetWalkableTilesInRange(logicalCellPos, moveRange, gameObject);
            GridHighlighter.Instance.HighlightCells(availableMoves);
        }
    }
}
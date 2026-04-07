using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Sprites
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int moveRange = 1;

        private SpriteRenderer spriteRenderer;
        private Vector3Int logicalCellPos;
        private Vector3 targetWorldPos;
        
        public static PlayerMovement Instance { get; private set; }
        public bool IsMoving { get; private set; }
        public Vector3Int CurrentCell => logicalCellPos;
        public int MoveRange => moveRange;
        
        private void Awake()
        {
            Instance = this;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            logicalCellPos = GridManager.Instance.WorldToCell(transform.position);
            GridManager.Instance.RegisterFixedEntity(logicalCellPos, gameObject);
            
            PlaceOnCell();
        }

        private void PlaceOnCell()
        {
            UpdateTargetPosition(logicalCellPos);
            transform.position = targetWorldPos;
        }

        private void Update()
        {
            MoveSmoothly();
        }
        
        public void ExecuteMove(Vector3Int targetCell)
        {
            var directionX = targetCell.x - logicalCellPos.x;
            UpdateSpriteFlip(directionX);

            UpdateTargetPosition(targetCell);

            IsMoving = true;
        }
        
        private void SetLogicalPosition(Vector3Int newCell)
        {
            GridManager.Instance.MoveEntity(logicalCellPos, newCell, gameObject);
            logicalCellPos = newCell;
        }

        private void MoveSmoothly()
        {
            if (!IsMoving) return;

            transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetWorldPos) < 0.001f)
            {
                transform.position = targetWorldPos;
                IsMoving = false;

                var newCell = GridManager.Instance.WorldToCell(transform.position);
                SetLogicalPosition(newCell); 
            }
        }
        
        private void UpdateSpriteFlip(int horizontalDirection)
        {
            if (horizontalDirection != 0)
                spriteRenderer.flipX = horizontalDirection < 0;
        }

        private void UpdateTargetPosition(Vector3Int cell)
        {
            targetWorldPos = GridManager.Instance.GetCellCenterWorld(cell);
            targetWorldPos.z = transform.position.z;
            
        }
    }
}
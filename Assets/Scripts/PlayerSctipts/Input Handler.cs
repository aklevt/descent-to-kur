using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        ///???
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
        var clickedCell = ConvertClickToCell();
        var availableMoves = AbilityController.Instance.AvailableCells;

        if (availableMoves.Contains(clickedCell))
            AbilityController.Instance.DoAction(clickedCell);

        /*if (!availableMoves.Contains(clickedCell) || !GridManager.Instance.IsCellWalkable(clickedCell, gameObject))
        {
            UpdateAvailableMoves();
            return;
        }*/
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

    private Vector3Int ConvertClickToCell()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        var worldPoint = mainCamera.ScreenToWorldPoint(mousePosition);
        var clickedCell = GridManager.Instance.WorldPointToCell(worldPoint);
        return clickedCell;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3Int lastHoveredCell;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Проверка, что курсор не над UI элементом
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (TurnManager.Instance == null || TurnManager.Instance.CurrentState != TurnState.PlayerTurn)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            HandleMouseClick();

        if (Mouse.current == null) return;
        
        var mousePos = Mouse.current.position.ReadValue();
        var worldPoint = mainCamera.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, -mainCamera.transform.position.z));
        var hoveredCell = GridManager.Instance.WorldToCell(worldPoint);

        if (hoveredCell != lastHoveredCell)
        {
            lastHoveredCell = hoveredCell;
            AbilityController.Instance.HandleCellHover(hoveredCell);
        }
    }

    private void HandleMouseClick()
    {
        var mousePos = Mouse.current.position.ReadValue();
        var worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -mainCamera.transform.position.z));
        var clickedCell = GridManager.Instance.WorldToCell(worldPoint);

        AbilityController.Instance.HandleCellClick(clickedCell);
    }
}
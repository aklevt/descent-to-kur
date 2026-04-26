using Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3Int lastHoveredCell;
    
    private bool isCameraDragging;
    private Vector2 dragStartMousePos;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Core.LevelController.Instance == null || !Core.LevelController.Instance.IsLevelLoaded)
            return;

        HandleCameraInput();
        
        HandleAbilityHotkeys();

        if (Keyboard.current.spaceKey.isPressed)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (TurnManager.Instance == null || TurnManager.Instance.CurrentState != TurnState.PlayerTurn)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            HandleMouseClick();

        if (Mouse.current == null) return;

        HandleCellHover();
    }

    private void HandleCameraInput()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CameraFollow.Instance.ResetFocus();
        }

        if (Keyboard.current.spaceKey.isPressed && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isCameraDragging = true;
            dragStartMousePos = Mouse.current.position.ReadValue();
            CameraFollow.Instance.StartDrag(dragStartMousePos);
        }

        if (isCameraDragging && Keyboard.current.spaceKey.isPressed && Mouse.current.leftButton.isPressed)
        {
            var currentMousePos = Mouse.current.position.ReadValue();
            CameraFollow.Instance.UpdateDrag(currentMousePos);
        }

        if (isCameraDragging && (!Keyboard.current.spaceKey.isPressed || !Mouse.current.leftButton.isPressed))
        {
            isCameraDragging = false;
            CameraFollow.Instance.EndDrag();
        }
    }

    private void HandleAbilityHotkeys()
    {
        if (TurnManager.Instance == null || TurnManager.Instance.CurrentState != TurnState.PlayerTurn)
            return;

        var kb = Keyboard.current;
        if (kb == null) return;

        for (var i = 0; i < 9; i++)
        {
            var key = (Key)((int)Key.Digit1 + i);

            if (kb[key].wasPressedThisFrame)
            {
                AbilityController.Instance.SelectAbilityByIndex(i);
                break;
            }
        }
    }

    private void HandleMouseClick()
    {
        var mousePos = Mouse.current.position.ReadValue();
        var worldPoint = mainCamera.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, -mainCamera.transform.position.z));
        var clickedCell = GridManager.Instance.WorldToCell(worldPoint);

        AbilityController.Instance.HandleCellClick(clickedCell);
    }

    private void HandleCellHover()
    {
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
}
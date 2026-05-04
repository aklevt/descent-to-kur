using Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3Int lastHoveredCell;
    
    // private bool isCameraDragging;
    // private Vector2 dragStartMousePos;

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
        
        HandleEndTurnInput();

        // if (Keyboard.current.spaceKey.isPressed)
        //     return;

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
        // ПКМ - возврат камеры к игроку
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            CameraFollow.Instance?.ResetFocus();
        }

        // WASD - перемещение камеры
        var kb = Keyboard.current;
        if (kb == null) return;

        var cameraMovement = Vector2.zero;

        if (kb.wKey.isPressed) cameraMovement.y += 1f;
        if (kb.sKey.isPressed) cameraMovement.y -= 1f;
        if (kb.aKey.isPressed) cameraMovement.x -= 1f;
        if (kb.dKey.isPressed) cameraMovement.x += 1f;

        if (cameraMovement.sqrMagnitude > 0.01f)
        {
            CameraFollow.Instance?.MoveFreeLook(cameraMovement.normalized);
        }
        
        // Скролл колеса мыши - зум
        if (Mouse.current != null)
        {
            var scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.1f)
            {
                CameraFollow.Instance?.Zoom(scroll * 0.01f);
            }
        }
    }

    private void HandleEndTurnInput()
    {
        if (TurnManager.Instance == null || TurnManager.Instance.CurrentState != TurnState.PlayerTurn)
            return;

        var kb = Keyboard.current;
        if (kb == null) return;

        // Пробел - завершить ход
        if (kb.spaceKey.wasPressedThisFrame)
        {
            TurnManager.Instance.EndPlayerTurn();
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseOverSelectableChecker : MonoBehaviour
{
    public GraphicRaycaster graphicRaycaster;
    public EventSystem eventSystem;

    private PlayerControls input;

    private void Awake()
    {
        input = new PlayerControls();
    }

    private void MouseMoved(InputAction.CallbackContext context)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = context.ReadValue<Vector2>();
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        foreach(RaycastResult result in results)
        {
            UiSelectable selectable = result.gameObject.GetComponentInParent<UiSelectable>();
            if (selectable != null)
                selectable.OnSelect(null);
        }
    }

    public void Enable()
    {
        input.Ui.Enable();
        input.Ui.MouseMove.performed += MouseMoved;
    }

    public void Disable()
    {
        if (input != null)
        {
            input.Ui.MouseMove.performed -= MouseMoved;
            input.Ui.Disable();
        }
    }
}

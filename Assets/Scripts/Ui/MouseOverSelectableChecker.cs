using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseOverSelectableChecker : MonoBehaviour
{
    public GraphicRaycaster graphicRaycaster;
    public EventSystem eventSystem;

    private PlayerControls input;
    private UiSelectable currentlySelectedItem;

    private Dictionary<GameObject, UiSelectable> selectableDictionary = new Dictionary<GameObject, UiSelectable>();

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

        foreach (RaycastResult result in results)
        {
            if (!selectableDictionary.ContainsKey(result.gameObject))
                selectableDictionary.Add(result.gameObject, result.gameObject.GetComponentInParent<UiSelectable>());

            UiSelectable selectable = selectableDictionary[result.gameObject];
            if (selectable != null && selectable != currentlySelectedItem)
            {
                if (!selectable.RequirePointOnSelectable || selectable.PointIsOnSelectable(pointerData.position))
                {
                    selectable.SelectUnderlyingComponent();
                    currentlySelectedItem = selectable;
                }
            }
        }
    }

    public void SetSelectedItem(UiSelectable selectable)
    {
        currentlySelectedItem = selectable;
    }

    public void ClickCurrentItem()
    {
        if (currentlySelectedItem != null)
        {
            currentlySelectedItem.ForceClick();
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

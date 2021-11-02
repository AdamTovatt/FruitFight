using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultipleChoiceSlider : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public List<int> Choices;

    public delegate void ValueChangedHandler(MultipleChoiceSlider sender, int value);
    public event ValueChangedHandler OnValueChanged;

    public bool InputEnabled { get; set; } = true;

    public int Value { get { return sliderValue; } }

    private int sliderValue;
    private float lastInputTime;
    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private bool inputBurnedOut = false;

    void Awake()
    {
        lastInputTime = Time.time;
        playerControls = new PlayerControls();
    }

    public void InitializeInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        this.playerInput.SwitchCurrentActionMap("Ui");
        this.playerInput.onActionTriggered += HandleInput;
    }

    private void OnDestroy()
    {
        if (this.playerInput != null)
            this.playerInput.onActionTriggered -= HandleInput;
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        if (InputEnabled)
        {
            if (context.canceled)
            {
                inputBurnedOut = false;
                return;
            }

            if (context.performed)
            {
                if (Time.time - lastInputTime > 1f || !inputBurnedOut)
                {
                    if (context.action.id == playerControls.Ui.Move.id)
                    {
                        Vector2 inputValue = context.ReadValue<Vector2>();

                        if (Mathf.Abs(inputValue.x) < 0.3f)
                        {
                            inputBurnedOut = false;
                            return;
                        }

                        sliderValue += inputValue.x > 0 ? 1 : -1;
                        ValueChanged();
                        lastInputTime = Time.time;
                        inputBurnedOut = true;
                    }
                }
            }
        }
    }

    private void ValueChanged()
    {
        if (sliderValue < 0)
        {
            sliderValue = Choices.Count - 1;
        }

        if (sliderValue > Choices.Count - 1)
        {
            sliderValue = 0;
        }

        OnValueChanged?.Invoke(this, Choices[sliderValue]);
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}

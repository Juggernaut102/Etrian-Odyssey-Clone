using UnityEngine;

// The sole purpose of this class is to create a single input system actions instance, and share it with all other scripts that need to read player input.
// This is to avoid the issue of multiple instances of the input system actions being created, which can lead to conflicts and unintended behavior.
// By centralizing the input handling in this class, we can ensure that all scripts are using the same instance and that input is processed consistently throughout the game.
public class PlayerInputHandler : MonoBehaviour
{
    public InputSystem_Actions Controls { get; private set; }

    private void Awake()
    {
        Controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
    }
}

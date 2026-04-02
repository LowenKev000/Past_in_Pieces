using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class RotateObject : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference lookAction;
    public InputActionReference clickAction;

    [Header("Settings")]
    public float sensitivity = 0.2f;

    [Header("Axis Locking")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    private float currentXRotation = 0f;
    private float currentYRotation = 0f;
    private float currentZRotation = 0f;

    private void OnEnable()
    {
        lookAction.action.Enable();
        clickAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
        clickAction.action.Disable();
    }

    void Update()
    {
        if (!clickAction.action.IsPressed()) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

        if (!lockX)
            currentXRotation -= mouseDelta.y * sensitivity;

        if (!lockY)
            currentYRotation += mouseDelta.x * sensitivity;

        if (!lockZ)
            currentZRotation += mouseDelta.x * sensitivity * 0.5f;

        transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, currentZRotation);
    }
}
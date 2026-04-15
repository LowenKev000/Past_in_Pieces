using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class RotateObject : MonoBehaviour
{
    [Header("Debug")]
    [Tooltip("Enable debug logs for rotation input and values.")]
    public bool debugMode = false;

    [Header("Input Actions")]
    public InputActionReference lookAction;
    public InputActionReference clickAction;

    [Header("Settings")]
    public float sensitivity = 0.2f;

    [Header("Axis Locking")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    private float currentXRotation;
    private float currentYRotation;
    private float currentZRotation;

    private Vector3 initialRotation;

    private void Awake()
    {
        initialRotation = transform.rotation.eulerAngles;

        currentXRotation = initialRotation.x;
        currentYRotation = initialRotation.y;
        currentZRotation = initialRotation.z;

        if (debugMode)
            Debug.Log($"[RotateObject] Initial Rotation: {initialRotation}");
    }

    private void OnEnable()
    {
        lookAction.action.Enable();
        clickAction.action.Enable();

        if (debugMode)
            Debug.Log("[RotateObject] Input actions enabled");
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
        clickAction.action.Disable();

        if (debugMode)
            Debug.Log("[RotateObject] Input actions disabled");
    }

    void Update()
    {
        if (!clickAction.action.IsPressed()) return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

        if (debugMode)
            Debug.Log($"[RotateObject] Mouse Delta: {mouseDelta}");

        if (!lockX)
            currentXRotation -= mouseDelta.y * sensitivity;

        if (!lockY)
            currentYRotation += mouseDelta.x * sensitivity;

        if (!lockZ)
            currentZRotation += mouseDelta.x * sensitivity * 0.5f;

        transform.rotation = Quaternion.Euler(
            currentXRotation,
            currentYRotation,
            currentZRotation
        );

        if (debugMode)
            Debug.Log($"[RotateObject] Rotation: {currentXRotation}, {currentYRotation}, {currentZRotation}");
    }
}
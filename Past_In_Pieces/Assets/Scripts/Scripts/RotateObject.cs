using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class RotateObject : MonoBehaviour
{
    [Header("Debug")]
    [Tooltip("Enable debug logs for rotation input and values.")]
    public bool debugMode = false;

    [Header("Input Actions")]
    // Mouse or stick movement (used to determine how much to rotate)
    public InputActionReference lookAction;

    // Input used to trigger rotation (e.g. mouse button held down)
    public InputActionReference clickAction;

    [Header("Settings")]
    // Controls how fast the object rotates based on input
    public float sensitivity = 0.2f;

    [Header("Axis Locking")]
    // Allows disabling rotation on specific axes
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    // Stores the current rotation values we manually control
    private float currentXRotation;
    private float currentYRotation;
    private float currentZRotation;

    // Stores the starting rotation of the object
    private Vector3 initialRotation;


    private void Awake()
    {
        // Save the object's initial rotation so we can start from it
        initialRotation = transform.rotation.eulerAngles;

        currentXRotation = initialRotation.x;
        currentYRotation = initialRotation.y;
        currentZRotation = initialRotation.z;

        if (debugMode)
            Debug.Log($"[RotateObject] Initial Rotation: {initialRotation}");
    }


    private void OnEnable()
    {
        // Enable input actions when this object becomes active
        lookAction.action.Enable();
        clickAction.action.Enable();

        if (debugMode)
            Debug.Log("[RotateObject] Input actions enabled");
    }


    private void OnDisable()
    {
        // Disable input actions when this object is turned off
        lookAction.action.Disable();
        clickAction.action.Disable();

        if (debugMode)
            Debug.Log("[RotateObject] Input actions disabled");
    }


    void Update()
    {
        // Only rotate while the click/drag input is being held
        if (!clickAction.action.IsPressed()) return;

        // Prevent rotating when interacting with UI (buttons, menus, etc.)
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject()) return;

        // Read how much the mouse/stick moved this frame
        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

        if (debugMode)
            Debug.Log($"[RotateObject] Mouse Delta: {mouseDelta}");

        // Apply rotation based on input, respecting axis locks

        // Vertical mouse movement rotates around X (up/down tilt)
        if (!lockX)
            currentXRotation -= mouseDelta.y * sensitivity;

        // Horizontal mouse movement rotates around Y (left/right turn)
        if (!lockY)
            currentYRotation += mouseDelta.x * sensitivity;

        // Optional Z rotation for added effect (slight tilt/roll)
        if (!lockZ)
            currentZRotation += mouseDelta.x * sensitivity * 0.5f;

        // Apply the final rotation to the object
        transform.rotation = Quaternion.Euler(
            currentXRotation,
            currentYRotation,
            currentZRotation
        );

        if (debugMode)
            Debug.Log($"[RotateObject] Rotation: {currentXRotation}, {currentYRotation}, {currentZRotation}");
    }
}
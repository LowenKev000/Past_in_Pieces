using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCamera : MonoBehaviour
{
    [Header("Mode")]
    public bool freeCam = true;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference upAction;
    public InputActionReference downAction;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float verticalSpeed = 5f;
    public float mouseSensitivity = 2f;

    private float xRotation = 0f;

    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        upAction.action.Enable();
        downAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        upAction.action.Disable();
        downAction.action.Disable();
    }

    void Start()
    {
        ApplyCursorState();
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void ApplyCursorState()
    {
        if (freeCam)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMouseLook()
    {
        if (!freeCam && !Mouse.current.leftButton.isPressed)
            return;

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        float yRotation = transform.eulerAngles.y + mouseX;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        float verticalInput = 0f;
        if (upAction.action.IsPressed()) verticalInput += 1f;
        if (downAction.action.IsPressed()) verticalInput -= 1f;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 verticalMove = Vector3.up * verticalInput;

        transform.position += (move * moveSpeed + verticalMove * verticalSpeed) * Time.deltaTime;
    }

    public void SetFreeCam(bool value)
    {
        freeCam = value;
        ApplyCursorState();
    }
}
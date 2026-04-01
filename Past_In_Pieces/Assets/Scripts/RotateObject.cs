using UnityEngine;
using UnityEngine.InputSystem;

public class RotateObject : MonoBehaviour
{
    public InputActionReference lookAction;

    public float rotateAmount = 15f;
    public float stepInterval = 0.2f;

    private float timerX;
    private float timerY;

    void OnEnable()
    {
        lookAction.action.Enable();
    }

    void OnDisable()
    {
        lookAction.action.Disable();
    }

    void Update()
    {
        Vector2 input = lookAction.action.ReadValue<Vector2>();

        timerX += Time.deltaTime;
        timerY += Time.deltaTime;

        if (input.x > 0f && timerY >= stepInterval)
        {
            transform.Rotate(Vector3.up, rotateAmount, Space.World);
            timerY = 0f;
        }
        else if (input.x < 0f && timerY >= stepInterval)
        {
            transform.Rotate(Vector3.up, -rotateAmount, Space.World);
            timerY = 0f;
        }

        if (input.y > 0f && timerX >= stepInterval)
        {
            transform.Rotate(Vector3.right, -rotateAmount, Space.World);
            timerX = 0f;
        }
        else if (input.y < 0f && timerX >= stepInterval)
        {
            transform.Rotate(Vector3.right, rotateAmount, Space.World);
            timerX = 0f;
        }
    }
}
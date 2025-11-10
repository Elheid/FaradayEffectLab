using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //public float moveSpeed = 5f;
    public float lookSpeed = 0.05f;

    private InputSystem_Actions inputActions;
    //private Vector2 moveInput;
    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        //inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        //inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        //Debug.Log("CameraController update");

        // move
        //Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        //transform.position += move * moveSpeed * Time.deltaTime;

        // rotate
        yaw += lookInput.x * lookSpeed;
        pitch -= lookInput.y * lookSpeed;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}

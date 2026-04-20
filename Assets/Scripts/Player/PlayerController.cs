using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 0.1f;
    public float gravity = -20f;
    public Transform cameraHolder;

    private CharacterController characterController;
    private PlayerControls controls;
    private float xRotation = 0f;
    private float verticalVelocity;

    public Vector3 SpawnPosition { get; private set; }
    public Quaternion SpawnRotation { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void Start()
    {
        SpawnPosition = transform.position;
        SpawnRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (WorldGameManager.Instance != null && WorldGameManager.Instance.IsLevelComplete)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Look();
        Move();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool shouldUnlock = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = shouldUnlock ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = shouldUnlock;
        }
    }

    private void Move()
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        direction = Vector3.ClampMagnitude(direction, 1f);

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 velocity = direction * moveSpeed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void Look()
    {
        if (Cursor.lockState != CursorLockMode.Locked || cameraHolder == null)
        {
            return;
        }

        Vector2 lookInput = controls.Player.Look.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        characterController.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        xRotation = 0f;
        verticalVelocity = 0f;

        if (cameraHolder != null)
        {
            cameraHolder.localRotation = Quaternion.identity;
        }

        characterController.enabled = true;
    }
}

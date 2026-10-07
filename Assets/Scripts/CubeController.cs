using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;

    [Header("Mouse Look Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2.0f;
    public float minPitch = -40.0f; // Look down limit
    public float maxPitch = 60.0f;  // Look up limit

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.6f;

    private Rigidbody rb;
    private bool isGrounded;
    private float cameraPitch = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Freeze rotation on physics so the cube doesn't roll or tip over
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Lock and hide the mouse cursor in the game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Mouse Look Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the Cube left/right around the Y-axis
        transform.Rotate(Vector3.up * mouseX);

        // Pitch the Camera up/down around its X-axis
        if (cameraTransform != null)
        {
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        // 2. Ground Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // 3. Jump Input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Press Escape to unlock the cursor if needed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FixedUpdate()
    {
        // 4. WASD Movement relative to where the Cube/Camera is facing
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D
        float moveZ = Input.GetAxisRaw("Vertical");   // W/S

        Vector3 moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        // Apply movement while preserving gravity/jumping velocity
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }
}
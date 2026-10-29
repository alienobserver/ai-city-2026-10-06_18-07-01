using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class CubeController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float acceleration = 15.0f;
    public float jumpForce = 5.0f;

    [Header("Mouse Look Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2.0f;
    public float minPitch = -40.0f;
    public float maxPitch = 60.0f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.6f;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private float cameraPitch = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // If the math puzzle is open, STOP camera movement completely!
        if (MathPuzzleTrigger.IsPuzzleActive) return; 

        // --- YOUR EXISTING MOUSE LOOK CODE GOES HERE ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        if (cameraTransform != null)
        {
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        // 2. Ground Check
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);

        // 3. Jump Input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Trigger the jump animation
            animator.SetTrigger("Jump");
        }

        // 4. Update Animator Parameters for Movement
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        
        // Option A: Send exact speed value (if using a Blend Tree for walking/running)
        animator.SetFloat("Speed", horizontalSpeed);

        // Option B: Send a simple true/false boolean for walking vs idle
        bool isMoving = horizontalSpeed > 0.0001f;
        animator.SetBool("IsWalking", isMoving);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 smoothedVelocity = Vector3.Lerp(
            new Vector3(currentVelocity.x, 0, currentVelocity.z), 
            targetVelocity, 
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(smoothedVelocity.x, currentVelocity.y, smoothedVelocity.z);
    }
}
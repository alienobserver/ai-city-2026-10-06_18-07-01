using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Detect Spacebar press for Jump (only when grounded)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // Read WASD / Arrow Keys inputs
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveVertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * moveSpeed);
    }

    // Reset jump when sphere touches the ground
    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
}
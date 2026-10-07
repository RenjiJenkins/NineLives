using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Active Host Reference")]
    public HostLifeData currentHost;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    
    private float horizontalInput;
    private bool isGrounded;
    private int jumpsRemaining;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Ground Detection
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpsRemaining = currentHost ? currentHost.maxJumps : 1;
        }

        // Jump Execution
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentHost.jumpForce);
            jumpsRemaining--;
        }
    }

    private void FixedUpdate()
    {
        float speed = currentHost ? currentHost.moveSpeed : 8f;
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
    }

    // Call this when Julian switches host bodies upon death
    public void LoadHostData(HostLifeData newData)
    {
        currentHost = newData;
        boxCollider.size = newData.colliderSize;
        boxCollider.offset = newData.colliderOffset;
        
        if (animator && newData.animatorController)
            animator.runtimeAnimatorController = newData.animatorController;
    }
}
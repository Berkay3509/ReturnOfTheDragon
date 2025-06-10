using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpPower = 15f;
    [SerializeField] private float gravityScale = 5f;
    [SerializeField] private float linearDrag = 2f;

    [Header("Jump Timing")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX = 7f;
    [SerializeField] private float wallJumpY = 12f;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Mobile Controls")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private Button jumpButton;

    // Components
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    // State variables
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float horizontalInput;
    private bool isWallSliding;
    public Text WINTEXT;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Physics setup
        body.gravityScale = gravityScale;
        body.drag = linearDrag;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Mobile controls
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(() => jumpBufferCounter = jumpBufferTime);
        }
    }

    private void Update()
    {
        // Input handling
        horizontalInput = joystick != null ? joystick.Horizontal : Input.GetAxisRaw("Horizontal");

        // Character flipping
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Animation control
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        // Coyote time update
        if (isGrounded())
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        // Jump buffer countdown
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Win")
        {
            WINTEXT.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

    }
    private void FixedUpdate()
    {
        // Wall sliding check
        isWallSliding = onWall() && !isGrounded() && horizontalInput != 0;

        if (isWallSliding)
        {
            body.gravityScale = 0.5f;
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -2f, float.MaxValue));
        }
        else
        {
            body.gravityScale = gravityScale;
        }

        // Horizontal movement
        if (!isWallSliding)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        }

        // Jump execution (in FixedUpdate for better physics)
        if (jumpBufferCounter > 0 && CanJump())
        {
            Jump();
            jumpBufferCounter = 0;
        }
    }

    private bool CanJump()
    {
        return (isGrounded() || coyoteCounter > 0 || isWallSliding);
    }

    private void Jump()
    {
        // Reset vertical velocity before jumping
        body.velocity = new Vector2(body.velocity.x, 0);

        if (isWallSliding)
        {
            WallJump();
        }
        else
        {
            body.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        Vector2 wallJumpDirection = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY);
        body.AddForce(wallJumpDirection, ForceMode2D.Impulse);
        StartCoroutine(WallJumpCooldown());
    }

    private IEnumerator WallJumpCooldown()
    {
        float originalGravity = body.gravityScale;
        body.gravityScale = 0;
        yield return new WaitForSeconds(0.1f);
        body.gravityScale = originalGravity;
    }

    private bool isGrounded()
    {
        float extraHeight = 0.2f;
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            extraHeight,
            groundLayer
        );

#if UNITY_EDITOR
        Debug.DrawRay(boxCollider.bounds.center, Vector2.down * (boxCollider.bounds.extents.y + extraHeight),
                     hit.collider != null ? Color.green : Color.red);
#endif

        return hit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            new Vector2(transform.localScale.x, 0),
            0.1f,
            wallLayer
        );
        return hit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
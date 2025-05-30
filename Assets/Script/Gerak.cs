using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMoves : MonoBehaviour
{
    public float speed = 10f;
    public float jump = 15f;

    private float gerakan;
    private Rigidbody2D rb;

    [Header("Ground & Wall Check")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Input
        gerakan = Input.GetAxisRaw("Horizontal");

        // Flip sprite
        if (gerakan < 0)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (gerakan > 0)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        // Movement
        rb.linearVelocity = new Vector2(gerakan * speed, rb.linearVelocity.y);

        // Check ground and wall
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);

        // Wall Slide Logic
        isWallSliding = isTouchingWall && !isGrounded && gerakan != 0;

        if (isWallSliding && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = collision.transform;
        }

        if (collision.gameObject.CompareTag("Death"))
        {
            Die();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
        }
    }

    private void Die()
    {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;

        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

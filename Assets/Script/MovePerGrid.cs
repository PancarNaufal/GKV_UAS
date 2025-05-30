using UnityEngine;

public class MovePerGrid : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;
    public LayerMask obstacleLayer;
    public Animator animator;

    private bool isMoving = false;
    private Vector3 inputDirection;

    void Update()
    {
        if (!isMoving)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Cegah diagonal
            if (Mathf.Abs(horizontal) > 0) vertical = 0;

            inputDirection = new Vector3(horizontal, vertical, 0f);

            // Hanya jika ada input
            if (inputDirection != Vector3.zero)
            {
                animator.SetFloat("Horizontal", inputDirection.x);
                animator.SetFloat("Vertical", inputDirection.y);

                Vector3 targetPos = transform.position + inputDirection.normalized * gridSize;

                // Cek tabrakan
                if (!Physics2D.OverlapCircle(targetPos, 0.1f, obstacleLayer))
                {
                    StartCoroutine(MoveToPosition(targetPos));
                }
            }
        }
    }

    System.Collections.IEnumerator MoveToPosition(Vector3 target)
    {
        isMoving = true;
        animator.SetFloat("Speed", 1f);

        while ((target - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
        animator.SetFloat("Speed", 0f);
    }
}

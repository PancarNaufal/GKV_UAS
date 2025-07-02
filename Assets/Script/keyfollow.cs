using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
private Transform playerTransform;
public float followSpeed = 5f;
public Vector3 offset = new Vector3(1f, 1f, 0f); // Adjust as needed

private bool isFollowing = false;

public void StartFollowing(Transform player)
{
    playerTransform = player;
    isFollowing = true;
    GetComponent<SpriteRenderer>().enabled = true; // Make the key visible
}

void Update()
{
    if (isFollowing && playerTransform != null)
    {
        Vector3 targetPosition = playerTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
}
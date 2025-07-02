// ActivateSmallKey.cs

using UnityEngine;

public class ActivateSmallKey : MonoBehaviour
{
    public GameObject smallKeyPrefab;
    private GameObject currentSmallKey;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // MODIFIKASI: Berikan status kunci kepada player
                playerMovement.hasKey = true;
                Debug.Log("Player has obtained the key!");

                if (currentSmallKey == null)
                {
                    currentSmallKey = Instantiate(smallKeyPrefab, transform.position, Quaternion.identity);
                    FollowPlayer followScript = currentSmallKey.GetComponent<FollowPlayer>();
                    if (followScript != null)
                    {
                        followScript.StartFollowing(other.transform);
                    }
                }
                
                Destroy(gameObject);
            }
        }
    }
}
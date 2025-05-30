using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Rotasi berdasarkan kombinasi tombol arah
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) &&
            (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            // Atas + Kanan
            transform.rotation = Quaternion.Euler(0f, 0f, 45f);
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) &&
                 (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            // Atas + Kiri
            transform.rotation = Quaternion.Euler(0f, 0f, 135f);
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) &&
                 (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            // Bawah + Kanan
            transform.rotation = Quaternion.Euler(0f, 0f, -45f);
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) &&
                 (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            // Bawah + Kiri
            transform.rotation = Quaternion.Euler(0f, 0f, -135f);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Kiri
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Kanan
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f); // Bawah
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f); // Atas
        }

        float gerakanX = Input.GetAxis("Horizontal");
        float gerakanY = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(gerakanX * speed, gerakanY * speed);

    }
}
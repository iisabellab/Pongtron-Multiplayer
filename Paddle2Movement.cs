using Unity.Netcode;
using UnityEngine;

public class Paddle2Movement : NetworkBehaviour
{
    public float speed = 5.0f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return; // ✅ Only allow movement for the owner (Client)

        float moveDirection = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection = -1;
        }

        if (moveDirection != 0)
        {
            MoveServerRpc(moveDirection);
        }
    }

    [ServerRpc]
    private void MoveServerRpc(float moveDirection)
    {
        Vector2 newPosition = transform.position + Vector3.up * moveDirection * speed * Time.deltaTime;
        transform.position = newPosition; // ✅ Move paddle on the server so all clients see the update
    }
}

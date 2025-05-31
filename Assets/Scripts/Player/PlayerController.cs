using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void HandleInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        player.moveInput = input;

        if (input != Vector2.zero)
        {
            player.lastMoveDir = input;
        }

        rb.velocity = input * player.moveSpeed;
    }
}

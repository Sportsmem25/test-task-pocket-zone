using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Weapon weapon;
    private bool isFacingRight = true;
    private float speed = 3f;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var input = Vector2.zero;
        if (PlayerInput.Instance != null)
            input = PlayerInput.Instance.MoveInput;

        if(input.sqrMagnitude < 0.001f)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        rb.velocity = input * speed;
        
        if (input.x != 0f)
            Flip(input.x > 0f);
    }

    private void Flip(bool moveRight)
    {
        if (moveRight == isFacingRight) return;
        isFacingRight = moveRight;
        transform.localScale = new Vector3(isFacingRight ? 1f : -1f, 1f, 1f);
        if (weapon?.FirePoint != null)
            weapon.FirePoint.transform.Rotate(0f, 180f, 0f);
    }
}

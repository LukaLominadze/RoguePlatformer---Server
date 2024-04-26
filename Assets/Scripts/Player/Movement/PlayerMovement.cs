using Riptide;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PlayerCollision collision;
    [SerializeField] Rigidbody2D rbody;
    [SerializeField] BoxCollider2D boxCollider;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpFallMultiplier;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldownTime;

    private ushort index = 0;

    private short horizontalInput;

    private float originalTime;
    private float currentDashSpeed;

    private bool[] inputs;

    private bool isDashing = false;

    private bool jumped = false;

    enum DashState
    {
        ready, active, cooldown
    }

    DashState dashState = DashState.ready;

    private void Start()
    {
        inputs = new bool[2];
        originalTime = dashTime;
    }

    public void GetInputs(short horizontalInput, bool[] inputs)
    {
        this.horizontalInput = horizontalInput;
        this.inputs = inputs;
    }

    private void FixedUpdate()
    {
        if(horizontalInput != 0)
        {
            transform.localScale = new Vector3(horizontalInput, 1, 1);
        }

        if(dashState != DashState.active)
        {
            transform.position += Vector3.right * horizontalInput * movementSpeed;
        }
        Jump();
        Dash();

        if (NetworkManager.Singleton.CurrentTick % 2 != 0) return;
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientID.playerMovement);
        message.AddUShort(player.Id);
        message.AddUShort(NetworkManager.Singleton.CurrentTick);
        message.AddVector2(boxCollider.bounds.center);
        message.AddBool(isDashing);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private void Jump()
    {
        if (collision.OnGround())
        {
            if (inputs[0])
            {
                rbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumped = true;
                Invoke("CanJump", 0.025f);
            }
            return;
        }
        if(rbody.velocity.y < 0)
        {
            rbody.velocity += Vector2.up * Physics2D.gravity * fallMultiplier * Time.fixedDeltaTime;
        }
        else if(rbody.velocity.y > 0 && !inputs[0])
        {
            rbody.velocity += Vector2.up * Physics2D.gravity * lowJumpFallMultiplier * Time.fixedDeltaTime;
        }
    }

    private void CanJump()
    {
        jumped = false;
    }

    private void Dash()
    {
        switch (dashState)
        {
            case DashState.ready:
                if (inputs[1])
                {
                    currentDashSpeed = transform.localScale.x == 1 ? dashSpeed : -dashSpeed;
                    rbody.gravityScale = 0;
                    isDashing = true;
                    dashState = DashState.active;
                }
                break;
            case DashState.active:
                transform.position += Vector3.right * currentDashSpeed;
                rbody.velocity *= Vector2.right;
                dashTime -= Time.fixedDeltaTime;
                if(dashTime <= 0)
                {
                    dashTime = originalTime;
                    originalTime = dashCooldownTime;
                    rbody.gravityScale = 1;
                    isDashing = false;
                    dashState = DashState.cooldown;
                }
                break;
            case DashState.cooldown:
                dashCooldownTime -= Time.fixedDeltaTime;
                if(dashCooldownTime <= 0)
                {
                    dashCooldownTime = originalTime;
                    originalTime = dashTime;
                    dashState = DashState.ready;
                }
                break;
        }
    }
}

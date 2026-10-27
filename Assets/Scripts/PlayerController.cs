using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float swimSpeed = 5f;
    public float jumpForce = 7f;
    public Transform cam;

    Rigidbody rb;
    bool jumpPressed;
    bool inWater;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
    }

    public void SetInWater(bool value)
    {
        inWater = value;
        rb.useGravity = !value;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;

        // caiu para fora do mapa
        if (transform.position.y < -25f && GameManager.Instance != null)
            GameManager.Instance.FellInWater();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.GameEnded)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        float h = 0, v = 0, up = 0;
        var k = Keyboard.current;
        if (k != null)
        {
            if (k.aKey.isPressed || k.leftArrowKey.isPressed) h -= 1;
            if (k.dKey.isPressed || k.rightArrowKey.isPressed) h += 1;
            if (k.sKey.isPressed || k.downArrowKey.isPressed) v -= 1;
            if (k.wKey.isPressed || k.upArrowKey.isPressed) v += 1;
            if (k.spaceKey.isPressed) up += 1;
            if (k.leftShiftKey.isPressed) up -= 1;
        }

        Vector3 fwd = cam.forward; fwd.y = 0; fwd.Normalize();
        Vector3 right = cam.right; right.y = 0; right.Normalize();
        Vector3 dir = fwd * v + right * h;
        if (dir.magnitude > 1) dir.Normalize();

        if (inWater)
        {
            // nadando: sem gravidade, Espaco sobe, Shift desce
            Vector3 vel = dir * swimSpeed;
            vel.y = up * swimSpeed;
            rb.linearVelocity = vel;
        }
        else
        {
            // no gelo: anda e pula
            Vector3 vel = dir * moveSpeed;
            vel.y = rb.linearVelocity.y;
            rb.linearVelocity = vel;

            if (jumpPressed && IsGrounded())
                rb.linearVelocity = new Vector3(vel.x, jumpForce, vel.z);
        }
        jumpPressed = false;

        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.6f, ~0, QueryTriggerInteraction.Ignore);
    }
}
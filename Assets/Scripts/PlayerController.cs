// P-gelados - Jackeline e Matheus
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Pinguim: anda e pula no gelo, nada na agua, leva dano (empurrao + piscar) e anima.
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 6f;
    public float swimSpeed = 5f;
    public float jumpForce = 7f;
    public float turnSpeed = 12f;
    public Transform cam;

    [Header("Dano")]
    public float knockbackForce = 5f;
    public float knockbackTime = 0.25f;

    [Header("Animacao (opcional - procura sozinho)")]
    public Animator animator;

    Rigidbody rb;
    Collider col;
    Renderer[] renderers;

    float jumpBufferUntil;
    bool touchingWater;
    bool swimming;
    float waterSurfaceY = 0f;
    float noSwimUntil;
    float knockbackUntil;
    bool grounded;
    bool moving;
    float airTime;
    string currentAnim = "";
    Coroutine blinkRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (cam == null && Camera.main != null) cam = Camera.main.transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;
        renderers = GetComponentsInChildren<Renderer>();
    }

    // Chamado pelo script Water
    public void SetInWater(bool value)
    {
        SetInWater(value, waterSurfaceY);
    }

    public void SetInWater(bool value, float surfaceY)
    {
        touchingWater = value;
        if (value) waterSurfaceY = surfaceY;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.spaceKey.wasPressedThisFrame)
            jumpBufferUntil = Time.time + 0.15f;

        // caiu para fora do mapa
        if (transform.position.y < -25f && GameManager.Instance != null && !GameManager.Instance.GameEnded)
            GameManager.Instance.FellInWater();

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.GameEnded)
        {
            Vector3 ev = rb.linearVelocity;
            ev.x = 0f;
            ev.z = 0f;
            if (swimming) ev.y = 0f;
            rb.linearVelocity = ev;
            return;
        }

        if (cam == null && Camera.main != null) cam = Camera.main.transform;

        // ----- entrada -----
        float h = 0f, v = 0f, up = 0f;
        Keyboard k = Keyboard.current;
        if (k != null)
        {
            if (k.aKey.isPressed || k.leftArrowKey.isPressed) h -= 1f;
            if (k.dKey.isPressed || k.rightArrowKey.isPressed) h += 1f;
            if (k.sKey.isPressed || k.downArrowKey.isPressed) v -= 1f;
            if (k.wKey.isPressed || k.upArrowKey.isPressed) v += 1f;
            if (k.spaceKey.isPressed) up += 1f;
            if (k.leftShiftKey.isPressed) up -= 1f;
        }

        Vector3 fwd = Vector3.forward;
        Vector3 right = Vector3.right;
        if (cam != null)
        {
            fwd = cam.forward; fwd.y = 0f; fwd.Normalize();
            right = cam.right; right.y = 0f; right.Normalize();
        }
        Vector3 dir = fwd * v + right * h;
        if (dir.magnitude > 1f) dir.Normalize();
        moving = dir.sqrMagnitude > 0.01f;

        // ----- nadando ou andando? -----
        Vector3 center = col != null ? col.bounds.center : transform.position + Vector3.up * 0.5f;
        bool shouldSwim = touchingWater && Time.time >= noSwimUntil && center.y < waterSurfaceY - 0.05f;
        if (shouldSwim != swimming)
        {
            swimming = shouldSwim;
            rb.useGravity = !swimming;
            if (swimming && GameManager.Instance != null) GameManager.Instance.PlaySplash();
        }

        grounded = CheckGrounded();
        if (grounded) airTime = 0f; else airTime += Time.fixedDeltaTime;

        // durante o empurrao do dano, nao controla
        if (Time.time < knockbackUntil) return;

        bool wantJump = Time.time <= jumpBufferUntil;

        if (swimming)
        {
            // nadando: sem gravidade, Espaco sobe, Shift desce
            bool atSurface = center.y >= waterSurfaceY - 0.2f;
            Vector3 vel = dir * swimSpeed;
            float vy = up * swimSpeed;
            if (atSurface && vy > 0f) vy = 0f; // nao sai voando da agua

            // na superficie, ESPACO faz um pulo para sair da agua (ex.: subir no gelo)
            if (wantJump && atSurface)
            {
                vel = dir * moveSpeed;
                vy = jumpForce;
                noSwimUntil = Time.time + 0.5f;
                jumpBufferUntil = 0f;
                if (GameManager.Instance != null) GameManager.Instance.PlayJump();
            }
            vel.y = vy;
            rb.linearVelocity = vel;
        }
        else
        {
            // no gelo: anda e pula
            Vector3 vel = dir * moveSpeed;
            vel.y = rb.linearVelocity.y;
            if (wantJump && grounded)
            {
                vel.y = jumpForce;
                jumpBufferUntil = 0f;
                if (GameManager.Instance != null) GameManager.Instance.PlayJump();
            }
            rb.linearVelocity = vel;
        }

        if (moving)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, target, turnSpeed * Time.fixedDeltaTime));
        }
    }

    bool CheckGrounded()
    {
        if (swimming) return false;
        Vector3 c;
        float r;
        if (col != null)
        {
            Bounds b = col.bounds;
            c = b.center;
            r = b.extents.y;
        }
        else
        {
            c = transform.position + Vector3.up * 0.5f;
            r = 0.5f;
        }
        float castR = r * 0.6f;
        float dist = r - castR + 0.15f;
        RaycastHit hit;
        return Physics.SphereCast(c, castR, Vector3.down, out hit, dist, ~0, QueryTriggerInteraction.Ignore);
    }

    // ---------------- Dano ----------------
    public void Knockback(Vector3 from)
    {
        Vector3 away = transform.position - from;
        away.y = 0f;
        if (away.sqrMagnitude < 0.01f) away = -transform.forward;
        away.Normalize();
        Vector3 vel = away * knockbackForce;
        vel.y = swimming ? 0f : 3.5f;
        rb.linearVelocity = vel;
        knockbackUntil = Time.time + knockbackTime;
    }

    public void StartBlink(float duration)
    {
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkRoutine(duration));
    }

    IEnumerator BlinkRoutine(float duration)
    {
        float end = Time.time + duration;
        while (Time.time < end)
        {
            SetVisible(false);
            yield return new WaitForSeconds(0.07f);
            SetVisible(true);
            yield return new WaitForSeconds(0.07f);
        }
        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null) renderers[i].enabled = visible;
        }
    }

    public void TeleportTo(Vector3 pos, Quaternion rot)
    {
        rb.linearVelocity = Vector3.zero;
        rb.position = pos;
        rb.rotation = rot;
        transform.SetPositionAndRotation(pos, rot);
        touchingWater = false;
        swimming = false;
        rb.useGravity = true;
        knockbackUntil = 0f;
        StartBlink(1.5f);
    }

    // ---------------- Animacao (triggers do controller do pinguim) ----------------
    void UpdateAnimation()
    {
        if (animator == null) return;
        if (GameManager.Instance != null && GameManager.Instance.GameEnded) return;

        string wanted;
        if (swimming) wanted = "Swim";
        else if (!grounded && airTime > 0.15f) wanted = "Jump";
        else if (moving) wanted = "Walk";
        else wanted = "Idle2";
        SetAnim(wanted);
    }

    void SetAnim(string animName)
    {
        if (animator == null || animName == currentAnim) return;
        if (currentAnim != "") animator.ResetTrigger(currentAnim);
        currentAnim = animName;
        animator.SetTrigger(animName);
    }

    public void PlayDead() { SetAnim("Dead"); }
    public void PlayVictory() { SetAnim("Jump"); }
}

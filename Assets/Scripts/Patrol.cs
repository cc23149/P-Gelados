// P-gelados - Jackeline e Matheus
using UnityEngine;

// Inimigo: patrulha de um ponto ao outro e, se o jogador nadar por perto, persegue.
// Mesmos campos de antes (direction, distance, speed) + opcoes novas.
public class Patrol : MonoBehaviour
{
    [Header("Patrulha")]
    public Vector3 direction = Vector3.right;
    public float distance = 5f;
    public float speed = 2f;

    [Header("Perseguir o jogador (so quando ele esta nadando)")]
    public bool chasePlayer = true;
    public float chaseRange = 7f;
    public float chaseSpeed = 3.2f;
    public float leashRange = 14f;     // distancia maxima da rota
    public float chaseMaxY = -0.3f;    // so persegue se o jogador estiver abaixo disso

    [Header("Visual")]
    public float turnSpeed = 6f;
    public float yawOffset = 0f;       // se o modelo nadar "de lado", teste 90 / -90 / 180

    Vector3 home;
    float pathT;
    float pathDir = 1f;
    Transform target;

    void Start()
    {
        home = transform.position;
        if (direction.sqrMagnitude < 0.0001f) direction = Vector3.right;
        direction.Normalize();
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) target = p.transform;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.GameEnded) return;

        float dt = Time.deltaTime;
        Vector3 pos = transform.position;
        Vector3 pathPoint = home + direction * pathT;
        Vector3 newPos;

        bool chasing = false;
        Vector3 playerPoint = pos;
        if (chasePlayer && target != null)
        {
            playerPoint = target.position + Vector3.up * 0.4f;
            float d = Vector3.Distance(pos, playerPoint);
            bool playerSubmerged = target.position.y < chaseMaxY;
            bool insideLeash = Vector3.Distance(playerPoint, home + direction * (distance * 0.5f)) < leashRange + distance * 0.5f;
            chasing = playerSubmerged && insideLeash && d < chaseRange;
        }

        if (chasing)
        {
            newPos = Vector3.MoveTowards(pos, playerPoint, chaseSpeed * dt);
        }
        else if (Vector3.Distance(pos, pathPoint) > 0.6f)
        {
            // voltando para a rota
            newPos = Vector3.MoveTowards(pos, pathPoint, speed * 1.5f * dt);
        }
        else
        {
            pathT += pathDir * speed * dt;
            if (pathT >= distance) { pathT = distance; pathDir = -1f; }
            if (pathT <= 0f) { pathT = 0f; pathDir = 1f; }
            newPos = home + direction * pathT;
        }

        Vector3 delta = newPos - pos;
        transform.position = newPos;
        if (delta.sqrMagnitude > 0.000001f)
        {
            Quaternion look = Quaternion.LookRotation(delta.normalized) * Quaternion.Euler(0f, yawOffset, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * dt);
        }
    }
}

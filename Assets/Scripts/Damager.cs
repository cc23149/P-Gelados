// P-gelados - Jackeline e Matheus
using UnityEngine;

// Coloque em inimigos e obstaculos (collider como Trigger): encostar no jogador tira 1 vida.
public class Damager : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Hit(other);
    }

    void OnTriggerStay(Collider other)
    {
        Hit(other);
    }

    void Hit(Collider other)
    {
        if (!(other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)) return;
        if (GameManager.Instance != null) GameManager.Instance.TakeDamage(transform.position);
    }
}

<<<<<<< HEAD
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
=======
using UnityEngine;

public class Damager : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) GameManager.Instance.TakeDamage();
    }
}
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d

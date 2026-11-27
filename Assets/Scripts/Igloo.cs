// P-gelados - Jackeline e Matheus
using UnityEngine;

// Iglu: so da a vitoria se o jogador ja tiver os peixes necessarios.
public class Igloo : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (!(other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)) return;
        if (GameManager.Instance != null) GameManager.Instance.ReachIgloo();
    }
}

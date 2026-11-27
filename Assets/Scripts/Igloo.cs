<<<<<<< HEAD
// P-gelados - Jackeline e Matheus
using UnityEngine;

// Iglu: so da a vitoria se o jogador ja tiver os peixes necessarios.
=======
using UnityEngine;

>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d
public class Igloo : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
<<<<<<< HEAD
        if (!(other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)) return;
        if (GameManager.Instance != null) GameManager.Instance.ReachIgloo();
    }
}
=======
        if (other.CompareTag("Player")) GameManager.Instance.ReachIgloo();
    }
}
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d

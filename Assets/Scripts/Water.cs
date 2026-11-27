<<<<<<< HEAD
// P-gelados - Jackeline e Matheus
using UnityEngine;

// Volume de agua (trigger). Avisa o jogador que esta na agua e qual a altura da superficie.
public class Water : MonoBehaviour
{
    Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
    }

    float SurfaceY
    {
        get { return col != null ? col.bounds.max.y : transform.position.y; }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController p = other.GetComponentInParent<PlayerController>();
        if (p != null) p.SetInWater(true, SurfaceY);
    }

    void OnTriggerStay(Collider other)
    {
        PlayerController p = other.GetComponentInParent<PlayerController>();
        if (p != null) p.SetInWater(true, SurfaceY);
=======
using UnityEngine;

public class Water : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerController>().SetInWater(true);
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d
    }

    void OnTriggerExit(Collider other)
    {
<<<<<<< HEAD
        PlayerController p = other.GetComponentInParent<PlayerController>();
        if (p != null) p.SetInWater(false, SurfaceY);
    }
}
=======
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerController>().SetInWater(false);
    }
}
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d

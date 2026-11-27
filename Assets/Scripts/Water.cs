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
    }

    void OnTriggerExit(Collider other)
    {
        PlayerController p = other.GetComponentInParent<PlayerController>();
        if (p != null) p.SetInWater(false, SurfaceY);
    }
}

using UnityEngine;

public class Damager : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) GameManager.Instance.TakeDamage();
    }
}
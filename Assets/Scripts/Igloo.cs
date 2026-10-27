using UnityEngine;

public class Igloo : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) GameManager.Instance.ReachIgloo();
    }
}
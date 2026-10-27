using UnityEngine;

public class Fish : MonoBehaviour
{
    public float spin = 90f;

    void Update() { transform.Rotate(0, spin * Time.deltaTime, 0, Space.World); }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddFish();
            Destroy(gameObject);
        }
    }
}
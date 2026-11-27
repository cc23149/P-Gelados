// P-gelados - Jackeline e Matheus
using UnityEngine;

// Peixe coletavel: gira, flutua e some ao ser pego (+1 peixe).
public class Fish : MonoBehaviour
{
    public float spin = 90f;
    public float bobHeight = 0.2f;
    public float bobSpeed = 1.5f;

    Vector3 basePos;
    float phase;

    void Start()
    {
        basePos = transform.position;
        phase = Random.value * 6.28f;
    }

    void Update()
    {
        transform.Rotate(0f, spin * Time.deltaTime, 0f, Space.World);
        transform.position = basePos + Vector3.up * Mathf.Sin(Time.time * bobSpeed + phase) * bobHeight;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!(other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)) return;
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        GameManager.Instance.AddFish();
        Destroy(gameObject);
    }
}

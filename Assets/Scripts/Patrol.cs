using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Vector3 direction = Vector3.right;
    public float distance = 5f;
    public float speed = 2f;

    Vector3 start;

    void Start() { start = transform.position; direction.Normalize(); }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, distance);
        Vector3 newPos = start + direction * t;
        Vector3 move = newPos - transform.position;
        if (move.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(move);
        transform.position = newPos;
    }
}
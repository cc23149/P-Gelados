// P-gelados - Jackeline e Matheus
using UnityEngine;

// Faz a textura da agua "andar" para parecer que ha ondas.
public class WaterScroll : MonoBehaviour
{
    public Vector2 speed = new Vector2(0.03f, 0.015f);

    Material mat;

    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null) mat = r.material; // instancia propria deste objeto
    }

    void Update()
    {
        if (mat == null) return;
        mat.mainTextureOffset = speed * Time.time;
    }
}

<<<<<<< HEAD
// P-gelados - Jackeline e Matheus
using UnityEngine;
using UnityEngine.InputSystem;

// Camera que segue o pinguim (Q/E giram) e ganha neblina azul quando fica debaixo d'agua.
=======
using UnityEngine;

>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -8);
    public float smooth = 5f;
<<<<<<< HEAD
    public float rotateSpeed = 90f;

    [Header("Efeito debaixo d'agua")]
    public bool underwaterEffect = true;
    public float waterSurfaceY = 0f;
    public Color underwaterColor = new Color(0.05f, 0.32f, 0.55f, 1f);
    public float underwaterFogDensity = 0.04f;

    float yaw;
    bool underwater;
    bool origFog;
    Color origFogColor;
    float origFogDensity;
    FogMode origFogMode;

    void Start()
    {
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
        }

        Water w = FindFirstObjectByType<Water>();
        if (w != null)
        {
            Collider c = w.GetComponent<Collider>();
            if (c != null) waterSurfaceY = c.bounds.max.y;
        }

        origFog = RenderSettings.fog;
        origFogColor = RenderSettings.fogColor;
        origFogDensity = RenderSettings.fogDensity;
        origFogMode = RenderSettings.fogMode;

        if (target != null) transform.position = target.position + offset;
    }
=======
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d

    void LateUpdate()
    {
        if (target == null) return;
<<<<<<< HEAD

        Keyboard k = Keyboard.current;
        if (k != null)
        {
            if (k.qKey.isPressed) yaw -= rotateSpeed * Time.deltaTime;
            if (k.eKey.isPressed) yaw += rotateSpeed * Time.deltaTime;
        }

        Vector3 desired = target.position + Quaternion.Euler(0f, yaw, 0f) * offset;
        float t = 1f - Mathf.Exp(-smooth * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desired, t);
        transform.LookAt(target.position + Vector3.up);

        UpdateUnderwater();
    }

    void UpdateUnderwater()
    {
        bool u = underwaterEffect && transform.position.y < waterSurfaceY;
        if (u == underwater) return;
        underwater = u;
        if (underwater)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = underwaterColor;
            RenderSettings.fogDensity = underwaterFogDensity;
        }
        else
        {
            RenderSettings.fog = origFog;
            RenderSettings.fogMode = origFogMode;
            RenderSettings.fogColor = origFogColor;
            RenderSettings.fogDensity = origFogDensity;
        }
    }

    void OnDestroy()
    {
        if (underwater)
        {
            RenderSettings.fog = origFog;
            RenderSettings.fogMode = origFogMode;
            RenderSettings.fogColor = origFogColor;
            RenderSettings.fogDensity = origFogDensity;
        }
    }
}
=======
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smooth * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);
    }
}
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d

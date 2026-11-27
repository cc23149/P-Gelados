<<<<<<< HEAD
// P-gelados - Jackeline e Matheus
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

// Controla peixes (pontuacao), vidas, dano, vitoria e derrota.
=======
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

<<<<<<< HEAD
    [Header("Regras")]
    public int fishNeeded = 10;
    public int lives = 3;
    public float invulnTime = 1.5f;

    [Header("HUD (o menu P-Gelados preenche; se ficar vazio o jogo cria sozinho)")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text messageText;
    public GameObject messagePanel;
    public RectTransform fishBarFill;
    public Image[] lifeIcons;
    public Image damageFlash;
    public GameObject endPanel;
    public TMP_Text endTitleText;
    public TMP_Text endDetailText;

    [Header("Jogador")]
    public Transform player;
    public Transform respawnPoint;   // opcional: se vazio, renasce onde comecou

    [Header("Sons (opcional)")]
    public AudioClip collectSound;
    public AudioClip hurtSound;
    public AudioClip jumpSound;
    public AudioClip splashSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip music;
    [Range(0f, 1f)] public float musicVolume = 0.3f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    public bool GameEnded { get; private set; }
    public bool Won { get; private set; }
    public int FishCollected { get { return fish; } }
    public int Lives { get { return currentLives; } }

    int fish;
    int currentLives;
    int totalFish;
    float lastHit = -99f;
    float messageHideAt;
    string currentMessage = "";
    float startTime;
    float endTimeSeconds;
    Vector3 startPos;
    Quaternion startRot = Quaternion.identity;
    PlayerController playerCtrl;
    AudioSource sfxSource;
    AudioSource musicSource;

    static readonly Color LifeOn = new Color(1f, 0.35f, 0.4f, 1f);
    static readonly Color LifeOff = new Color(0.2f, 0.2f, 0.25f, 0.6f);

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Start()
    {
        currentLives = lives;
        startTime = Time.time;

        if (player != null)
        {
            startPos = player.position;
            startRot = player.rotation;
            playerCtrl = player.GetComponent<PlayerController>();
        }

        totalFish = FindObjectsByType<Fish>(FindObjectsSortMode.None).Length;
        if (totalFish > 0 && fishNeeded > totalFish) fishNeeded = totalFish; // garante que da pra ganhar

        if (scoreText == null || livesText == null || messageText == null)
            HudBuilder.Build(this); // HUD automatico se esquecerem de montar

        if (endPanel != null) endPanel.SetActive(false);
        if (damageFlash != null)
        {
            Color c = damageFlash.color;
            c.a = 0f;
            damageFlash.color = c;
        }
        ClearMessage();

        if (music != null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = music;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.spatialBlend = 0f;
            musicSource.Play();
        }

        UpdateHUD();
        ShowMessage("Colete " + fishNeeded + " peixes e chegue ao iglu!", 5f);
=======
    public int fishNeeded = 10;
    public int lives = 3;
    public float invulnTime = 1.5f;
    public TMP_Text scoreText, livesText, messageText;
    public Transform player;

    int fish;
    float lastHit = -99f;
    Vector3 startPos;
    public bool GameEnded { get; private set; }

    void Awake() { Instance = this; }

    void Start()
    {
        startPos = player.position;
        messageText.text = "";
        UpdateHUD();
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d
    }

    void Update()
    {
<<<<<<< HEAD
        Keyboard k = Keyboard.current;
        if (k != null)
        {
            if (k.rKey.wasPressedThisFrame || (GameEnded && k.enterKey.wasPressedThisFrame))
                Restart();
        }

        if (!GameEnded && messageText != null && currentMessage != "" && Time.time > messageHideAt)
            ClearMessage();

        if (damageFlash != null && damageFlash.color.a > 0f)
        {
            Color c = damageFlash.color;
            c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * 1.2f);
            damageFlash.color = c;
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ---------------- Coleta ----------------
=======
        if (GameEnded && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d
    public void AddFish()
    {
        if (GameEnded) return;
        fish++;
<<<<<<< HEAD
        Play(collectSound);
        UpdateHUD();
        if (fish == fishNeeded) ShowMessage("Peixes suficientes! Corra para o iglu!", 5f);
    }

    // ---------------- Dano ----------------
    public void TakeDamage()
    {
        DoDamage(Vector3.zero, false);
    }

    public void TakeDamage(Vector3 sourcePosition)
    {
        DoDamage(sourcePosition, true);
    }

    void DoDamage(Vector3 source, bool hasSource)
    {
        if (GameEnded || Time.time - lastHit < invulnTime) return;
        lastHit = Time.time;
        currentLives--;
        Play(hurtSound);
        Flash();
        UpdateHUD();
        if (currentLives <= 0) { Lose(); return; }

        if (playerCtrl != null)
        {
            if (hasSource) playerCtrl.Knockback(source);
            playerCtrl.StartBlink(invulnTime);
        }
    }

    // Chamado quando o jogador cai para fora do mapa
    public void FellInWater()
    {
        if (GameEnded) return;
        currentLives--;
        Play(hurtSound);
        Flash();
        UpdateHUD();
        if (currentLives <= 0) { Lose(); return; }
        Respawn();
        ShowMessage("Voce caiu! Tente de novo.", 2f);
=======
        UpdateHUD();
    }

    public void TakeDamage()
    {
        if (GameEnded || Time.time - lastHit < invulnTime) return;
        lastHit = Time.time;
        lives--;
        UpdateHUD();
        if (lives <= 0) Lose();
    }

    public void FellInWater()
    {
        if (GameEnded) return;
        lives--;
        UpdateHUD();
        if (lives <= 0) { Lose(); return; }
        Respawn();
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d
    }

    void Respawn()
    {
<<<<<<< HEAD
        if (player == null) return;
        Vector3 pos = respawnPoint != null ? respawnPoint.position : startPos;
        Quaternion rot = respawnPoint != null ? respawnPoint.rotation : startRot;
        if (playerCtrl != null) playerCtrl.TeleportTo(pos, rot);
        else player.SetPositionAndRotation(pos, rot);
        lastHit = Time.time;
    }

    // ---------------- Vitoria / Derrota ----------------
    public void ReachIgloo()
    {
        if (GameEnded) return;
        if (fish >= fishNeeded) Win();
        else ShowMessage("Faltam " + (fishNeeded - fish) + " peixes para alimentar a familia!", 2f);
    }

    void Win()
    {
        GameEnded = true;
        Won = true;
        endTimeSeconds = Time.time - startTime;
        Play(winSound);
        if (playerCtrl != null) playerCtrl.PlayVictory();
        if (musicSource != null) musicSource.volume = musicVolume * 0.4f;
        ShowEnd("VITORIA!", "A familia esta alimentada!");
=======
        var rb = player.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        player.position = startPos;
        lastHit = Time.time;
    }

    public void ReachIgloo()
    {
        if (GameEnded) return;
        if (fish >= fishNeeded)
        {
            GameEnded = true;
            messageText.text = "VITORIA! A familia esta alimentada!\nAperte R para jogar de novo";
        }
        else
        {
            messageText.text = "Faltam " + (fishNeeded - fish) + " peixes!";
        }
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d
    }

    void Lose()
    {
        GameEnded = true;
<<<<<<< HEAD
        Won = false;
        endTimeSeconds = Time.time - startTime;
        Play(loseSound);
        if (playerCtrl != null) playerCtrl.PlayDead();
        if (musicSource != null) musicSource.volume = musicVolume * 0.4f;
        ShowEnd("DERROTA", "Acabaram as vidas...");
    }

    void ShowEnd(string title, string subtitle)
    {
        ClearMessage();
        string details = subtitle
            + "\nPeixes: " + fish + " / " + fishNeeded
            + "     Vidas: " + currentLives
            + "     Tempo: " + FormatTime(endTimeSeconds)
            + "\n\nAperte R para jogar de novo";

        if (endPanel != null)
        {
            endPanel.SetActive(true);
            if (endTitleText != null) endTitleText.text = title;
            if (endDetailText != null) endDetailText.text = details;
        }
        else if (messageText != null)
        {
            messageText.text = title + "\n" + details;
            currentMessage = "fim";
            if (messagePanel != null) messagePanel.SetActive(true);
        }
    }

    static string FormatTime(float t)
    {
        int m = (int)(t / 60f);
        int s = (int)(t % 60f);
        return m.ToString("00") + ":" + s.ToString("00");
    }

    // ---------------- HUD ----------------
    public void ShowMessage(string text, float seconds)
    {
        if (messageText == null) return;
        if (currentMessage != text)
        {
            messageText.text = text;
            currentMessage = text;
        }
        messageHideAt = Time.time + seconds;
        if (messagePanel != null) messagePanel.SetActive(true);
    }

    void ClearMessage()
    {
        currentMessage = "";
        if (messageText != null) messageText.text = "";
        if (messagePanel != null) messagePanel.SetActive(false);
=======
        messageText.text = "DERROTA! Acabaram as vidas.\nAperte R para tentar de novo";
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d
    }

    void UpdateHUD()
    {
<<<<<<< HEAD
        if (scoreText != null) scoreText.text = "Peixes: " + fish + " / " + fishNeeded;
        if (livesText != null) livesText.text = "Vidas: " + currentLives;

        if (fishBarFill != null)
        {
            Vector2 a = fishBarFill.anchorMax;
            a.x = fishNeeded > 0 ? Mathf.Clamp01((float)fish / fishNeeded) : 0f;
            fishBarFill.anchorMax = a;
        }

        if (lifeIcons != null)
        {
            for (int i = 0; i < lifeIcons.Length; i++)
            {
                if (lifeIcons[i] != null) lifeIcons[i].color = i < currentLives ? LifeOn : LifeOff;
            }
        }
    }

    void Flash()
    {
        if (damageFlash == null) return;
        Color c = damageFlash.color;
        c.a = 0.45f;
        damageFlash.color = c;
    }

    // ---------------- Som ----------------
    public void Play(AudioClip clip)
    {
        if (clip != null && sfxSource != null) sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayJump() { Play(jumpSound); }
    public void PlaySplash() { Play(splashSound); }
}
=======
        scoreText.text = "Peixes: " + fish + " / " + fishNeeded;
        livesText.text = "Vidas: " + lives;
    }
}
>>>>>>> 7fbd2e63d41476789740341ce6fb4952d1cad78d

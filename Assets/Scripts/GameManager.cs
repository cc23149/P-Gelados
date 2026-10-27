using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    }

    void Update()
    {
        if (GameEnded && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddFish()
    {
        if (GameEnded) return;
        fish++;
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
    }

    void Respawn()
    {
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
    }

    void Lose()
    {
        GameEnded = true;
        messageText.text = "DERROTA! Acabaram as vidas.\nAperte R para tentar de novo";
    }

    void UpdateHUD()
    {
        scoreText.text = "Peixes: " + fish + " / " + fishNeeded;
        livesText.text = "Vidas: " + lives;
    }
}
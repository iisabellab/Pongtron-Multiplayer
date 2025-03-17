using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class BallController : NetworkBehaviour
{
    public static BallController Instance;
    public Rigidbody2D rb;
    public float initialSpeed = 5f;
    public float speedIncreaseFactor = 1.05f;

    private AudioSource paddleHitAudio;
    private AudioSource wallHitAudio;
    private AudioSource resetAudio;

    private bool gameOver = false;
    private bool gameStarted = false; 

    private void Awake()
    {
        if (Instance == null) Instance = this;

        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 3)
        {
            paddleHitAudio = audioSources[0];
            wallHitAudio = audioSources[1];
            resetAudio = audioSources[2];
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer && !gameStarted) 
        {
            gameStarted = true;
            StartCoroutine(LaunchBallAfterDelay(1f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer || gameOver) return; // Prevent movement if the game is over

        if (collision.gameObject.CompareTag("Paddle"))
        {
            rb.linearVelocity *= speedIncreaseFactor; // Keep velocity instead of linearVelocity

            if (paddleHitAudio != null && paddleHitAudio.clip != null)
            {
                paddleHitAudio.Play();
            }
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            if (wallHitAudio != null && wallHitAudio.clip != null)
            {
                wallHitAudio.Play();
            }
        }
        else if (collision.gameObject.CompareTag("GoalWall"))
        {
            ScoreCount scoreManager = FindFirstObjectByType<ScoreCount>();

            if (collision.gameObject.name == "LeftWall")
            {
                scoreManager?.Player2Goal();
            }
            else if (collision.gameObject.name == "RightWall")
            {
                scoreManager?.Player1Goal();
            }

            if (!gameOver) 
            {
                StartCoroutine(ResetBall());
            }
        }
    }

    private IEnumerator ResetBall()
    {
        rb.linearVelocity = Vector2.zero; // Stops movement instead of linearVelocity
        transform.position = Vector3.zero;

        if (resetAudio != null && resetAudio.clip != null)
        {
            resetAudio.Play();
        }

        yield return new WaitForSeconds(1f);

        if (!gameOver) 
        {
            LaunchBall();
        }
    }

    private void LaunchBall()
    {
        Vector2 randomDirection = new Vector2(Random.Range(0, 2) == 0 ? -1 : 1, Random.Range(-1f, 1f)).normalized;
        rb.linearVelocity = randomDirection * initialSpeed;
    }

    private IEnumerator LaunchBallAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!gameOver) LaunchBall();
    }

    public void StopGame() 
    {
        gameOver = true;
        rb.linearVelocity = Vector2.zero;
    }
}

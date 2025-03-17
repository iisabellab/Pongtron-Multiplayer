using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ScoreCount : NetworkBehaviour
{
    private NetworkVariable<int> playerScore1 = new NetworkVariable<int>();
    private NetworkVariable<int> playerScore2 = new NetworkVariable<int>();

    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI winMessageText; 

    private AudioSource goalAudio; 
    private AudioSource winAudio;  

    private void Start()
    {
        playerScore1.OnValueChanged += UpdatePlayer1Score;
        playerScore2.OnValueChanged += UpdatePlayer2Score;

        // get AudioSources
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            goalAudio = audioSources[0];
            winAudio = audioSources[1];
        }

        // win message is hidden at start
        if (winMessageText != null)
        {
            winMessageText.gameObject.SetActive(false);
        }
    }

    public void Player1Goal()
    {
        if (IsServer)
        {
            playerScore1.Value++;

            // Play goal sound
            if (goalAudio != null && goalAudio.clip != null)
            {
                goalAudio.Play();
            }

            if (playerScore1.Value >= 11)
            {
                AnnounceWinnerClientRpc("Player 1 Wins!");
                FindFirstObjectByType<BallController>()?.StopGame(); // Stop ball movement

            }
        }
    }

    public void Player2Goal()
    {
        if (IsServer)
        {
            playerScore2.Value++;

            // Play goal sound
            if (goalAudio != null && goalAudio.clip != null)
            {
                goalAudio.Play();
            }

            if (playerScore2.Value >= 11)
            {
                AnnounceWinnerClientRpc("Player 2 Wins!");
                FindFirstObjectByType<BallController>()?.StopGame();
            }
        }
    }

    private void UpdatePlayer1Score(int oldValue, int newValue)
    {
        if (player1ScoreText != null)
        {
            player1ScoreText.text = newValue.ToString();
        }
    }

    private void UpdatePlayer2Score(int oldValue, int newValue)
    {
        if (player2ScoreText != null)
        {
            player2ScoreText.text = newValue.ToString();
        }
    }

    [ClientRpc]
    private void AnnounceWinnerClientRpc(string winnerMessage)
    {
        if (winMessageText != null)
        {
            winMessageText.text = winnerMessage;
            winMessageText.gameObject.SetActive(true);
        }

        // Play win sound
        if (winAudio != null && winAudio.clip != null)
        {
            winAudio.Play();
        }
    }
}

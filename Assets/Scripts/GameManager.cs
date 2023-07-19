///
/// The game manager keeps track of the game. Things such as the score, if the level is complete, etc
///

using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public PacmanAgent agent;

    public int score { get; private set; }
    public int lives { get; private set; }
    public int ghostMultiplier { get; private set; } = 1; // If a ghost is eaten, how much more is the next ghost worth

    public int winScore = 100; // Score added when the game is won
    public int loseScore = -100; // Score added when pacman dies, NOTE it is added so it must be negative
    public int pelletScore = 1;

    public float maxResetTime = 30f; // max time in seconds if no pellets are eaten after which the game is reset

    private float timeSinceLastPelletEaten = 0f; // time since the last pellet was eaten
    private int iteration = 0;
    private int wins = 0;

    public int start_lives = 1; // default number of lives
    public bool enableGhost = true; // start with ghosts or not, for testing/training purposes

    public TextMeshProUGUI text_score;
    public TextMeshProUGUI text_lives;
    public TextMeshProUGUI text_iterations;
    public TextMeshProUGUI text_rounds;
    public TextMeshProUGUI text_wins;
 


    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        // increment the time since the last pellet was eaten
        this.timeSinceLastPelletEaten += Time.deltaTime;

        // check threshold exceeded or not
        if (timeSinceLastPelletEaten >= maxResetTime)
        {
            this.PacmanEaten(); // call pacman eaten, this will reset the game also
        }
    }

    private void NewGame()
    {
        this.agent.EndEpisode(); // End the agent episode
        
        iteration++;

        this.text_iterations.text = "Iteration: " + iteration;
        this.text_rounds.text = "Rounds: " + iteration;

        SetScore(0);
        SetLives(this.start_lives);
        NewRound();
    }

    private void NewRound()
    {
        // Re-enable all pelets
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        ResetGhostMultiplier();

        // enable all ghost
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
            this.ghosts[i].gameObject.SetActive(this.enableGhost);
        }

        // enable pacman
        this.pacman.ResetState();

        // Reset timer
        timeSinceLastPelletEaten = 0f;
    }

    private void GameOver()
    {
        // TODO Change for ML agents
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);

        // tank the score
        SetScore(this.score + this.loseScore);

        // new game TODO Above code is not needed in the case of ML
        NewGame();
    }

    private void SetScore(int score)
    {
        this.score = score;
        text_score.text = "Score: " + this.score;

        this.agent.SetReward(this.score); // Reward or penalize the agent
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        text_lives.text = "Lives: " + this.lives;
    }

    public void GhostEaten(Ghost ghost)
    {
        SetScore(this.score + ghost.points * this.ghostMultiplier);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        this.pacman.gameObject.SetActive(false);

        SetLives(this.lives - 1);

        if (this.lives > 0)
        {
            ResetState(); // Don't reset pellets, only pacman and ghosts
        } else
        {
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        // Reset timer 
        timeSinceLastPelletEaten = 0f;


        // disable pellet so you can't eat it again and its not visible
        pellet.gameObject.SetActive(false);

        // increase score
        SetScore(this.score + this.pelletScore);

        if (!HasRemainingPellets())
        {
            wins++;
            text_wins.text = "Wins: " + wins;

            // game is finished, all pellets are eaten
            // disable pacman so you cannot be eaten
            this.pacman.gameObject.SetActive(false);

            // Increase score
            SetScore(this.score + this.winScore);

            NewGame();
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
       // change all ghosts states to frightened
       for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }



        PelletEaten(pellet);

        // If a power pellet is eaten whilst still within the duration, we want to reset the time so we cancel invoke first
        // And then we redo -> will stack ghost multiplier aswell
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);


    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in this.pellets)
        {
            // is a pellet still active?
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }
}

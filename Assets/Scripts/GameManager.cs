///
/// The game manager keeps track of the game. Things such as the score, if the level is complete, etc
///

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public int score { get; private set; }
    public int lives { get; private set; }
    public int ghostMultiplier { get; private set; } = 1; // If a ghost is eaten, how much more is the next ghost worth
    public int winScore { get; private set; } = 10000; // Score added when the game is won

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        Debug.Log(this.score.ToString());
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(1); // I only use a single life as its about the AI not the OG game, however could add multiple lives later
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
            this.ghosts[i].gameObject.SetActive(true);
        }

        // enable pacman
        this.pacman.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        // TODO Change for ML agents
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);

        // new game TODO Above code is not needed in the case of ML
        NewGame();
    }

    private void SetScore(int score)
    {
        this.score = score;
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
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
        // disable pellet so you can't eat it again and its not visible
        pellet.gameObject.SetActive(false);

        // increase score
        SetScore(this.score + pellet.points);

        if (!HasRemainingPellets())
        {
            // game is finished, all pellets are eaten
            // disable pacman so you cannot be eaten
            this.pacman.gameObject.SetActive(false);

            // Increase score
            SetScore(this.score + this.winScore);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        // TODO change ghost state
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

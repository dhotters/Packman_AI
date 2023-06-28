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

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {

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
        SetScore(this.score + ghost.points);
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
}

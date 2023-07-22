///
/// The game manager keeps track of the game. Things such as the score, if the level is complete, etc
///

using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public PacmanAgent agent;

    public float score { get; private set; }
    public int lives { get; private set; }
    public int ghostMultiplier { get; private set; } = 1; // If a ghost is eaten, how much more is the next ghost worth

    public int remainingPacdots { get; private set; }

    public float winScore = 2f; // Score added when the game is won
    public float loseScore = -2f; // Score added when pacman dies, NOTE it is added so it must be negative
    public float loseLiveScore = -0.3f;
    public float pelletScore = 0.005f;
    public float stuckScore = -1f;

    public float maxResetTime = 30f; // max time in seconds if no pellets are eaten after which the game is reset
    public float maxStuckTime = 1f; // how many seconds can pacman stay still untill being punished

    private float timeSinceLastPelletEaten = 0f; // time since the last pellet was eaten
    private float timeSinceStill = 0f;
    private int current_iteration = 0;
    private int wins = 0;

    public bool enablePowerPellets = true;

    [Range(1, 244)] public int numPellets = 244; // 244 in total

    public int start_lives = 1; // default number of lives

    public TextMeshProUGUI text_score;
    public TextMeshProUGUI text_lives;
    public TextMeshProUGUI text_iterations;
    public TextMeshProUGUI text_rounds;
    public TextMeshProUGUI text_wins;

    private Vector3 lastPos;
 


    private void Start()
    {
        lastPos = pacman.transform.position;
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

        // check if pacman is in the same location as before
        if (pacman.transform.position == lastPos)
        {
            timeSinceStill += Time.deltaTime; // add to time
        }

        if (timeSinceStill >= maxStuckTime)
        {
            agent.AddReward(stuckScore);
            SetScore(this.score + stuckScore);
            timeSinceStill = 0; // also reset timer
        }

        lastPos = pacman.transform.position;
    }

    private void NewGame()
    {
        this.agent.EndEpisode(); // End the agent episode

        remainingPacdots = numPellets;
        
        current_iteration++;

        this.text_iterations.text = "Iteration: " + agent.CompletedEpisodes;
        this.text_rounds.text = "Rounds: " + current_iteration;

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

        // Create a list of random numbers
        List<int> randomList = new List<int>();
        for (int i = 0; i < 244-numPellets; i++)
        {

            // get a random number
            int num = UnityEngine.Random.Range(0, 244);

            while (randomList.Contains(num))
            {
                // if the list contains the number, generate another number untill it doesnt
                num = UnityEngine.Random.Range(0, 244);
            }

            // add the number to the list
            randomList.Add(num);
        }
        
        // Enable the correct pellets and make sure they are in the correct layer
        for (int i = 0; i < 244; i++)
        {
            Transform p = this.pellets.GetChild(i); // current pellet

            // check if this index is in the list of random numbers
            if (randomList.Contains(i))
            {
                // disable the pellet
                p.gameObject.SetActive(false);

                // move to other pellet layer
                p.gameObject.layer = LayerMask.NameToLayer("UnusedPellet");
            } else
            {
                // if the current pellet should be enabled
                // enable the pellet
                p.gameObject.SetActive(true);

                // add to correct layer
                p.gameObject.layer = LayerMask.NameToLayer("Pellet");
            }
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


            // enable ghost but only if it should be enabled
            ghosts[i].gameObject.SetActive(ghosts[i].enable);
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
        agent.AddReward(this.loseScore);

        // new game TODO Above code is not needed in the case of ML
        NewGame();
    }

    private void SetScore(float score)
    {
        this.score = score;
        text_score.text = "Score: " + Math.Round(this.score, 2);

        //this.agent.SetReward(this.score); // Reward or penalize the agent
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        text_lives.text = "Lives: " + this.lives;
    }

    public void GhostEaten(Ghost ghost)
    {
        SetScore(this.score + ghost.points * this.ghostMultiplier);
        this.agent.AddReward(ghost.points * this.ghostMultiplier);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        this.pacman.gameObject.SetActive(false);

        SetLives(this.lives - 1);

        if (this.lives > 0)
        {
            ResetState(); // Don't reset pellets, only pacman and ghosts
            SetScore(this.score + this.loseLiveScore);
            agent.AddReward(this.loseLiveScore);
        } else
        {
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        // Reset timer 
        timeSinceLastPelletEaten = 0f;

        // subtract from total
        remainingPacdots -= 1;


        // disable pellet so you can't eat it again and its not visible
        pellet.gameObject.SetActive(false);

        // increase score
        SetScore(this.score + this.pelletScore);
        agent.AddReward(this.pelletScore);

        if (!HasRemainingPellets())
        {
            wins++;
            text_wins.text = "Wins: " + wins;

            // game is finished, all pellets are eaten
            // disable pacman so you cannot be eaten
            this.pacman.gameObject.SetActive(false);

            // Increase score
            SetScore(this.score + this.winScore);
            agent.AddReward(this.winScore);

            NewGame();
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        // Power pelleteaten only uses intended behaviour if enablePowerPellets is true else it runs pelleteaten

       // change all ghosts states to frightened
       if (enablePowerPellets)
        {
            for (int i = 0; i < this.ghosts.Length; i++)
            {
                this.ghosts[i].frightened.Enable(pellet.duration);
            }
        }
       



        PelletEaten(pellet);

        // If a power pellet is eaten whilst still within the duration, we want to reset the time so we cancel invoke first
        // And then we redo -> will stack ghost multiplier aswell
        if (enablePowerPellets)
        {
            CancelInvoke();
            Invoke(nameof(ResetGhostMultiplier), pellet.duration);
        }
        


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

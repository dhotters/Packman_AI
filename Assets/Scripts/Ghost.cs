using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Movement movement { get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }

    public GhostBehavior initialBehavior; // Not all ghost start with the same behavior, this is the initial

    public Transform target; // the target of what we are chasing, will be pacman
    public int points = 200; // How many points are added when a ghost is eaten

    public Type ghostType = new Type();

    public enum Type
    {
        Blinky,
        Inky,
        Pinky,
        Clyde
    }

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<GhostHome>();
        this.scatter = GetComponent<GhostScatter>();
        this.chase = GetComponent<GhostChase>();
        this.frightened = GetComponent<GhostFrightened>();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();

        // initial state of all ghosts, always start with scatter
        this.frightened.Disable();
        this.chase.Disable();
        this.scatter.Enable();
        
        if (this.home != this.initialBehavior)
        {
            this.home.Disable();
        }

        if (this.initialBehavior != null)
        {
            this.initialBehavior.Enable();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if ghost collides with pacman
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            // if the ghost is frightened then the ghost is eaten, else pacman is eaten
            if (this.frightened.enabled)
            {
                FindObjectOfType<GameManager>().GhostEaten(this);
            } else
            {
                FindObjectOfType<GameManager>().PacmanEaten();
            }
        }
    }
}

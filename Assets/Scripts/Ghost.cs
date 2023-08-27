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
    public Transform scatterTarget;
    public int points = 0.2; // How many points are added when a ghost is eaten

    public bool enable = true;

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

    private void OnDrawGizmos()
    {
        switch (ghostType)
        {
            case Ghost.Type.Blinky:
                Gizmos.color = Color.red;
                break;
            case Ghost.Type.Inky:
                Gizmos.color = Color.cyan;
                break;
            case Ghost.Type.Pinky:
                Gizmos.color = new Color(255, 192, 203);
                break;
            case Ghost.Type.Clyde:
                Gizmos.color = new Color(255, 165, 0);
                break;
        }
        
        Gizmos.DrawSphere(target.position, 0.5f);
    }

    public void ResetState()
    {
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

using UnityEngine;

public class GhostFrightened : GhostBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    public bool eaten { get; private set; }

    // override enable to make sure ghost is in correct state incase a power pellet is eaten whilst one was active for example
    public override void Enable(float duration)
    {
        base.Enable(duration);

        // he becomes blue
        this.body.enabled = false;
        this.eyes.enabled = false;
        this.blue.enabled = true;
        this.white.enabled = false;

        // after half the duration invoke flash
        Invoke(nameof(Flash), duration / 2.0f);
    }

    public override void Disable()
    {
        base.Disable();

        // make the ghost normal again
        this.body.enabled = true;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;
    }

    private void Flash()
    {
        if (!this.eaten)
        {
            this.blue.enabled = false;
            this.white.enabled = true;
            this.white.GetComponent<AnimatedSprite>().Restart();
        }
    }

    private void Eaten()
    {
        // ghost gets eaten
        this.eaten = true;

        // Set the ghost position back at its home
        Vector3 position = this.ghost.home.inside.position;
        position.z = this.ghost.transform.position.z;
        this.ghost.transform.position = position;

        // enable home behavior, note the duration required such that ghost does not leave home
        // before frightened state is over
        this.ghost.home.Enable(this.duration);

        // correct sprites
        this.body.enabled = false;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;
    }

    private void OnEnable()
    {
        // if frightened you get half speed
        this.ghost.movement.speedMultiplier = 0.5f;
        this.eaten = false;
    }

    private void OnDisable()
    {
        this.ghost.movement.speedMultiplier = 1.0f;
        this.eaten = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if ghost collides with pacman
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {

            if (this.enabled)
            {
                Eaten();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // movement

        Node node = other.GetComponent<Node>();

        if (node != null && enabled)
        {
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue;

            // we move into the direction that maximizes the distance between ghost and pacman
            foreach (Vector2 availableDirection in node.availableDirections)
            {
                // new pos if we would move in current availabledirection
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);

                // compute distance
                float distance = (ghost.target.position - newPosition).sqrMagnitude; // sqrMagnitude because its faster which is important for ML

                // find direction which maximizes distance
                if (distance > maxDistance)
                {
                    direction = availableDirection;
                    maxDistance = distance;
                }
            }

            this.ghost.movement.SetDirection(direction);
        }
    }
}

using UnityEngine;

public class GhostChase : GhostBehavior
{
    private void OnDisable()
    {
        // after stopping chase, enter scatter mode
        this.ghost.scatter.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Get the ghost type
        Ghost.Type type = ghost.ghostType;

        // perform behavior based on type
        switch(type)
        {
            case Ghost.Type.Blinky:
                BlinkyChase(other);
                break;
            case Ghost.Type.Inky:
                InkyChase(other);
                break;
            case Ghost.Type.Pinky:
                PinkyChase(other);
                break;
            case Ghost.Type.Clyde:
                ClydeChase(other);
                break;

        }
    }

    private void BlinkyChase(Collider2D other)
    {
        // Get the node
        Node node = other.GetComponent<Node>();

        if (node != null && enabled && !ghost.frightened.enabled)
        {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            // we move into the direction that minimizes the distance between ghost and pacman
            foreach (Vector2 availableDirection in node.availableDirections)
            {
                // new pos if we would move in current availabledirection
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);

                // compute distance
                float distance = (ghost.target.position - newPosition).sqrMagnitude; // sqrMagnitude because its faster which is important for ML

                // find direction which minizes distance
                if (distance < minDistance)
                {
                    direction = availableDirection;
                    minDistance = distance;
                }
            }

            this.ghost.movement.SetDirection(direction);
        }
    }

    private void InkyChase(Collider2D other)
    {
        // Get the node
        Node node = other.GetComponent<Node>();

        if (node != null && enabled && !ghost.frightened.enabled)
        {
            // TODO

        }
    }

    private void PinkyChase(Collider2D other)
    {
        // Get the node
        Node node = other.GetComponent<Node>();

        if (node != null && enabled && !ghost.frightened.enabled)
        {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            // we move into the direction that minimizes the distance between ghost and pacman
            foreach (Vector2 availableDirection in node.availableDirections)
            {
                // new pos if we would move in current availabledirection
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);

                // For pinky the target is 2 pellets infront of pacman
                Vector3 pelletDir = ghost.target.GetComponent<Movement>().direction * 2.0f;
                Vector3 targetPos = ghost.target.position + pelletDir;

                // compute distance
                float distance = (targetPos - newPosition).sqrMagnitude; // sqrMagnitude because its faster which is important for ML

                // find direction which minizes distance
                if (distance < minDistance)
                {
                    direction = availableDirection;
                    minDistance = distance;
                }
            }

            this.ghost.movement.SetDirection(direction);
        }
    }

    private void ClydeChase(Collider2D other)
    {

    }
}

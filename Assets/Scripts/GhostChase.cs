using UnityEngine;

public class GhostChase : GhostBehavior
{

    public Transform transformBlinky;

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
        ///
        /// Blinky chases pacman by simply targeting him and taking the shortest path
        ///

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
        ///
        /// Inky's target is relative to both blinky and pacman
        /// The distance blinky is from pinky's target is doubled to get inky's target
        ///

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

                // For pinky the target is 4 pellets infront of pacman
                Vector3 pelletDir = ghost.target.GetComponent<Movement>().direction * 2.0f;
                Vector3 target1 = ghost.target.position + pelletDir; // target 2 pellets infron of pacman

                Vector3 targetPos = (target1 - transformBlinky.position) * 2; // Vector from blinky to the target, size doubled

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

    private void PinkyChase(Collider2D other)
    {
        ///
        /// Pinky chases after packman by targeting 4 pellets infront of pacman
        ///

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

                // For pinky the target is 4 pellets infront of pacman
                Vector3 pelletDir = ghost.target.GetComponent<Movement>().direction * 4.0f;
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
        ///
        /// Clyde chases pacman by doing the same as blinky, however clyde tries to head to his
        /// scatter corner (lower left) when he is within 8-dot radius of pacman
        ///

        // TODO
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

                // get distance from pacman
                float distance_pacman = (ghost.target.position - ghost.transform.position).sqrMagnitude;

                // if not within 8 tiles, do normal blinky
                // NOTE 64 because sqrMagnitude
                Vector3 tgt;
                if (distance_pacman >= 64)
                {
                    tgt = ghost.target.position;
                } else
                {
                    // if within 8 tiles, go to scatter target
                    tgt = ghost.scatterTarget.position;
                }

                // compute distance
                float distance = (tgt - newPosition).sqrMagnitude; // sqrMagnitude because its faster which is important for ML

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
}

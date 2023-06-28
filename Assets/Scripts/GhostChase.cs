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
}

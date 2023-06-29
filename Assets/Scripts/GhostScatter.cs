using UnityEngine;

public class GhostScatter : GhostBehavior
{

    private void OnDisable()
    {
        // after stopping scattering, enter chase mode
        this.ghost.chase.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Node node = other.GetComponent<Node>();

        //// NOTE frightened will overwrite this movement hence the check
        //if (node != null && this.enabled && !this.ghost.frightened.enabled)
        //{
        //    // Go into random direction
        //    int index = Random.Range(0, node.availableDirections.Count);

        //    // check that the ghost does not go back to the previous node
        //    if (node.availableDirections[index] == -this.ghost.movement.direction && node.availableDirections.Count > 1)
        //    {
        //        // if it does go back, negate it unless its the only direction possible
        //        index++; // simply take the next index instead of taking a random one again

        //        // correct for overflow
        //        if (index >= node.availableDirections.Count)
        //        {
        //            index = 0;
        //        }
        //    }

        //    this.ghost.movement.SetDirection(node.availableDirections[index]);
        //}

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
                float distance = (ghost.scatterTarget.position - newPosition).sqrMagnitude; // sqrMagnitude because its faster which is important for ML

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

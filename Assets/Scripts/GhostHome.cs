using System.Collections;
using UnityEngine;

public class GhostHome : GhostBehavior
{
    public Transform inside;
    public Transform outside;

    private void OnDisable()
    {
        StartCoroutine(ExitTransition());
    }

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private IEnumerator ExitTransition()
    {
        this.ghost.movement.SetDirection(Vector2.up, true);
        this.ghost.movement.rigidbody.isKinematic = true;
        this.ghost.movement.enabled = false;


        Vector3 position = this.transform.position;

        float duration = 0.5f; // time between transitions
        float elapsed = 0.0f;

        // from start pos to inside pos
        while (elapsed < duration)
        {
            // linearly interpolate between prev position and new position
            Vector3 newPosition = Vector3.Lerp(position, this.inside.position, elapsed / duration);
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0.0f;

        // from inside pos to outside pos
        while (elapsed < duration)
        {
            // linearly interpolate between prev position and new position
            Vector3 newPosition = Vector3.Lerp(position, this.outside.position, elapsed / duration);
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }


        this.ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f), true); // random direction
        this.ghost.movement.rigidbody.isKinematic = false;
        this.ghost.movement.enabled = true;
    }
}

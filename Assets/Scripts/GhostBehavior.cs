using UnityEngine;

[RequireComponent(typeof(Ghost))]
public abstract class GhostBehavior : MonoBehaviour
{
    public Ghost ghost { get; private set; }
    public float duration;

    private void Awake()
    {
        this.ghost = GetComponent<Ghost>();
        this.enabled = false;
    }

    // Enable functions to allow for different durations, if none is specified use the default
    public void Enable()
    {
        Enable(this.duration);
    }

    public virtual void Enable(float duration)
    {
        this.enabled = true;

        // If a power pellet is eaten once it was already enabled, we want to reset the duration hence the cancel invoke
        CancelInvoke();
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        enabled = false;
        CancelInvoke();
    }
}

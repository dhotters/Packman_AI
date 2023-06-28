using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 8.0f; // duration of the power pellet state

    protected override void Eat()
    {
        FindObjectOfType<GameManager>().PowerPelletEaten(this);
    }
}

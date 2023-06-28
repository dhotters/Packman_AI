using UnityEngine;

public class Pellet : MonoBehaviour
{
    public int points = 10; // how many points is a pellet worth

    protected virtual void Eat()
    {
        FindObjectOfType<GameManager>().PelletEaten(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if the object is pacman, eat the pellet
        if (other.gameObject.layer == LayerMask.NameToLayer("Pacman")) 
        {
            Eat();
        }
    }
}

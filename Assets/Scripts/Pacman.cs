using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    public Movement movement { get; private set; }

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
    }

    //private void Update()
    //{

    //    // get current direction
    //    Vector2 current_dir = this.movement.direction;

    //    // set new direction, defaulted to the current dir
    //    Vector2 new_dir;

    //    // Movement using arrow keys or WASD
    //    // This movement is RELATIVE ie pressing left rotates character to its left
    //    // Note set new dir inside the if statement such that the queue works properly
    //    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
    //    {
    //        //this.movement.SetDirection(Vector2.up);
    //        new_dir = current_dir;
    //        this.movement.SetDirection(new_dir);

    //    } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
    //    {
    //        //this.movement.SetDirection(Vector2.down);
    //        new_dir = -current_dir;
    //        this.movement.SetDirection(new_dir);
    //    }
    //    else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        //this.movement.SetDirection(Vector2.left);
    //        new_dir = new Vector2(-current_dir.y, current_dir.x);
    //        this.movement.SetDirection(new_dir);
    //    } 
    //    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
    //    {
    //        //this.movement.SetDirection(Vector2.right);
    //        new_dir = new Vector2(current_dir.y, -current_dir.x);
    //        this.movement.SetDirection(new_dir);
    //    }

    //    // Rotate pacman
    //    float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x);
    //    this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    //}

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();
    }
}

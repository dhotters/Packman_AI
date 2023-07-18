using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

/// <summary>
/// This script contains the pacman agent itself and feeds inputs to pacman
/// It fills in all observations, makes all decisions, performs all actions and receives rewards
/// </summary>

public class PacmanAgent : Agent
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public Movement movement;

    public void Start()
    {
        this.movement = this.pacman.movement;
    }

    // Collect the observations (how the agent interacts with environment)
    public override void CollectObservations(VectorSensor sensor)
    {
        // Add pacman position, cast to vector2
        sensor.AddObservation((Vector2)this.pacman.transform.position);

        // Add pellets positions, cast to vector2
        foreach (Transform pellet in this.pellets)
        {
            sensor.AddObservation((Vector2)pellet.position);
        }
    }

    // When an action is received (decision from agent or heuristic)
    public override void OnActionReceived(ActionBuffers actions)
    {
        // get action
        int moveDir = actions.DiscreteActions[0];

        /// <summary>
        /// We define the following actions:
        /// 0 - No change, keep moving in the current direction
        /// 1 - Go backward ie reverse
        /// 2 - Turn left
        /// 3 - Turn right
        /// </summary>


        // get current direction
        Vector2 current_dir = this.movement.direction;

        // define new direction
        Vector2 new_dir;

        // apply the new direction to pacman, NOTE that the setdirection is called inside the if statement to make the movement queue function properly
        if (moveDir == 0)
        {
            new_dir = current_dir;
            this.movement.SetDirection(new_dir);
        }
        else if (moveDir == 1)
        {
            new_dir = -current_dir;
            this.movement.SetDirection(new_dir);
        }
        else if (moveDir == 2)
        {
            new_dir = new Vector2(-current_dir.y, current_dir.x);
            this.movement.SetDirection(new_dir);
        }
        else if (moveDir == 3)
        {
            new_dir = new Vector2(current_dir.y, -current_dir.x);
            this.movement.SetDirection(new_dir);
        }

        // Rotate pacman
        float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x);
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }
}

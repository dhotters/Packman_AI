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
            //this.movement.SetDirection(new_dir);
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
        this.pacman.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    // User takes over control
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // get the discrete actions to modify them
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        /// <summary>
        /// We define the following actions:
        /// 0 - No change, keep moving in the current direction
        /// 1 - Go backward ie reverse
        /// 2 - Turn left
        /// 3 - Turn right
        /// </summary>

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = 0;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActions[0] = 2;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            discreteActions[0] = 3;
        }

        Debug.Log(discreteActions[0]);
    }
}

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
    private Movement movement;
    public GameManager manager;

    public bool relativeMovement = true; // use relative movement or absolute?


    // params for normalization
    private float minX = float.MaxValue;
    private float minY = float.MaxValue;

    private float maxX = float.MinValue;
    private float maxY = float.MinValue;

    public void Start()
    {
        this.movement = this.pacman.movement;


        // Get min x, y and max x, y values to normalize the position vectors of the agent
        foreach (Transform p in pellets)
        {
            if (p.position.x < minX)
            {
                minX = p.position.x;
            }

            if (p.position.y < minY)
            {
                minY = p.position.y;
            }

            if (p.position.x > maxX)
            {
                maxX = p.position.x;
            }

            if (p.position.y > maxY)
            {
                maxY = p.position.y;
            }
        }
    }

    // Collect the observations (how the agent interacts with environment)
    // Normalize the inputs
    public override void CollectObservations(VectorSensor sensor)
    {
        // Add pacman position, cast to vector2, normalized
        sensor.AddObservation(NormalizeVec2((Vector2)this.pacman.transform.position));

        // Add ghost positions and direction
        foreach (Ghost ghost in ghosts)
        {
            if (ghost.gameObject.activeSelf)
            {
                sensor.AddObservation(NormalizeVec2((Vector2)ghost.transform.position));
                sensor.AddObservation(ghost.movement.direction);
                //sensor.AddObservation(Vector2.zero);
                //sensor.AddObservation(Vector2.zero);
            } else
            {
                sensor.AddObservation(Vector2.zero);
                sensor.AddObservation(Vector2.zero);
            }
        }

        // also add raycast hit information
        Vector2[] directions;
        if (relativeMovement) 
        {
            // if moving relative input observations relatively aswell ie forward backward left and right to packman
            // this way we stay consistent
            Vector2 forward = movement.direction;
            Vector2 backward = -movement.direction;

            Vector2 left = new Vector2(-forward.y, forward.x);
            Vector2 right = new Vector2(forward.y, -forward.x);

            directions = new Vector2[] { forward, backward, right, left };
        } 
        else
        {
            directions = new Vector2[] { Vector2.up, Vector2.down, Vector2.right, Vector2.left };
        }

        //IList<float> occupiedDirections = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f };

        for (int i = 0; i < 4; i++)
        {
            bool occupied = movement.Occupied(directions[i]);
            sensor.AddObservation(occupied);
            //occupiedDirections[i] = occupied ? 0.0f : 1.0f;
        }

        //sensor.AddObservation(occupiedDirections);

        // amount of pacdots left, normalized
        sensor.AddObservation(manager.remainingPacdots/manager.pellets.childCount);

        // distance to closest pellet
        //sensor.AddObservation(closestPelletDistance());

        // closest pellet coordinates
        sensor.AddObservation(closestPellet());

        // closest power pellet coordinates
        sensor.AddObservation(closestPowerPellet());

        // distance to closest ghost
        //sensor.AddObservation(closestGhostDistance());
        //sensor.AddObservation(100);

        // Add pellets positions, cast to vector2
        //foreach (Transform pellet in this.pellets)
        //{
        //    if (pellet.gameObject.GetComponent<PowerPellet>() == null)
        //    {
        //        // check if the pellet is active
        //        if (pellet.gameObject.activeSelf && pellet.gameObject.layer == LayerMask.NameToLayer("Pellet"))
        //        {
        //            // if the pellet is active and in the pellet layer, then add it to the observation
        //            sensor.AddObservation(NormalizeVec2((Vector2)pellet.position));
        //        }
        //        else if (!pellet.gameObject.activeSelf && pellet.gameObject.layer == LayerMask.NameToLayer("Pellet"))
        //        {
        //            // if the pellet is not active but is inside the pellet layer then add zero vector
        //            sensor.AddObservation(Vector2.zero);
        //        }
        //        // if its not in the pellet layer, ignore
        //    }
        //    // if its a power pellet ignore
        //}

        // Add power pellet locations

        //foreach (Transform pellet in this.pellets)
        //{
        //    if (pellet.gameObject.GetComponent<PowerPellet>() != null && pellet.gameObject.activeSelf)
        //    {
        //        sensor.AddObservation(NormalizeVec2((Vector2)pellet.position));
        //    }
        //    else if (pellet.gameObject.GetComponent<PowerPellet>() != null && !pellet.gameObject.activeSelf)
        //    {
        //        sensor.AddObservation(Vector2.zero);
        //    }
        //}
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

        if (this.relativeMovement) { 
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
        } else
        {
            if (moveDir == 0)
            {
                this.movement.SetDirection(Vector2.up);
            }
            else if (moveDir == 1)
            {
                this.movement.SetDirection(Vector2.down);
            }
            else if (moveDir == 2)
            {
                this.movement.SetDirection(Vector2.left);
            }
            else if (moveDir == 3)
            {
                this.movement.SetDirection(Vector2.right);
            }
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
        else
        {
            discreteActions[0] = 4;
        }
    }

    private float closestPelletDistance(bool normalize = true)
    {
        float minDist = float.MaxValue;
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                float dist = Vector2.Distance((Vector2)pacman.transform.position, (Vector2)pellet.position);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }
        }

        // NOTE This normalization assumes a map that is rectangular
        if (normalize)
        {
            float dx2 = Mathf.Pow(maxX - minX, 2);
            float dy2 = Mathf.Pow(maxY - minY, 2);
            float maxDiagonalDist = Mathf.Sqrt(dx2 + dy2);

            minDist = minDist / maxDiagonalDist;
        }

        return minDist;
    }

    private Vector2 closestPellet(bool normalize = true)
    {
        Vector2 closest = Vector2.zero;
        float minDist = float.MaxValue;
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                float dist = Vector2.Distance((Vector2)pacman.transform.position, (Vector2)pellet.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = (Vector2)pellet.position;
                }
            }
        }
        return NormalizeVec2(closest);
    }

    private Vector2 closestPowerPellet(bool normalize = true)
    {
        Vector2 closest = Vector2.zero;
        float minDist = float.MaxValue;
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf && pellet.gameObject.GetComponent<PowerPellet>() != null)
            {
                float dist = Vector2.Distance((Vector2)pacman.transform.position, (Vector2)pellet.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = (Vector2)pellet.position;
                }
            }
        }
        return NormalizeVec2(closest);
    }

    private float closestGhostDistance()
    {
        float minDist = 1000;
        foreach (Ghost ghost in ghosts)
        {
            if (ghost.gameObject.activeSelf)
            {
                float dist = Vector2.Distance((Vector2)pacman.transform.position, (Vector2)ghost.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }
        }
        return minDist;
    }

    private Vector2 NormalizeVec2(Vector2 vector)
    {
        float normX = (vector.x - minX) / (maxX - minX);
        float normY = (vector.y - minY) / (maxY - minY);

        return new Vector2(normX, normY);
    }
}

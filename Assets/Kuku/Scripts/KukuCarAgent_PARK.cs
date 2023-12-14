using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using UnityStandardAssets.Vehicles.Car;
using static UnityEngine.GraphicsBuffer;

public class KukuCarAgent : Agent
{
    #region RewardConfig
    private float distanceScalingFactor = 0.01f; // Scaling factor to adjust reward sensitivity
    private float maxReward = 1.0f; // Maximum reward when closest
    private float minReward = 0.0f; // Minimum reward when farthest
    public float alignmentScalingFactor = 0.02f; // Adjust as needed
    public float positioningScalingFactor = 0.01f; // Adjust as needed
    public float maxAllowedDistance = 1f;
    #endregion

    #region RewardFunctions

    private float calculateDistanceBasedRewardCloser()
    {
        float reward = 0;
        float distance = Vector3.Distance(target.transform.position, transform.position);

        // Reward for staying in the parking spot
        if (isInSpot)
        {
            reward = maxReward * 5f;
            Debug.Log("In Parking spot so max reward : " + reward);
        }
        else if (distance < closestDistance)
        {
            // Reward for getting closer to the target
            closestDistance = distance;
            reward = maxReward - distance * distanceScalingFactor; //Always positive
            Debug.Log("Reward for new close distance : " + reward);

        }
        /*
         * else if (!isInSpot)
        {
            // Penalize for not getting closer when not in the spot
            reward = (closestDistance - distance) * distanceScalingFactor; //Always negative
            reward = Math.Clamp(reward, -100f, 0f);
            Debug.Log("Not closer distance and not in spot so penalty : " + reward);
        }
         */

        return reward;
    }

    private float calculateStillInSamePosReward(float tolerance = 0.0001f)
    {
        if (isInSpot) return 100.0f;
        // Calculate the distance between the two positions using magnitude.
        float distance = (lastPos - transform.position).magnitude;

        lastPos = transform.position;

        // Check if the distance is within the specified tolerance.
        if (distance < tolerance)
        {
            Debug.LogWarning("Penalized for being close to last position");
            return -1f;

        }

        // Return 0 when they are not close.
        return 0.0f;
    }

    private int calculateActionsTakenReward()
    {
        int maxActions = MaxStep;
        int actionsReward =maxActions - actionsTaken;
        return actionsReward * 5;
    }

    private float CalculateParkingReward()
    {
        if (Vector3.Distance(target.transform.position, transform.position) < 3)
        {
            float alignmentReward = CalculateAlignmentReward();
            float positioningReward = CalculatePositioningReward();

            // Combine alignment and positioning rewards
            float reward = maxReward + alignmentReward + positioningReward;

            // Ensure the reward doesn't go below the minimum reward
            reward = Mathf.Max(reward, minReward);

            return reward;
        }

        // If not in the spot, return a default reward
        return 0.0f;
    }

    #endregion

    #region InternalRewardFunctions


    float CalculateAlignmentReward()
    {
        // Calculate alignment reward based on the cosine of the angle between car's forward and spot's forward
        float forwardAlignment = Vector3.Dot(transform.forward, target.transform.forward);
        float backwardAlignment = Vector3.Dot(-transform.forward, target.transform.forward);
        float alignmentAngle = Mathf.Max(forwardAlignment, backwardAlignment); // Use the larger alignment

        // Calculate the alignment reward
        return alignmentScalingFactor * alignmentAngle;
    }

    private float CalculatePositioningReward()
    {
        // Calculate positioning reward based on the distance to the spot's center
        Vector3 relativeLocation = target.transform.position - transform.position;
        return positioningScalingFactor * (1 - relativeLocation.magnitude / maxAllowedDistance);
    }

    private bool IsCloseToPerfectPosition()
    {
        // Check alignment with the parking spot's orientation
        float angleThreshold = perfectParkingAngleThreshold; // Adjust as needed
        // Calculate forward alignment
        float forwardAlignment = Vector3.Angle(transform.forward, target.transform.forward);
        // Calculate backward alignment
        float backwardAlignment = Vector3.Angle(-transform.forward, target.transform.forward);
        // Use the smaller alignment angle between forward and backward
        float alignmentAngle = Mathf.Min(forwardAlignment, backwardAlignment);

        // Check the distance to the center of the parking spot
        float maxDistanceToCenter = perfectParkingDistanceFromCenterThreshold; // Adjust as needed
        Vector3 relativeLocation = target.transform.position - transform.position;
        float distanceToCenter = relativeLocation.magnitude;

        // Check if both alignment and distance criteria are met
        bool isCloseToPerfect = alignmentAngle < angleThreshold && distanceToCenter < maxDistanceToCenter;

        return isCloseToPerfect;

    }
    #endregion

    #region Episodes
    public override void OnEpisodeBegin()
    { 
        var envScript = GetComponent<EnvScriptAbstract>();
        if (envScript != null)
        {
            envScript.Reset();
        }
        else
        {
            GetComponentInParent<EnvScriptAbstract>().Reset();
        }
        isInSpot = false;
        var dist = Vector3.Distance(transform.position, target.transform.position);
        previousDistance = dist;
        closestDistance = dist;
        isInSpot = false;
        actionsTaken = 0;
        lastPos = transform.position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    #endregion


    [SerializeField]
    public GameObject target;
    [SerializeField]
    float perfectParkingAngleThreshold = 20.0f;
    [SerializeField]
    float perfectParkingDistanceFromCenterThreshold = 2.0f;

    private CarController carController; //Controls the car
    private Rigidbody rb; //Physics component of the car
    private RayPerceptionSensorComponent3D sensor;

    private float previousDistance = float.MaxValue; // Initialize to a large value initially
    private float closestDistance = float.MaxValue;
    private Vector3 lastPos;
    private bool isInSpot = false;
    private int actionsTaken = 0;


    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
        rb = GetComponent<Rigidbody>();
        sensor = GetComponent<RayPerceptionSensorComponent3D>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Basic observations
        sensor.AddObservation(carController.CurrentSpeed);
        sensor.AddObservation(target.transform.position - transform.position); //Relative position of the parking spot. Do not normalize as we want to reward based on distance.
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(target.transform.rotation);
        /*
         *  //Boolean observation to determine if it is inside parking spot
        sensor.AddObservation(isInSpot);
        //Float observation to determine rotation difference angle between agent and parking spot
        if (!isInSpot) {
            sensor.AddObservation(0f); //Foward angle
            sensor.AddObservation(0f); //Backward angle
            sensor.AddObservation(0f); //best angle
        }
        else
        {
            // Calculate alignment reward based on the angle between car's forward and spot's forward
            float fa = Vector3.Angle(transform.forward, target.transform.forward);
            float ba = Vector3.Angle(-transform.forward, target.transform.forward);
            float aa = Mathf.Min(fa, ba); // Use the smaller angle

            sensor.AddObservation(fa);
            sensor.AddObservation(ba);
            sensor.AddObservation(aa);
        }
         */
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        actionsTaken += 1;
        if (actionsTaken + 1 == MaxStep)
        {
            AddReward(calculateActionsTakenReward());
        }
        Debug.ClearDeveloperConsole();
        float steering = actions.ContinuousActions[0];
        float accel = actions.ContinuousActions[1];
        float reverse = actions.ContinuousActions[2];

        // Input is from -1 to 1, map values accordingly
        accel = (accel + 1) / 2;
        reverse = (reverse + 1) / 2;

        accel = accel - reverse;

        carController.Move(steering,accel,0,0);
        AddReward(CalculateReward());

        if (IsCloseToPerfectPosition())
        {
            Debug.LogWarning("Close to perfection Ending EP");
            AddReward(1000f);
            AddReward(calculateActionsTakenReward());
            //EndEpisode(); DISABLED FOR MULTIAGENT TRAINING. ENABLE FOR OTHER TRAINING WITH SINGLE AGENT IN MAP
            var mae = GetComponentInParent<MultiAgentEnv>();
            if (mae != null)
            {
                mae.agentEPEnded(this.gameObject);
            }
            else
            {
                EndEpisode();
            }
        }

        
    }

    private float CalculateReward()
    {
        //Reward for getting closer each call. If not closer penalise it.
        //Reward if in parking spot by huge amount. Additional Reward for parking alignment.
        //Penalize if vehicle not moving
        return calculateDistanceBasedRewardCloser() + CalculateParkingReward() + calculateStillInSamePosReward();
    }





   

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            Debug.LogWarning("In Parking Spot");
            isInSpot = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            isInSpot = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("Hit object Ending EP");
        SetReward(-100000f);
        //EndEpisode(); DISABLED FOR MULTI AGENT TRAINING. ENABLE IN OTHER MAP WITH SINGLE AGENT
        var mae = GetComponentInParent<MultiAgentEnv>();
        if (mae != null)
        {
            mae.agentEPEnded(this.gameObject);
        }
        else
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        float steering = 0f;
        float accel = 0f;
        float reverse = 0f;
        if (Input.GetKey(KeyCode.A)) {
            steering = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            steering = 1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            accel = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            reverse = 1;
        }

        // Input from network is between -1 to 1, map values accordingly
        accel = accel * 2 - 1;
        reverse = reverse * 2 - 1;

        continuousActionsOut[0] = steering;
        continuousActionsOut[1] = accel;
        continuousActionsOut[2] = reverse;
    }


    /*
     * PARKING SYSTEM :-
    * Rewards and penalties
    * We can reward the Agent for getting close to its parking spot and penalise if it's distance is more than the last time
    * If inside a parking spot we give the max reward of "Getting close to parking spot" as well as rewards for having many section of it's part inside the parking spot
    * We can heavily penalize for hitting other cars and end episode
    * We can reward heavily for being fully inside the parking zone and end episode.
        */







    void rewardV2()
    {
        // Reward for getting closer    
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget < previousDistance)
        {
            if ((int)(distanceToTarget / 3f) < (int)(previousDistance / 3f))
                AddReward(0.02f);

            previousDistance = distanceToTarget;
        }
        else
        {
            // Note: '* 2' is a hard coded value here, which I introduced after tuning the penalty to occur less frequently than
            // the reward, in order to not 'scare' the AI of performing corrective maneuvers where it has to first increase the
            // distance to the target parking spot.
            if ((int)(distanceToTarget / (3f * 2)) > (int)(previousDistance / (3f * 2)))
            {
                AddReward(-0.04f);

                previousDistance = distanceToTarget;
            }
        }
    }





}

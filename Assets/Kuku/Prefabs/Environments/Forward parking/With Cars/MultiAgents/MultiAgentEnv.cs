using System.Collections.Generic;
using UnityEngine;

public class MultiAgentEnv : EnvScriptAbstract
{

    [SerializeField]
    private List<GameObject> targets = new List<GameObject>();
    [SerializeField]
    private List<GameObject> agents = new List<GameObject>();
    [SerializeField]
    private List<Vector3> carSpawnLocs = new List<Vector3>();

    public override void Reset()
    {
     
        //Every reset we are going to reset car location
        //Assign random parking spot to them

        List<GameObject> unassignedSpot = new List<GameObject> ();
        unassignedSpot.AddRange(targets);
        

        foreach (GameObject g in agents)
        {
            int randIndex = Random.Range(0, unassignedSpot.Count - 1);
            g.GetComponent<KukuCarAgent>().target = unassignedSpot[randIndex];
            unassignedSpot.RemoveAt(randIndex);
        }


        /*
        bool randomBoolean = (UnityEngine.Random.Range(0, 2) == 0); // This will generate a random true or false.
        if (randomBoolean)
        {
            car.transform.localRotation = Quaternion.Euler(0, 0f, 0);
        }
        else
        {
            car.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        }
        */

        GenerateCarSpawnLocations();
        
    }

    public new void Start()
    {
        //TODO: Setup all car spawn location
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.F)) { 
            foreach (GameObject g in agents)
            {
                g.GetComponent<KukuCarAgent>().EndEpisode();
                Debug.LogWarning("Ended Ep by force");
            }
        }
    }

    private void GenerateCarSpawnLocations()
    {

        List<Vector3> unassignedLoc = new List<Vector3> ();
        unassignedLoc.AddRange(carSpawnLocs);

        foreach (GameObject g in agents)
        {
            int randIndex = Random.Range(0, unassignedLoc.Count - 1);
            g.transform.localPosition = unassignedLoc[randIndex];
            unassignedLoc.RemoveAt(randIndex);
            g.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private List<GameObject> epEnded = new List<GameObject>();
    public void agentEPEnded(GameObject g)
    {
        if (!epEnded.Contains(g))
        {
            epEnded.Add(g);
        }

        bool resetNow = true;
        foreach(GameObject f in agents)
        {
            if (!epEnded.Contains(f))
            {
                resetNow = false; break;
            }
        }

        if (resetNow)
        {
            epEnded.Clear();
            Reset();
            Debug.LogWarning("Multi agent all agent reset ending EP");
        }
    }
}

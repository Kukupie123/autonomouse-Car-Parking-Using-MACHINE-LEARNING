using System.Collections.Generic;
using UnityEngine;

public class SimeFor : EnvScriptAbstract
{

    private List<Vector3> carSpawnPositions = new List<Vector3>();

    [SerializeField]
    private List<GameObject> cars = new List<GameObject>();

    int maxPosLimit = 10;
    int currentmaxPos = 0;
    bool spawnTop = false;

    public override void Reset()
    {
        int index = Random.Range(0, 4);
        target.transform.localPosition = targetPos[index];

        int carIndex = 0;
        for (int i = 0; i < targetPos.Count; i++)
        {
            if (i == index) continue;
            cars[carIndex].transform.localPosition = targetPos[i];
            carIndex++;
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

        car.transform.localPosition = GenerateCarSpawnLocation();
        car.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public new void Start()
    {
        targetPos.Clear();
        targetPos.Add(new Vector3(7.51999998f, 0, 9.94999981f));
        targetPos.Add(new Vector3(0.99000001f, 0, 9.94999981f));
        targetPos.Add(new Vector3(-6.3499999f, 0, 9.94999981f));
        targetPos.Add(new Vector3(-14.3400002f, 0, 9.94999981f));
        targetPos.Add(new Vector3(-21.6200008f, 0, 9.94999981f));

    }

    private Vector3 GenerateCarSpawnLocation()
    {
        currentmaxPos += 1;

        var topLoc = new Vector3(Random.Range(-22f,7f), 0, 22.5f);
        //var topRot = Quaternion.Euler(0, 0, 0);

        var botLoc = new Vector3(Random.Range(-22f, 7f), 0, -4.19f);
        //var botRot = Quaternion.Euler(0, 0, 0);

        if (currentmaxPos > maxPosLimit)
        {
            spawnTop = !spawnTop;
            currentmaxPos = 0;
        }

        if (spawnTop)
        {
            return topLoc;
        }
        return botLoc;
    }
}

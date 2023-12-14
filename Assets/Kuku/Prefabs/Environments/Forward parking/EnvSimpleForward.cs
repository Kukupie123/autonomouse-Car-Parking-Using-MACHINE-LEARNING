using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSimpleForward : EnvScriptAbstract
{

    private List<Vector3> carSpawnPositions = new List<Vector3>();

    public override void Reset()
    {
        int index = Random.Range(0, 4);
        target.transform.localPosition = targetPos[index];
        /*
        bool randomBoolean = (Random.Range(0, 2) == 0); // This will generate a random true or false.
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
        car.transform.localRotation = Quaternion.Euler(0, 0f, 0);
    }

    public new void Start()
    {
        base.Start();

    }

    private Vector3 GenerateCarSpawnLocation()
    {
        var v1 = new Vector3(Random.Range(-15, 0.69f), 0, 5.25f);
        var v2 = new Vector3(Random.Range(-15, 0.69f), 0, 22.5f);

        if (Random.Range(1,20) % 2 == 0)
        {
            return v1;
        }
        return v2;
    }
}

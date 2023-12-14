using UnityEngine;

public class EnvSimple90 : EnvScriptAbstract
{
    public override void Reset()
    {
        car.transform.localPosition = new Vector3(Random.Range(-15.84f, -0.63f), 0, Random.Range(4.7f, 7.3f));
        if (Random.Range(0, 1) != 1)
        {
            car.transform.localRotation = Quaternion.Euler(0, 91, 0);
        }
        else
        {
            car.transform.localRotation = Quaternion.Euler(0, -91, 0);
        }

        int index = Random.Range(0, 4);
        target.transform.localPosition = targetPos[index];
    }
}

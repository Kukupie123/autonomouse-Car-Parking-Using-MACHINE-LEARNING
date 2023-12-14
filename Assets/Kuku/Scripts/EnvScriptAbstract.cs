using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvScriptAbstract : MonoBehaviour
{
    [SerializeField] protected GameObject target;
    [SerializeField] protected GameObject car;
    [SerializeField] protected List<GameObject> obstacles = new List<GameObject>();
    protected List<Vector3> targetPos = new List<Vector3>();

    public void Start()
    {
        targetPos.Add(new Vector3(-16.44f, 0, 14.77f));
        targetPos.Add(new Vector3(-11.72f, 0, 14.77f));
        targetPos.Add(new Vector3(-7.45f, 0, 14.77f));
        targetPos.Add(new Vector3(-3.16f, 0, 14.77f));
        targetPos.Add(new Vector3(1.2f, 0, 14.77f));

    }

    public abstract void Reset();

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            car.GetComponent<KukuCarAgent>().EndEpisode();
        }
    }



}

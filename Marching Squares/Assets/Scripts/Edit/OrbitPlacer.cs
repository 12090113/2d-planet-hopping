using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitPlacer : MonoBehaviour
{
    public GravityObject gravObj;
    public GravityObject centralBody;
    public float massOffset = 0;
    public Direction direction = Direction.Clockwise;
    public bool orbit = true;
    
    public enum Direction
    {
        Clockwise = 1,
        Counterclockwise = -1
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            orbit = false;
            return;
        }
        gravObj = GetComponent<GravityObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (orbit && transform.hasChanged)
            Orbit();
        transform.hasChanged = false;
    }

    void Orbit()
    {
        if (gravObj == null)
            gravObj = GetComponent<GravityObject>();
        GravityObject[] list = FindObjectsOfType<GravityObject>();
        if (centralBody == null)
        {
            GravityObject max = list[0];
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].mass > max.mass && list[i] != gravObj)
                {
                    max = list[i];
                }
            }
            centralBody = max;
        }
        float dist = (centralBody.transform.position - transform.position).magnitude;
        massOffset = 0;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] != centralBody && list[i].mass > 1000f)
            {
                float dist2 = (centralBody.transform.position - list[i].transform.position).magnitude;
                if (dist > dist2)
                {
                    massOffset += list[i].mass;
                }
            }
        }
        gravObj = GetComponent<GravityObject>();
        Vector3 pos1 = gravObj.transform.position;
        Vector3 pos2 = centralBody.transform.position;
        Vector3 dir = Vector2.Perpendicular(pos2 - pos1).normalized * (int)direction;
        gravObj.velocity = Mathf.Sqrt(NBodySimulation.G * (centralBody.mass + massOffset) / Vector3.Distance(pos1, pos2)) * dir + centralBody.velocity;
    }
}

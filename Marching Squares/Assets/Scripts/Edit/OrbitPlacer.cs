using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitPlacer : MonoBehaviour
{
    public GravityObject gravObj;
    public GravityObject[] centralBody;
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
            centralBody = new GravityObject[1];
            GravityObject max = list[0];
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].mass > max.mass && list[i] != gravObj)
                {
                    max = list[i];
                }
            }
            centralBody[0] = max;
        }
        (Vector3, Vector3, float) center = Center();
        Debug.Log(center);
        Vector3 pos = center.Item1;
        Vector3 vel = center.Item2;
        float mass = center.Item3;
        float dist = (pos - transform.position).magnitude;
        massOffset = 0;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].mass > 1000f)
            {
                float dist2 = (pos - list[i].transform.position).magnitude;
                if (dist > dist2)
                {
                    massOffset += list[i].mass;
                }
            }
        }
        massOffset -= mass;
        gravObj = GetComponent<GravityObject>();
        Vector3 pos1 = gravObj.transform.position;
        Vector3 dir = Vector2.Perpendicular(pos - pos1).normalized * (int)direction;
        gravObj.velocity = Mathf.Sqrt(NBodySimulation.G * (mass + massOffset) / Vector3.Distance(pos1, pos)) * dir + vel;
    }

    (Vector3, Vector3, float) Center()
    {
        Vector3 centerOfMass = Vector3.zero;
        float totalMass = 0f;
        Vector3 totalVelocity = Vector3.zero;
        foreach (GravityObject body in centralBody)
        {
            centerOfMass += body.transform.position * body.mass;
            totalMass += body.mass;
            totalVelocity += body.velocity;
        }
        centerOfMass /= totalMass;
        return (centerOfMass, totalVelocity, totalMass);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitPlacer : MonoBehaviour
{
    public GravityObject gravObj;
    public GravityObject centralBody;
    public float massOffset = 0;
    public bool orbit = true;
    // Start is called before the first frame update
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
        if (orbit)
            Orbit();
    }

    void Orbit()
    {
        if (gravObj == null)
            gravObj = GetComponent<GravityObject>();
        if (centralBody == null)
        {
            GravityObject[] list = FindObjectsOfType<GravityObject>();
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
        gravObj = GetComponent<GravityObject>();
        Vector3 pos1 = gravObj.transform.position;
        Vector3 pos2 = centralBody.transform.position;
        Vector3 dir = Vector2.Perpendicular(pos1 - pos2).normalized;
        gravObj.velocity = Mathf.Sqrt(NBodySimulation.G * (centralBody.mass + massOffset) / Vector3.Distance(pos1, pos2)) * dir + centralBody.velocity;
    }
}

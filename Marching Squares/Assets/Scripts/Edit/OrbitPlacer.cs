using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitPlacer : MonoBehaviour
{
    GravityObject gravObj;
    public GravityObject centralBody;
    public float massOffset = 0;
    public bool orbit = true;
    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isPlaying)
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
        gravObj = GetComponent<GravityObject>();
        Debug.Log("hi");
        Vector3 pos1 = gravObj.transform.position;
        Vector3 pos2 = centralBody.transform.position;
        Vector3 dir = Vector2.Perpendicular(pos1 - pos2).normalized;
        gravObj.velocity = Mathf.Sqrt(NBodySimulation.G * (centralBody.mass + massOffset) / Vector3.Distance(pos1, pos2)) * dir + centralBody.velocity;
    }
}

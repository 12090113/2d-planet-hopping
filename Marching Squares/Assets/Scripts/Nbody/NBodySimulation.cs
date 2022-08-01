using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBodySimulation : MonoBehaviour
{
    GravityObject[] bodies;
    Rigidbody2D[] bodiesrb;
    //static NBodySimulation instance;

    int threads = 1;
    public ComputeShader shader;
    public static ComputeBuffer pos_buf;
    //public static ComputeBuffer vel_buf;
    public static ComputeBuffer mass_buf;
    public static ComputeBuffer force_buf;

    void Awake()
    {
        bodies = FindObjectsOfType<GravityObject>();
        bodiesrb = new Rigidbody2D[bodies.Length];
        threads = Mathf.CeilToInt(bodies.Length / 64f) * 64;
    }

    void Start()
    {
        pos_buf = new ComputeBuffer(threads, 3 * sizeof(float));
        //vel_buf = new ComputeBuffer(threads, 3 * sizeof(float));
        mass_buf = new ComputeBuffer(threads, sizeof(float));
        force_buf = new ComputeBuffer(threads, 3 * sizeof(float));

        // These global buffers apply to every shader with these buffers defined.
        Shader.SetGlobalBuffer(Shader.PropertyToID("position"), pos_buf);
        //Shader.SetGlobalBuffer(Shader.PropertyToID("velocity"), vel_buf);
        Shader.SetGlobalBuffer(Shader.PropertyToID("mass"), mass_buf);
        Shader.SetGlobalBuffer(Shader.PropertyToID("force"), force_buf);

        float[] pos_data = new float[threads * 3];
        //float[] vel_data = new float[threads * 3];
        float[] mass_data = new float[threads];
        float[] force_data = new float[threads * 3];
        Debug.Log(bodies.Length);
        Debug.Log(threads);
        for (int i = 0; i < bodies.Length; i++)
        {
            pos_data[i * 3 + 0] = bodies[i].transform.position.x;
            pos_data[i * 3 + 1] = bodies[i].transform.position.y;
            pos_data[i * 3 + 2] = bodies[i].transform.position.z;
            //vel_data[i * 3 + 0] = bodies[i].velocity.x;
            //vel_data[i * 3 + 1] = bodies[i].velocity.y;
            //vel_data[i * 3 + 2] = bodies[i].velocity.z;
            mass_data[i] = bodies[i].mass;
            force_data[i * 3 + 0] = force_data[i * 3 + 1] = force_data[i * 3 + 2] = 0;
            bodiesrb[i] = bodies[i].GetComponent<Rigidbody2D>();
            bodiesrb[i].mass = bodies[i].mass;
            Debug.Log(bodies[i].name + ": mass= " + bodies[i].mass + ", rb mass= " + bodiesrb[i].mass);
            bodiesrb[i].velocity = bodies[i].velocity;
        }
        pos_buf.SetData(pos_data);
        //vel_buf.SetData(vel_data);
        mass_buf.SetData(mass_data);
        force_buf.SetData(force_data);
        shader.SetInt("num", bodies.Length);
    }

    void FixedUpdate()
    {
        shader.Dispatch(shader.FindKernel("CSMain"), 1, threads/64, 1);

        float[] pos_data = new float[threads * 3];
        float[] force_data = new float[threads * 3];
        force_buf.GetData(force_data);
        for (int i = 0; i < bodies.Length; i++)
        {
            bodiesrb[i].AddForce(new Vector2(force_data[i * 3], force_data[i * 3 + 1]));
            if (bodies[i].alignWithGravity)
                bodies[i].AlignWith(new Vector2(force_data[i * 3], force_data[i * 3 + 1]));
            Vector3 pos = bodies[i].transform.position;
            pos_data[i * 3 + 0] = pos.x;
            pos_data[i * 3 + 1] = pos.y;
            pos_data[i * 3 + 2] = pos.z;
        }
        pos_buf.SetData(pos_data);
    }

    void OnDestroy()
    {
        pos_buf.Dispose();
        //vel_buf.Dispose();
        mass_buf.Dispose();
        force_buf.Dispose();
    }
}
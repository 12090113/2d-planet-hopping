using UnityEngine.Rendering.Universal;
using UnityEngine;

public class PlanetShadow : MonoBehaviour
{
    public Transform sun;
    // Start is called before the first frame update
    void Start()
    {
        //sun = FindObjectOfType<Light2D>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.up = sun.position - transform.position;
    }
}

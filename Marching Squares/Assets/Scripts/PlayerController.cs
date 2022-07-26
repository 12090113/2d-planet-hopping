using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform planet;
    Rigidbody2D rb;
    //private new Collider2D collider;
    bool input = true;
    //const float G = 0.001f;
    public float speed = 3;
    public bool grounded = false;
    public int jumped = 0;

    private float targetAngle = 0f; // the desired angle
    private float curAngle; // current angle
    private float accel; // applied accel
    private float angSpeed = 0f; // current ang speed
    private float maxAccel = 90f;//180f; // max accel in degrees/second2
    private float maxASpeed = 45f;//90f; // max angular speed in degrees/second
    private float pGain = 20f; // the proportional gain
    private float dGain = 10f; // differential gain
    private float lastError;

    void Start()
    {
        //planet = FindObjectOfType<GravityObject>().transform;
        rb = GetComponent<Rigidbody2D>();
        //collider = GetComponent<Collider2D>();

        targetAngle = transform.eulerAngles.z; // get the current angle just for start
        curAngle = targetAngle;
    }

    void FixedUpdate()
    {
        AlignWith(planet);
        if (grounded)
        {
            if (input)
            {
                if (jumped <= 0)
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        rb.AddForce(transform.up * 200);
                        jumped = 50;
                    }
                } else
                {
                    jumped -= 1;
                }
                
                if (Input.GetKey(KeyCode.D))
                {
                    rb.AddForce(transform.right * speed);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    rb.AddForce(transform.right * -speed);
                }
            }
        } else
        {
            if (jumped > 0)
            {
                jumped -= 3;
            }
        }
        //float dist = Vector3.Distance(transform.position, planet.position);
        //float g = G * planet.mass / (dist * dist);
        //Vector3 dir = planet.position - transform.position;
        //rb.AddForce(g * dir);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        grounded = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        grounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        grounded = false;
    }

    void AlignWith(Transform target)
    {
        targetAngle = Mathf.Atan2(planet.position.y - transform.position.y, planet.position.x - transform.position.x) * Mathf.Rad2Deg + 90;
        var error = Mathf.DeltaAngle(curAngle, targetAngle); // generate the error signal
        var diff = (error - lastError) / Time.fixedDeltaTime; // calculate differential error
        lastError = error;
        // calculate the acceleration:
        accel = error * pGain + diff * dGain;
        // limit it to the max acceleration
        accel = Mathf.Clamp(accel, -maxAccel, maxAccel);
        // apply accel to angular speed:
        angSpeed += accel * Time.deltaTime;
        // limit max angular speed
        angSpeed = Mathf.Clamp(angSpeed, -maxASpeed, maxASpeed);
        curAngle += angSpeed * Time.deltaTime; // apply the rotation to the angle...
                                               // and make the object follow the angle (must be modulo 360)
        transform.rotation = Quaternion.Euler(0, 0, curAngle % 360);
    }
}

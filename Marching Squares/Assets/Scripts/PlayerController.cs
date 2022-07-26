using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform tran;
    Rigidbody2D playerrb;
    Rigidbody2D rb;
    GravityObject gravObj;
    public Rigidbody2D ship;
    Transform seat;
    //private new Collider2D collider;
    bool input = true;
    //const float G = 0.001f;
    public float baseSpeed = 20;
    public float speed = 20;
    public bool grounded = false;
    public bool jetpack = false;
    public int jumped = 0;
    public float rotation = 1;
    public float timeSpeed = 1;

    private float targetAngle = 0f; // the desired angle
    private float curAngle; // current angle
    private float accel; // applied accel
    private float angSpeed = 0f; // current ang speed
    private float maxAccel = 90f;//180f; // max accel in degrees/second2
    private float maxASpeed = 45f;//90f; // max angular speed in degrees/second
    private float pGain = 20f; // the proportional gain
    private float dGain = 10f; // differential gain
    private float lastError;
    public enum InputMode
    {
        Ground,
        Space,
        Ship
    }
    public InputMode inputMode = InputMode.Ground;
    private InputMode prevInput = InputMode.Ground;

    void Start()
    {
        //planet = FindObjectOfType<GravityObject>().tran;
        playerrb = GetComponent<Rigidbody2D>();
        rb = playerrb;
        gravObj = GetComponent<GravityObject>();
        tran = transform;
        seat = ship.transform.GetChild(0);
        //collider = GetComponent<Collider2D>();

        targetAngle = tran.eulerAngles.z; // get the current angle just for start
        curAngle = targetAngle;
    }

    void FixedUpdate()
    {
        //AlignWith(planet);
        //if (grounded)
        //{
        if (input)
        {
            if (inputMode == InputMode.Ground)
            {
                if (jumped <= 0)
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        rb.AddForce(tran.up * 500);
                        jumped = 50;
                    }
                }
                else
                {
                    jumped -= 1;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    rb.AddForce(tran.right * speed);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    rb.AddForce(tran.right * -speed);
                }
            }
            else if (inputMode >= InputMode.Space)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    rb.AddForce(tran.right * speed);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    rb.AddForce(tran.right * -speed);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    rb.AddForce(tran.up * speed);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    rb.AddForce(tran.up * -speed);
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    rb.AddTorque(rotation * (speed * speed)/400);
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    rb.AddTorque(-rotation * (speed * speed)/400);
                }
                else if (rb.angularVelocity < -12)
                {
                    rb.AddTorque(rotation * (speed * speed) / 400);
                }
                else if (rb.angularVelocity > 12)
                {
                    rb.AddTorque(-rotation * (speed * speed) / 400);
                }
                else
                {
                    rb.angularVelocity = 0;
                }
            }
            if (inputMode == InputMode.Ship)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    rb.AddForce(tran.up * speed * 10);
                }
                playerrb.velocity = ship.velocity;
                transform.SetPositionAndRotation(seat.position, seat.rotation);
            }
        }

        //} else
        //{
           // if (jumped > 0)
           // {
              //  jumped -= 3;
            //}
        //}
        //float dist = Vector3.Distance(tran.position, planet.position);
        //float g = G * planet.mass / (dist * dist);
        //Vector3 dir = planet.position - tran.position;
        //rb.AddForce(g * dir);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            timeSpeed += 1f;
            Time.timeScale = timeSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus) && Time.timeScale > 0)
        {
            timeSpeed -= 1f;
            Time.timeScale = timeSpeed;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (inputMode == InputMode.Ground)
            {
                inputMode = InputMode.Space;
                gravObj.alignWithGravity = false;
            }
            else if (inputMode == InputMode.Space)
            {
                inputMode = InputMode.Ground;
                gravObj.alignWithGravity = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (inputMode != InputMode.Ship)
            {
                prevInput = inputMode;
                inputMode = InputMode.Ship;
                speed = baseSpeed * 10;
                rb = ship;
            }
            else
            {
                inputMode = prevInput;
                speed = baseSpeed;
                rb = playerrb;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
        if (collision.name.Equals("Player"))
        {
            grounded = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        grounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        grounded = false;
    }

    public void AlignWith(Transform target)
    {
        targetAngle = target.transform.rotation.eulerAngles.z;
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
        tran.rotation = Quaternion.Euler(0, 0, curAngle % 360);
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Planet planet;
    Rigidbody2D rb;
    bool input = true;
    const float G = 0.001f;

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
        planet = FindObjectOfType<Planet>();
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = 0.01f;

        targetAngle = transform.eulerAngles.z; // get the current angle just for start
        curAngle = targetAngle;
    }

    void FixedUpdate()
    {
        AlignWith(planet.transform);

        if (input)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(transform.up * 1);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(transform.right * 1);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(transform.right * -1);
            }
        }
        float dist = Vector3.Distance(transform.position, planet.transform.position);
        float g = G * planet.mass / (dist * dist);
        Vector3 dir = planet.transform.position - transform.position;
        rb.AddForce(g * dir);
    }
    void AlignWith(Transform target)
    {
        targetAngle = Mathf.Atan2(planet.transform.position.y - transform.position.y, planet.transform.position.x - transform.position.x) * Mathf.Rad2Deg + 90;
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

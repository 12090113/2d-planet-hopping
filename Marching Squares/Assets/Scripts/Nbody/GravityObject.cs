using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public Vector3 velocity;
    public float mass = 1;
    public bool alignWithGravity = false;

    private float targetAngle = 0f; // the desired angle
    private float curAngle; // current angle
    private float accel; // applied accel
    private float angSpeed = 0f; // current ang speed
    private float maxAccel = 90f;//180f; // max accel in degrees/second2
    private float maxASpeed = 45f;//90f; // max angular speed in degrees/second
    private float pGain = 20f; // the proportional gain
    private float dGain = 10f; // differential gain
    private float lastError;

    public void AlignWith(Vector2 target)
    {
        targetAngle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg + 90;//Vector2.Angle(Vector2.,target); //Mathf.Atan2(planet.position.y - transform.position.y, planet.position.x - transform.position.x) * Mathf.Rad2Deg + 90;
        Debug.Log(target + ": " + targetAngle);
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

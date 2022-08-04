using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    Rigidbody2D rb;
    CircleCollider2D col;
    float radius = 1;
    public float drag = 0.1f;
    //List<(bool,Rigidbody2D)> rbs = new();
    Dictionary<Rigidbody2D, bool> rbs = new();
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        foreach (var collider in GetComponents<CircleCollider2D>())
        {
            if (collider.isTrigger)
                col = collider;
        }
        radius = col.radius * transform.lossyScale.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (var obj in rbs)
        {
            float dist = Vector2.Distance(transform.position, obj.Key.position);
            float dragcof = 1 - dist / radius;
            Debug.Log(obj.Key.name + ": " + dist + " / " + radius + " = " + dragcof);
            Vector2 relativeVelocity = rb.velocity - obj.Key.velocity;
            Vector2 vector = relativeVelocity.sqrMagnitude * relativeVelocity.normalized * drag * dragcof;
            float num = Mathf.Min(vector.magnitude * Time.fixedDeltaTime, relativeVelocity.magnitude);
            obj.Key.velocity += relativeVelocity.normalized * num;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rbother = collision.attachedRigidbody;
        if (rbother != null)
        {
            rbs.Add(rbother, false);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D rbother = collision.attachedRigidbody;
        if (rbother != null)
        {
            rbs.Remove(rbother);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Rigidbody2D rbother = collision.otherRigidbody;
        //if (rbs.TryGetValue(rbother, out bool value))
        //{
        //    value = true;
        //}
    }
}

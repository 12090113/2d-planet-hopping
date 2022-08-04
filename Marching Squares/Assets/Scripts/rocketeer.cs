using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocketeer : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 3;
    List<Rigidbody2D> rbs = new();
    PolygonCollider2D col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        foreach(var collider in GetComponents<PolygonCollider2D>())
        {
            if (collider.isTrigger)
                col = collider;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(transform.up * speed);
        }
        foreach (Rigidbody2D obj in rbs)
        {
            obj.GetComponent<PlayerController>()?.AlignWith(transform);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rbother = collision.attachedRigidbody;
        if (rbother != null)
        {
            rbs.Add(rbother);
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

}

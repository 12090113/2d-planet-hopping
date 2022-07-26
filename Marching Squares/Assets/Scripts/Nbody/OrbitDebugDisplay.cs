﻿using UnityEngine;

[ExecuteInEditMode]
public class OrbitDebugDisplay : MonoBehaviour {

    public int numSteps = 1000;
    public float timeStep = 0.1f;
    public bool usePhysicsTimeStep;

    public bool relativeToBody;
    public GravityObject centralBody;
    public float width = 100;
    public bool useThickLines;

    public bool drawLines = true;

    void Start () {
        if (Application.isPlaying) {
            useThickLines = true;
        }
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            drawLines = !drawLines;
            if (!drawLines) HideOrbits();
        }
        if (drawLines) {
            DrawOrbits ();
        }
    }

    void DrawOrbits () {
        GravityObject[] bodies = FindObjectsOfType<GravityObject> ();
        var virtualBodies = new VirtualBody[bodies.Length];
        var drawPoints = new Vector3[bodies.Length][];
        int referenceFrameIndex = 0;
        Vector3 referenceBodyInitialPosition = Vector3.zero;

        // Initialize virtual bodies (don't want to move the actual bodies)
        for (int i = 0; i < virtualBodies.Length; i++) {
            virtualBodies[i] = new VirtualBody (bodies[i]);
            drawPoints[i] = new Vector3[numSteps+1];
            drawPoints[i][0] = bodies[i].transform.position;

            if (bodies[i] == centralBody && relativeToBody) {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualBodies[i].position;
            }
        }

        // Simulate
        for (int step = 1; step < numSteps+1; step++) {
            Vector3 referenceBodyPosition = (relativeToBody) ? virtualBodies[referenceFrameIndex].position : Vector3.zero;
            // Update velocities
            for (int i = 0; i < virtualBodies.Length; i++) {
                virtualBodies[i].velocity += CalculateAcceleration (i, virtualBodies) * timeStep;
            }
            // Update positions
            for (int i = 0; i < virtualBodies.Length; i++) {
                Vector3 newPos = virtualBodies[i].position + virtualBodies[i].velocity * timeStep;
                virtualBodies[i].position = newPos;
                if (relativeToBody) {
                    var referenceFrameOffset = referenceBodyPosition - referenceBodyInitialPosition;
                    newPos -= referenceFrameOffset;
                }
                if (relativeToBody && i == referenceFrameIndex) {
                    newPos = referenceBodyInitialPosition;
                }

                drawPoints[i][step] = newPos;
            }
        }

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < virtualBodies.Length; bodyIndex++) {
            Random.InitState(bodies[bodyIndex].transform.GetSiblingIndex() + 1); // +1 for better colors because why not
            Color pathColour = Random.ColorHSV(0f, 1f, 1f, 1f, 0.75f, 1f);
            //if (bodies[bodyIndex].name.Equals("Player"))
            //{
            //    pathColour = Color.black;
            //}

            if (useThickLines) {
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer> ();
                lineRenderer.enabled = true;
                lineRenderer.positionCount = drawPoints[bodyIndex].Length;
                lineRenderer.SetPositions (drawPoints[bodyIndex]);
                lineRenderer.startColor = pathColour;
                lineRenderer.endColor = pathColour;
                lineRenderer.widthMultiplier = width;
            } else {
                for (int i = 0; i < drawPoints[bodyIndex].Length - 1; i++) {
                    Debug.DrawLine (drawPoints[bodyIndex][i], drawPoints[bodyIndex][i + 1], pathColour);
                }

                // Hide renderer
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer> ();
                if (lineRenderer) {
                    lineRenderer.enabled = false;
                }
            }

        }
    }

    Vector3 CalculateAcceleration (int i, VirtualBody[] virtualBodies) {
        Vector3 acceleration = Vector3.zero;
        for (int j = 0; j < virtualBodies.Length; j++) {
            if (i == j) {
                continue;
            }
            Vector3 forceDir = (virtualBodies[j].position - virtualBodies[i].position).normalized;
            float sqrDst = (virtualBodies[j].position - virtualBodies[i].position).sqrMagnitude;
            acceleration += forceDir * 0.1f * virtualBodies[j].mass / sqrDst;
        }
        return acceleration;
    }

    void HideOrbits () {
        GravityObject[] bodies = FindObjectsOfType<GravityObject> ();

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++) {
            var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer> ();
            lineRenderer.positionCount = 0;
        }
    }

    void OnValidate () {
        if (usePhysicsTimeStep) {
            timeStep = Time.fixedDeltaTime;
        }
    }

    class VirtualBody {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;

        public VirtualBody (GravityObject body) {
            position = body.transform.position;
            if (Application.isPlaying)
                velocity = body.GetComponent<Rigidbody2D>().velocity;
            else
                velocity = body.velocity;
            mass = body.mass;
        }
    }
}
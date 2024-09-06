using System;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 2.5f;
    public float turnSpeed = 5f;
    public float detectionRadious = 3.0f;

    public float alignmentValue;
    public float cohesionValue;
    public float separationValue;
    public float directionValue;

    private Func<Boid, Vector3> Alignment;
    private Func<Boid, Vector3> Cohesion;
    private Func<Boid, Vector3> Separation;
    private Func<Boid, Vector3> Direction;

    public void Init(Func<Boid, Vector3> Alignment,
        Func<Boid, Vector3> Cohesion,
        Func<Boid, Vector3> Separation,
        Func<Boid, Vector3> Direction)
    {
        this.Alignment = Alignment;
        this.Cohesion = Cohesion;
        this.Separation = Separation;
        this.Direction = Direction;
    }

    private void Update()
    {
        Vector3 acs = ACS();
        transform.forward = Vector3.Lerp(transform.forward, acs, turnSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public Vector3 ACS()
    {
        Vector3 ACS = (Alignment(this) * alignmentValue) + (Cohesion(this) * cohesionValue) +
                      (Separation(this) * separationValue) + (Direction(this) * directionValue);
        return ACS.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);

        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(transform.position, detectionRadious);
    }
}
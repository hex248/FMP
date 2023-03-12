using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Particle 
{
    float mass;
    public Vector3 location;
    Vector3 force;
    Vector3 velocity;
    public Particle(float mass, Vector3 startingLocation)
    {
        this.location = startingLocation;
        this.mass = mass;
    }

    public void Simulate()
    {
        this.force = Physics.gravity * this.mass;
        Collider[] others = Physics.OverlapSphere(this.location, 0.5f, ~0, QueryTriggerInteraction.Ignore);
        if(others.Length > 0)
        {
            foreach(Collider other in others)
            {
                if (Physics.Raycast(this.location, other.ClosestPoint(this.location) - this.location, out RaycastHit hit))
                {
                    Vector3 force = this.force + ((this.velocity / Time.fixedDeltaTime) * this.mass);
                    Vector3 reactionForce = this.mass * force * Mathf.Cos(Vector3.Angle(Vector3.down, -hit.normal));
                    Vector3 frictionForce = this.mass * force * Mathf.Sin(Vector3.Angle(Vector3.down, -hit.normal));
                    //this.force -= reactionXForce * Vector3.right * Mathf.Sin(Vector3.Angle(Vector3.up, hit.normal));
                    this.force -= reactionForce;
                    //this.force -= frictionForce * 0.4f;
                }
            }
        }
        this.velocity += (this.force / this.mass) * Time.fixedDeltaTime;
        this.location += this.velocity * Time.fixedDeltaTime;
    }
};

public class ClothSimulationScript : MonoBehaviour
{
    Particle[] particles;

    // Start is called before the first frame update
    void Start()
    {
        particles = new Particle[40];
        for(int i = 0; i < particles.Length - 1; i++)
        {
            particles[i] = new Particle(1.0f, Vector3.up * 10.0f + Vector3.right * i * 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = particles[0].location;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < particles.Length - 1; i++)
        {
            particles[i].Simulate();
        }
    }
    private void OnDrawGizmos()
    {
        for(int i = 1; i < particles.Length - 1; i++)
        {
            Gizmos.DrawLine(particles[i - 1].location, particles[i].location);
        }
    }
}

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
        if (Physics.Raycast(this.location, velocity.normalized, out RaycastHit hit) && hit.distance <= 0.5f)
        {
            float reactionForce = -(this.velocity.y / Time.fixedDeltaTime) * this.mass;
            this.force += reactionForce * hit.normal;
        }
        this.velocity += (this.force * Time.fixedDeltaTime) / this.mass;
        this.location += this.velocity * Time.fixedDeltaTime;
    }
};

public class ClothSimulationScript : MonoBehaviour
{
    Particle[] particles;

    // Start is called before the first frame update
    void Start()
    {
        particles = new Particle[1];
        particles[0] = new Particle(1.0f, Vector3.up * 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        particles[0].Simulate();
        transform.position = particles[0].location;
    }
}

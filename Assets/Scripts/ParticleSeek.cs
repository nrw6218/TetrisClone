using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to apply a given force to the particle system in the
/// direction of the given target.
/// </summary>
public class ParticleSeek : MonoBehaviour
{
    public Transform target;
    public float force = 10.0f;

    ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void LateUpdate()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];

        ps.GetParticles(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem.Particle p = particles[i];

            Vector3 worldPosition;

            if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
            {
                worldPosition = transform.TransformPoint(p.position);
            }
            else if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
            {
                worldPosition = ps.main.customSimulationSpace.TransformPoint(p.position);
            }
            else
            {
                worldPosition = p.position;
            }

            Vector3 distanceToTarget = (target.position - worldPosition).normalized;
            Vector3 seekForce = distanceToTarget * force * Time.deltaTime;

            p.velocity += seekForce;
            particles[i] = p;
        }

        ps.SetParticles(particles, particles.Length);
    }
}

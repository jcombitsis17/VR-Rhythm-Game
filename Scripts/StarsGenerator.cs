
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StarsGenerator : MonoBehaviour
{
    public int numStars = 100;
    public int radius = 75;
    public float starSize = 0.1f;
    public float fieldWidth = 20f;
    public float fieldHeight = 25f;

    ParticleSystem Particles;
    ParticleSystem.Particle[] Stars;


    void Awake()
    {
        Stars = new ParticleSystem.Particle[numStars];
        Particles = GetComponent<ParticleSystem>();

        float xOffset = fieldWidth * 0.5f;                                                                                                        // Offset the coordinates to distribute the spread
        float yOffset = fieldHeight * 0.5f;                                                                                                       // around the object's center

        for (int i = 0; i < numStars; i++)
        {
            float randSize = Random.Range(10f, 15f); // Randomize star size within parameters

            Stars[i].position = GetRandomInSphere(radius) + transform.position;
            Stars[i].startSize = starSize * randSize;
            Stars[i].startColor = new Color(1f, 1f, 1f, 1f);
        }
        Particles.SetParticles(Stars, Stars.Length);                                                                // Write data to the particle system
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0.02f, 0));
    }

    Vector3 GetRandomInSphere(float radius)
    {
        return Random.onUnitSphere * radius;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem particles = GetComponent<ParticleSystem>();
        Destroy(gameObject, particles.main.startLifetime.constant);
    }

    public void FixedUpdate()
    {
        transform.Translate(Vector3.back * Time.fixedDeltaTime * 10f); 
    }
}

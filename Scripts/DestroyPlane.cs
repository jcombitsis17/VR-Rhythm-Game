using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlane : MonoBehaviour
{
    private AudioSource hitSound;
    void Start()
    {
        hitSound = GetComponent<AudioSource>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block") && other.enabled)
        {
            Block block = other.GetComponent<Block>();
            block.DestroyBlock(0, true);
            hitSound.Play();
        }
    }
}

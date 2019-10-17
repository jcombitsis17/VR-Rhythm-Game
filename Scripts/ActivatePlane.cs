using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePlane : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            Block block = other.GetComponent<Block>();
            block.activated = true;
        }
    }
}

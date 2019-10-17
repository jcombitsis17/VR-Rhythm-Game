using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float lifetime;
    public TextMesh textMesh;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }
}

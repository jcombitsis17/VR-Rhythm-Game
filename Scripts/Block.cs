using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public char gesture;
    public bool activated = false;
    public Renderer arrowtext;
    public GameObject floatingTextPrefab;
    public ParticleSystem particles;
    
    private GameController gameController;

    private bool hit = false;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void FixedUpdate()
    { 
        if (!hit)
            transform.Translate(Vector3.back * Time.fixedDeltaTime * gameController.blockSpeed); //move the group
    }

    //Explosion 
    public void DestroyBlock(float accuracy, bool destroyPlane = false)
    {
        hit = true;
        gameController.RemoveBlockFromQueue(this);

        ParticleSystem.MainModule main = particles.main;
        if (destroyPlane)
        {            
            gameController.DestroyPlaneHit();
            main.startColor = Color.red;
        }
        else
        {
            main.startColor = Color.blue;
            GameObject floatingText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            floatingText.GetComponent<FloatingText>().SetText(Mathf.RoundToInt(accuracy * 100).ToString());
        }
        Instantiate(particles.gameObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

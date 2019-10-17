using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualization : MonoBehaviour
{
    public Animator trackAnimation;
    public Animator scoreAnimation;
    public Animator multiplierAnimation;

    public Color trackColorHighAccuracy;
    public Color trackColorMidAccuracy;
    public Color trackColorLowAccuracy;
    public Color trackColor;
    public List<Renderer> trackEdges;

    public ParticleSystem backgroundRipple;
    public Renderer UIPlane;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (Renderer trackEdge in trackEdges)
        {
            trackEdge.material.color = trackColor;
        }
    }

    public void ToggleBackgroundRipple(bool start, int bpm = 0)
    {
        ParticleSystem.EmissionModule emission = backgroundRipple.emission;
        emission.rateOverTime = bpm / 60.0f;
        if (start)
            backgroundRipple.Play();
        else
            backgroundRipple.Stop();
    }

    public void AnimateUI()
    {
        trackAnimation.Play(0);
        scoreAnimation.Play(0);
        multiplierAnimation.Play(0);
    }

    public IEnumerator ChangeTrackColor(int blockPosition, float accuracy)
    {
        int trackPosition = (blockPosition + 4)/2;
        Color newTrackColor = trackColorLowAccuracy;

        if (accuracy > 0.90)
            newTrackColor = trackColorHighAccuracy;
        else if (accuracy > 0.80)
            newTrackColor = trackColorMidAccuracy;

        trackEdges[trackPosition].material.color = newTrackColor;
        trackEdges[trackPosition + 1].material.color = newTrackColor;
        yield return new WaitForSeconds(0.2f);
        trackEdges[trackPosition].material.color = trackColor;
        trackEdges[trackPosition + 1].material.color = trackColor;
    }

    public void ChangeUIColor(int multiplier)
    {
        if (multiplier == 1)
            UIPlane.sharedMaterial.color = new Color(0f, 0f, 0.5f, 1f);
        else
            UIPlane.sharedMaterial.color = new Color(
                  Random.Range(0f, 1f),
                  Random.Range(0f, 1f),
                  Random.Range(0f, 1f),
                  1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class GetAudioData : MonoBehaviour
{
    AudioSource song;
    public static float[] samples = new float[128];

    // Start is called before the first frame update
    void Start()
    {
        song = GetComponent<AudioSource>();
    }

    public static void SetNumberOfSamples(int numberOfSamples)
    {
        samples = new float[numberOfSamples / 2];
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrum();
    }

    void GetSpectrum()
    {
        song.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }
}

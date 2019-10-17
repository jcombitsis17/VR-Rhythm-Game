using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualization : MonoBehaviour
{
    public GameObject cubePrefabRight;
    public GameObject cubePrefabLeft;
    private GameObject[] cubeWallRight;
    private GameObject[] cubeWallLeft;
    public float sampleScale;
    public float cubeSize;
    public float distance;
    public int updateInterval;
    public int numberOfCubes;

    public GameObject semicirclePrefab;
    private GameObject[] audioCircle;

    private float currentUpdate = 0;
    private Vector3[] startScale;
    private Vector3[] endScale;
    
    public Color startColor;
    private float startHue;
    private float startSaturation;
    private float startValue;
    public float hueStep;

    // Start is called before the first frame update
    void Start()
    {
        GetAudioData.SetNumberOfSamples(numberOfCubes);
        cubeWallRight = new GameObject[numberOfCubes / 2];
        cubeWallLeft = new GameObject[numberOfCubes / 2];
        audioCircle = new GameObject[numberOfCubes];
        startScale = new Vector3[numberOfCubes / 2];
        endScale = new Vector3[numberOfCubes / 2];
        //instantiateCubes(cubePrefabRight, "Right");
        //instantiateCubes(cubePrefabLeft, "Left");
        makeSemiCircle(semicirclePrefab);
        Color.RGBToHSV(startColor, out startHue, out startSaturation, out startValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentUpdate == 0)
        {
            for (int i = 0; i < numberOfCubes/2; i++)
            {
                startScale[i] = audioCircle[i].transform.localScale;
                endScale[i] = new Vector3(startScale[i].x, (GetAudioData.samples[i] * sampleScale), startScale[i].z);
            }
        }
        else
        {
            for (int i = 0; i < numberOfCubes/2; i++)
            {
                audioCircle[i].transform.localScale = Vector3.Lerp(startScale[i], endScale[i], (currentUpdate + 1) / updateInterval);
                audioCircle[i + (numberOfCubes/2)].transform.localScale = Vector3.Lerp(startScale[i], endScale[i], (currentUpdate + 1) / updateInterval);

                Mesh meshL = audioCircle[i].GetComponent<MeshFilter>().mesh;
                Mesh meshR = audioCircle[i + (numberOfCubes / 2)].GetComponent<MeshFilter>().mesh;
                Color[] colors = new Color[meshL.uv.Length];

                for (int j = 0; j < colors.Length; j++)
                    colors[j] = Color.HSVToRGB(startHue + (hueStep * j), startSaturation, startValue);

                meshL.colors = colors;
                meshR.colors = colors;
            }
        }
        currentUpdate = (currentUpdate + 1) % updateInterval;
    }

    void instantiateCubes(GameObject prefab, string side)
    {
        for (int i = 0; i < numberOfCubes/2; i++)
        {
            GameObject createCube = (GameObject)Instantiate(prefab);
            createCube.transform.position = prefab.transform.position;
            createCube.name = "AudioCube" + side + i;
            createCube.transform.position += Vector3.forward * 0.2f * i;
            if (side == "Right")
                cubeWallRight[i] = createCube;
            else
                cubeWallLeft[i] = createCube;
        }
    }

    void makeSemiCircle(GameObject prefab)
    {
        for (int i = 1; i <= numberOfCubes/2; i++)
        {
            GameObject createCubeRight = (GameObject)Instantiate(prefab);
            GameObject createCubeLeft = (GameObject)Instantiate(prefab);

            createCubeRight.transform.position = prefab.transform.position;
            createCubeRight.transform.parent = this.transform;
            createCubeRight.name = "SemiCircleRight" + i;
            this.transform.eulerAngles = new Vector3(0, 90.0f/(numberOfCubes/2) * i, 0);
            createCubeRight.transform.position = new Vector3(distance,0,3);
            createCubeRight.transform.localScale *= cubeSize;
            audioCircle[i-1] = createCubeRight;

            createCubeLeft.transform.position = prefab.transform.position;
            createCubeLeft.transform.parent = this.transform;
            createCubeLeft.name = "SemiCircleLeft" + i;
            this.transform.eulerAngles = new Vector3(0, -90.0f / (numberOfCubes/2) * (i-1), 0);
            createCubeLeft.transform.position = new Vector3(distance,0,3);
            createCubeLeft.transform.localScale *= cubeSize;
            audioCircle[i + (numberOfCubes/2) - 1] = createCubeLeft;
        }
    }
}

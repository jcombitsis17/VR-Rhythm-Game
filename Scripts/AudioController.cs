using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
using System.IO;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using UnityEngine.Networking;

[System.Serializable]
public class Song
{
    public AudioClip clip;
    public int bpm;
    public int threeStarScore;
    public int twoStarScore;
    public int oneStarScore;

    internal int totalNotes = -1;
    internal Dictionary<float, List<string>> notes = new Dictionary<float, List<string>>();
    internal TextMesh textMesh;

    public void ProcessMidi()
    {
        totalNotes = 0;
        MidiFile midiFile;

        string beginning = (Application.platform != RuntimePlatform.Android) ? "file://" : "";
        string path = beginning + Application.streamingAssetsPath + "/" + clip.name.Replace("'", "") + ".mid";
        WWW reader = new WWW(path);
        Debug.Log(path);
        while (!reader.isDone) ;
        Debug.Log(reader.error);
        MemoryStream stream = new MemoryStream(reader.bytes);
        midiFile = MidiFile.Read(stream);

        TempoMap map = midiFile.GetTempoMap();
        foreach (Note note in midiFile.GetNotes())
        {
            float time = note.TimeAs<MetricTimeSpan>(map).TotalMicroseconds / 1000000f;
            if (!notes.ContainsKey(time))
                notes.Add(time, new List<string>());
            notes[time].Add(note.NoteName.ToString());
            totalNotes++;
        }

    }
}

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;
    private GameController gameController;
    private UIController uiController;
    private Visualization visualization;
    
    public List<Song> songList = new List<Song>();

    public Song currentSong;


    void Start()
    {
        currentSong = null;
        gameController = GetComponent<GameController>();
        audioSource = GetComponent<AudioSource>();
        uiController = GetComponent<UIController>();
        visualization = GetComponent<Visualization>();
    }

    public void StartGame(float audioDelay, int index)
    {
        currentSong = songList[index];
        audioSource.clip = currentSong.clip;
        if (currentSong.notes.Count == 0)
            currentSong.ProcessMidi();
        Debug.Log("Game Started");
        foreach (KeyValuePair<float, List<string>> entry in currentSong.notes)
        {
            float delay = entry.Key;
            List<string> n = entry.Value;
            StartCoroutine(gameController.AddBlocksToQueue(delay, n));
        }
        float secPerBeat = 0.472441f;   // for 127 bpm (Levels)
        float offsetMult = 0.25f; // Jordan: 0.75f, Anchal: 0.25f, VR: ?
        audioSource.PlayDelayed(audioDelay - secPerBeat*offsetMult);
        StartCoroutine(PlayBackgroundRipple());
    }
    
    public int EndGame(int score)
    {
        int stars = 0;
        if (score >= currentSong.threeStarScore)
            stars = 3;
        else if (score >= currentSong.twoStarScore)
            stars = 2;
        else if (score >= currentSong.oneStarScore)
            stars = 1;
        currentSong = null;
        visualization.ToggleBackgroundRipple(false);
        return stars;
    }

    public void PauseMusic()
    {
        audioSource.Stop();
    }

    public IEnumerator PlayBackgroundRipple()
    {
        yield return new WaitForSeconds(gameController.duration);
        visualization.ToggleBackgroundRipple(true, currentSong.bpm);
    }
}

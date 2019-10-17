using System.Collections;
using System.IO;
using UnityEngine;

public enum MenuType { StartMenu, InGameMenu, EndMenu };

public class UIController : MonoBehaviour
{
    public TextMesh startSongList;
    public GameObject startMenu;

    public TextMesh endScoreText;
    public TextMesh endAccuracyText;
    public Transform starScore;
    public Texture starFilled;
    public Texture starEmpty;
    public GameObject endMenu;

    public TextMesh scoreText;
    public TextMesh comboText;
    public TextMesh multiplierText;
    public GameObject inGameUI;

    public MenuType currentMenu;

    private int songListCurrentPosition;
    private AudioController audioController;
    private GameController gameController;

    private AudioVisualization audioVis;

    void Start()
    {
        audioController = GetComponent<AudioController>();
        gameController = GetComponent<GameController>();
        startMenu.SetActive(true);
        endMenu.SetActive(false);
        inGameUI.SetActive(false);

        int i = 0;
        foreach (Song song in audioController.songList)
        {
            GameObject songListEntry = Instantiate(startSongList.gameObject, startMenu.transform);
            songListEntry.transform.position = new Vector3(0, -(i - 1) * 1.5f, 10);
            songListEntry.GetComponent<TextMesh>().text += Path.GetFileNameWithoutExtension(song.clip.name);
            song.textMesh = songListEntry.GetComponent<TextMesh>();
            i++;
        }
        audioController.songList[0].textMesh.color = Color.blue;
    }

    public void UpdateSongListPosition(char direction)
    {
        if (currentMenu == MenuType.StartMenu)
        {
            if (direction == 'D')
                songListCurrentPosition = Mathf.Clamp(songListCurrentPosition + 1, 0, audioController.songList.Count - 1);
            if (direction == 'U')
                songListCurrentPosition = Mathf.Clamp(songListCurrentPosition - 1, 0, audioController.songList.Count - 1);
            foreach (Song song in audioController.songList)
            {
                TextMesh textMesh = song.textMesh;
                if (audioController.songList.IndexOf(song) == songListCurrentPosition)
                    textMesh.color = Color.blue;
                else
                    textMesh.color = Color.white;
            }
        }
    }

    public void StartMenu()
    {
        currentMenu = MenuType.StartMenu;
        audioController.PauseMusic();
        startMenu.SetActive(true);
        endMenu.SetActive(false);
        inGameUI.SetActive(false);
    }

    public void StartGame()
    {
        audioController.StartGame(gameController.duration, songListCurrentPosition);
        startMenu.SetActive(false);
        endMenu.SetActive(false);
        inGameUI.SetActive(true);
        currentMenu = MenuType.InGameMenu;
    }

    public void UpdateUI(int multiplier, int score, int combo, int accuracy)
    {
        scoreText.text = score.ToString();
        comboText.text = combo.ToString();
        multiplierText.text = multiplier.ToString() + "x";
    }

    public void EndGame(int score, int accuracy, int stars)
    {
        endScoreText.text = score.ToString();
        endAccuracyText.text = accuracy.ToString() + "%";

        startMenu.SetActive(false);
        endMenu.SetActive(true);
        inGameUI.SetActive(false);
        currentMenu = MenuType.EndMenu;

        foreach (Transform child in starScore)
        {
            if (int.Parse(child.gameObject.name) <= stars)
                child.GetComponent<Renderer>().material.mainTexture = starFilled;
            else
                child.GetComponent<Renderer>().material.mainTexture = starEmpty;
        }
    }
}

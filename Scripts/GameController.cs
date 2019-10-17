using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float spawnDistance;
    public float blockSpeed; 
    internal float duration; //how long the blocks are alive, also the offset for the music to start 
    
    private int multiplier = 1;
    private int hitsToIncreaseMultiplier = 4;

    private float lastAccuracy;
    private float sumAccuracy = 0;
    private int score = 0;
    private int combo = 0;

    private int currentBlockNumber = 0;

    public Transform activatePlane;
    public Transform optimalLine;
    public GameObject hitCircle;
    

    //map notes to blocks
    [System.Serializable]
    public class NoteBlockMap
    {
        public string note;
        public GameObject block;
    }
    public List<NoteBlockMap> map = new List<NoteBlockMap>();


    private Queue<List<Block>> blockQueue = new Queue<List<Block>>();   
    private Vector3 previousOffset = Vector3.zero; //make sure the blocks don't in the same position twice

    private AudioController audioController;
    private UIController uiController;
    private Visualization visualization;

    private float optimalBlockHitLocation;
    
    public AnimationCurve accuracyCurve;

    public IEnumerator AddBlocksToQueue(float delay, List<string> notes)//basically creates new thread to wait for some time and create the block
    {
        yield return new WaitForSeconds(delay);
        List<Block> blocks = new List<Block>();
        foreach (string note in notes)
        {
            Vector3 offset = RandomOffset();
            GameObject block = map.Single(m => m.note == note).block;
            Vector3 location = new Vector3(offset.x, offset.y, spawnDistance + optimalBlockHitLocation);
            GameObject instance = Instantiate(block, location, Quaternion.identity);
            blocks.Add(instance.GetComponent<Block>());
        }

        blockQueue.Enqueue(blocks);

        yield return new WaitForSeconds(duration - 1.0f);
        foreach (Block b in blocks)
        {
            GameObject instance = Instantiate(hitCircle);
            instance.transform.position = new Vector3(b.transform.position.x, b.transform.position.y - 0.6f, optimalBlockHitLocation);
        }
    }

    private Vector3 RandomOffset()
    {
        Vector3 offset = new Vector3(Random.Range(-2, 2), -1, 0) * 2;
        while (offset == previousOffset)
        {
            offset = new Vector3(Random.Range(-2, 2), -1, 0) * 2;
            //offset = new Vector3(Random.Range(-2, 2), Random.Range(-2, 0), 0) * 2;
        }
        previousOffset = offset;
        return offset;
    }

    public void RemoveBlockFromQueue(Block b)
    {
        blockQueue.Peek().Remove(b);
    }
    
    public void Start()
    {
        audioController = GetComponent<AudioController>();
        uiController = GetComponent<UIController>();
        visualization = GetComponent<Visualization>();
        optimalBlockHitLocation = activatePlane.position.z / 2;
        optimalLine.position = new Vector3(0, -3, optimalBlockHitLocation);
        duration = (1.0f/blockSpeed) * spawnDistance;
    }

    public void Update()
    {
        uiController.UpdateUI(multiplier, score, combo, Mathf.RoundToInt(lastAccuracy));

        if (blockQueue.Count > 0)
        {
            if (blockQueue.Peek().Count == 0)
                blockQueue.Dequeue();
        }
        else if (audioController.currentSong != null)
        {
            if (audioController.currentSong.totalNotes != -1 && currentBlockNumber >= audioController.currentSong.totalNotes)
            {
                int totalNotes = audioController.currentSong.totalNotes;
                int numStars = audioController.EndGame(score);
                uiController.EndGame(score, Mathf.RoundToInt(sumAccuracy * 100 / totalNotes), numStars);

                //**Get rid of this line eventually
                if (numStars == 0)
                    Camera.main.GetComponent<Rigidbody>().isKinematic = false;
                //***
            }
        }
    }

    public void NewDirectionReceived(char direction)
    {
        if (blockQueue.Count > 0)
            TestGesture(direction);
        else
        {
            uiController.UpdateSongListPosition(direction);

            if (direction == 'P')
            {
                if (uiController.currentMenu == MenuType.StartMenu)
                {
                    StartGame();
                    uiController.StartGame();
                    visualization.AnimateUI();
                }
                else if (uiController.currentMenu == MenuType.EndMenu)
                {
                    uiController.StartMenu();
                }
            }
        }
    }

    private void StartGame()
    {
        currentBlockNumber = 0;
        sumAccuracy = 0;
        score = 0;
        combo = 0;
        multiplier = 1;
    }

    void TestGesture(char gesture)
    {
        foreach(Block b in blockQueue.Peek())
        {
            if (b.activated)
            {
                if (b.gesture == gesture)
                {
                    BlockHit(b);
                    break;
                } else
                {
                    combo = 0;
                }
            }
        }
    }

    void BlockHit(Block b)
    {
        lastAccuracy = accuracyCurve.Evaluate(1.0f - Mathf.Abs(optimalBlockHitLocation - b.transform.position.z) / optimalBlockHitLocation);
        sumAccuracy += lastAccuracy;

        score += Mathf.RoundToInt(lastAccuracy * 100) * multiplier;
        combo++;

        if (multiplier != 8 && combo % hitsToIncreaseMultiplier == 0)
            multiplier *= 2;

        StartCoroutine(visualization.ChangeTrackColor(Mathf.RoundToInt(b.transform.position.x), lastAccuracy));
        b.DestroyBlock(lastAccuracy);
        currentBlockNumber++;
    }

    public void DestroyPlaneHit()
    {
        if (multiplier != 1)
            multiplier /= 2;
        combo = 0;
        currentBlockNumber++;
    }
}

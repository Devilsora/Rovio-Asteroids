using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ObstacleValue
{
  LRG_ASTEROID = 20,
  MED_ASTEROID = 50,
  SML_ASTEROID = 100,
  LRG_UFO = 200,
  SML_UFO = 1000
}

public class ScoreTracker : MonoBehaviour
{
  public static ScoreTracker trackerInstance;
  private PlayerController currentPlayerRef;
  
  [SerializeField] private Text scoreText;
  [SerializeField] private GameObject lifeDisplay;
  [SerializeField] private GameObject lifeSymbol;
  [SerializeField] private GameObject gameOverPanel;
  [SerializeField] private GameObject gameOverScoreText;
  [SerializeField] private GameObject optionsScreen;
  private bool isPaused = false;

  [SerializeField] private int score;
  [SerializeField] private int pointsToNextLife = 10000;

  [Header("SFX")]
  [SerializeField] private AudioSource source;
  [SerializeField] private AudioClip extraLife;
  
  //lives text/image here

  // Start is called before the first frame update
  void Start()
  {
    //make sure this is the only score tracker object in the scene
    if (trackerInstance != null)
    {
      Destroy(gameObject);
    }
    else
    {
      trackerInstance = this;
      source = GetComponent<AudioSource>();
      currentPlayerRef = GameObject.FindObjectOfType<PlayerController>();
      for (int i = 0; i < currentPlayerRef.GetLives(); i++)
      {
        //populate the life symbols into the life bar
        Instantiate(lifeSymbol, lifeDisplay.transform);
      }

      score = 0;
      scoreText.text = score.ToString();
    }
  }

  public void LoseLife()
  {
    //remove latest life object from life counter
    Destroy(lifeDisplay.transform.GetChild(lifeDisplay.transform.childCount - 1).gameObject);
    
    //change text to display final score at end
    if (currentPlayerRef.GetLives() <= 0)
    {
      gameOverPanel.SetActive(true);
      gameOverScoreText.GetComponent<Text>().text = "SCORE: " + score.ToString();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.L))
    {
      //debug tool for adding point increments for extra lives
      if (Debug.isDebugBuild)
      {
        ChangeScore(10000);
      }
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
      isPaused = !isPaused;
      optionsScreen.SetActive(isPaused);
    }
  }

  public int GetScore()
  {
    return score;
  }

  //changes score (can be positive/negative based on if points is passed in as positive/negative)
  public void ChangeScore(int points)
  {
    //change score and update displays, update "timer" to get new life
    score += points;
    pointsToNextLife -= points;
    scoreText.text = score.ToString();
    
    if (pointsToNextLife <= 0)
    {
      //check if new score made w/ positive change can give player an extra life
      currentPlayerRef.AddLives(1);

      //add new icon to player lives and play life gain sound
      Instantiate(lifeSymbol, lifeDisplay.transform);
      source.clip = extraLife;
      source.Play();

      //reset "timer" to next life
      pointsToNextLife = 10000;
    }
  }


}

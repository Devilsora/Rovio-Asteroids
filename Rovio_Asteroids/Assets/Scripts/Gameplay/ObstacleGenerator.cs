using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private Sprite[] asteroidSprites;
  [SerializeField] ScoreTracker scoreRef;

  [Header("Obstacle Settings")]
  [SerializeField] private GameObject[] asteroidObjs;
  [SerializeField] private GameObject[] UFO_Objs;
  [SerializeField] private float[] UFO_speeds;
  [SerializeField] private float[] UFO_dirChangeSpeeds;
  [SerializeField] private float[] UFO_fireSpeeds;
  [SerializeField] private bool isTitleScreen;

  [Header("Obstacle Spawning")]
  public int asteroidLimit = 26;
  public int asteroidsInPlay;
  public int UFOspawnRange = 2;
  public float asteroidSpawnTimer = 4f;
  public float UFOSpawnTimer = 5f;

  //possible values to assign obstacles spawned
  private ObstacleValue[] possibleValues =
  {
    ObstacleValue.LRG_ASTEROID, ObstacleValue.MED_ASTEROID, ObstacleValue.SML_ASTEROID, ObstacleValue.SML_UFO, ObstacleValue.LRG_UFO
  };

  //screen dimensions to figure out where to spawn asteroids
  private Vector3 screenBotLeft;
  private Vector3 screenTopRight;
  private Vector3 screenTopLeft;
  
  private Vector3 offscreenBotLeft;
  private Vector3 offscreenTopRight;


  //need to track number of asteroids on screen - only 26 can be on screen at a time - so 26 spawned in at a time

  // Start is called before the first frame update
  void Start()
  {
    //get screen data to know where to spawn stuff
    screenBotLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
    screenTopLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z));
    screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
    
    offscreenBotLeft = screenBotLeft - new Vector3(UFOspawnRange, UFOspawnRange, UFOspawnRange);
    offscreenTopRight = screenTopRight + new Vector3(UFOspawnRange, UFOspawnRange, UFOspawnRange);
  }

  // Update is called once per frame
  void Update()
  {
    //deduct timer
    if (asteroidSpawnTimer >= 0)
      asteroidSpawnTimer -= Time.deltaTime;
    else
    {
      //spawn asteroid if we're not at asteroid limit yet
      if (canSpawnAsteroid())
      {
        GenerateAsteroid();
      }

      asteroidSpawnTimer = 1f;
    }

    //if this is the title screen, don't spawn UFOs - just want asteroids to decorate
    if (!isTitleScreen)
    {
      if (UFOSpawnTimer >= 0)
        UFOSpawnTimer -= Time.deltaTime;
      else
      {
        GenerateUFO();
        UFOSpawnTimer = 3f;
      }
    }
    
  }

  //checks if we haven't hit the asteroid limit yet
  public bool canSpawnAsteroid()
  {
    if (asteroidsInPlay < asteroidLimit)
      return true;

    return false;
  }

  //incrementer to be called by asteroids
  public void IncreaseAsteroidCount()
  {
    asteroidsInPlay++;
  }

  //incrementer to be called by asteroids
  public void DecreaseAsteroidCount()
  {
    asteroidsInPlay--;
  }

  //returns a random vector2
  public static Vector2 randomVector2(float angle, float minAngle = 0)
  {
    float random = Random.value * (angle * (Mathf.PI / 180)) + minAngle;
    return new Vector2(Mathf.Cos(random), Mathf.Sin(random)).normalized;
  }

  //generates an asteroid
  public void GenerateAsteroid()
  {
    //randomize the size for the new asteroid
    int index = Random.Range(0, 2);
    ObstacleValue size = possibleValues[index];

    //get the screen dimensions to spawn from
    Vector2 spawnPos = new Vector2(Random.Range(screenBotLeft.x, screenTopRight.x), Random.Range(screenBotLeft.y, screenTopRight.y));

    //ne wasteroid to spawn in
    GameObject newAsteroid = Instantiate(asteroidObjs[index], spawnPos, Quaternion.identity);

    //set it unactive first to
    newAsteroid.SetActive(false);
    newAsteroid.GetComponent<Asteroid>().SetInfo(size, scoreRef, this, asteroidObjs);

    //create a new force to impart into the asteroid
    Vector2 newForce = randomVector2(Random.Range(0f, 360f)) * (index + 1);
    
    //increase the asteroid count and make it active into the game
    IncreaseAsteroidCount();
    newAsteroid.SetActive(true);
    newAsteroid.GetComponent<Rigidbody2D>().AddForce(newForce, ForceMode2D.Impulse);
  }

  //UFOs always come in from left or right (never the top) and start offscreen
  public void GenerateUFO()
  {
    //decide which side of the screen we start on
    bool startLeft = Random.value < 0.5f;
    Vector2 spawnPos = Vector2.zero;

    //set spawn position to be offscreen 
    if (startLeft)
      spawnPos = new Vector2(Random.Range(offscreenBotLeft.x, screenTopLeft.x), Random.Range(screenBotLeft.y, screenTopRight.y));
    else
    {
      spawnPos = new Vector2(Random.Range(screenTopRight.x, offscreenTopRight.x), Random.Range(screenBotLeft.y, screenTopRight.y));
    }

    //get the spawn index for the UFO to spawn based on the current score
    int spawnIndex = 0;
    ObstacleValue UFOval = ObstacleValue.LRG_UFO;

    if (scoreRef.GetScore() >= 10000)
    {
      spawnIndex = 1; //determines which level of UFO we spawn
      UFOval = ObstacleValue.SML_UFO;
    }

    //instantiate a UFO off screen and have it shuttle in
    GameObject newUFO = Instantiate(UFO_Objs[spawnIndex], spawnPos, Quaternion.identity);

    //set inactive first to set up info
    newUFO.SetActive(false);
    newUFO.GetComponent<UFO>().SetInfo(UFO_speeds[spawnIndex], startLeft, UFO_fireSpeeds[spawnIndex], UFOval, 3f, 250f, scoreRef);
    newUFO.SetActive(true);

    //exert force in opposite direction of the side we start on
    if (startLeft)
      newUFO.GetComponent<Rigidbody2D>().AddForce(Vector2.right * UFO_speeds[spawnIndex], ForceMode2D.Impulse);
    else
    {
      newUFO.GetComponent<Rigidbody2D>().AddForce(Vector2.left * UFO_speeds[spawnIndex], ForceMode2D.Impulse);
    }
  }

  public Sprite GetAsteroidSprite(int index)
  {
    return asteroidSprites[index];
  }


}

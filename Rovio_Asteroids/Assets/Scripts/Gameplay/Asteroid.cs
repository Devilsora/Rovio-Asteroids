using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
  //component references
  private Rigidbody2D rb;
  private PolygonCollider2D pCollider;
  [SerializeField] private ObstacleValue value;

  //asteroids to spawn on breaking (if large, spawns med and sml - if med, spawns sml - if sml, spawns none)
  private GameObject medAsteroid;
  private GameObject smlAsteroid;

  //public references to scene objects
  public ScoreTracker scoreRef;
  public ObstacleGenerator generator;

  //explosion FX object
  [SerializeField] private GameObject explosion;

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    pCollider = gameObject.AddComponent<PolygonCollider2D>(); //setting the sprite before generating the collider makes sure that we don't have mismatching colliders

  }

  void Update()
  {

  }

  //on hit, split off and create new asteroids in an area around the asteroid
  public void SplitUp()
  {
    if (value != ObstacleValue.SML_ASTEROID)
    {
      GameObject asteroidOne = null, asteroidTwo = null;
      
      //spawn medium sized asteroids if a large asteroid is being destroyed
      //make sure asteroids aren't spawning on top of each other
      if (value == ObstacleValue.LRG_ASTEROID)
      {
        //before spawning asteroids, check if we have enough room to spawn more asteroids in
        //generally should allow the first one to spawn in, but still want to account for it
        if (generator.canSpawnAsteroid())
        {
          asteroidOne = Instantiate(medAsteroid, (Vector2)transform.position + Random.insideUnitCircle, transform.rotation);
          asteroidOne.GetComponent<Asteroid>().SetInfo(ObstacleValue.MED_ASTEROID, scoreRef, generator,
            new[] { null, null, smlAsteroid });

          //tell generator about the new asteroid spawned in
          generator.IncreaseAsteroidCount();
        }

        //if enough space, spawn a second asteroid that split off
        if (generator.canSpawnAsteroid())
        {
          asteroidTwo = Instantiate(medAsteroid, (Vector2)transform.position + Random.insideUnitCircle, transform.rotation);
          asteroidTwo.GetComponent<Asteroid>().SetInfo(ObstacleValue.MED_ASTEROID, scoreRef, generator,
            new[] { null, null, smlAsteroid });

          //tell generator about the new asteroid spawned in
          generator.IncreaseAsteroidCount();
        }

      }
      //spawn small sized asteroids if a large asteroid is being destroyed
      else if (value == ObstacleValue.MED_ASTEROID)
      {
        if (generator.canSpawnAsteroid())
        {
          asteroidOne = Instantiate(smlAsteroid, (Vector2)transform.position + Random.insideUnitCircle, transform.rotation);
          asteroidOne.GetComponent<Asteroid>().SetInfo(ObstacleValue.SML_ASTEROID, scoreRef, generator,
            new GameObject[] { null, null, null });
          generator.IncreaseAsteroidCount();
        }

        if (generator.canSpawnAsteroid())
        {
          asteroidTwo = Instantiate(smlAsteroid, (Vector2)transform.position + Random.insideUnitCircle, transform.rotation);
          asteroidTwo.GetComponent<Asteroid>().SetInfo(ObstacleValue.SML_ASTEROID, scoreRef, generator,
            new GameObject[] { null, null, null });
          generator.IncreaseAsteroidCount();
        }

      }
      
      //generate vector greater than (0,0) so it's not going to spawn in paused
      float randomAngle = Random.Range(5, 355);

      //if asteroid isn't null set active and impart motion to it
      if (asteroidOne != null)
      {
        asteroidOne.SetActive(true);
        asteroidOne.GetComponent<Rigidbody2D>().AddForce(ObstacleGenerator.randomVector2(randomAngle) * 2, ForceMode2D.Impulse);
      }

      //if asteroid isn't null set active and impart motion to it
      if (asteroidTwo != null)
      {
        asteroidTwo.SetActive(true);
        randomAngle = Random.Range(5, 359);
        asteroidTwo.GetComponent<Rigidbody2D>().AddForce(ObstacleGenerator.randomVector2(randomAngle) * 2, ForceMode2D.Impulse);
      }
    }
  }

  //set asteroid info - psuedo constructor for the asteroid
  public void SetInfo(ObstacleValue newSize, ScoreTracker sRef, ObstacleGenerator generatorRef, GameObject[] asteroids)
  {

    value = newSize;
    scoreRef = sRef;
    generator = generatorRef;
    GetComponent<SpriteRenderer>().sprite = generator.GetAsteroidSprite(Random.Range(1, 4));

    //just need to take the smaller asteroids - assumes that every asteroid spawned is large - dont need the first one since it's already large
    medAsteroid = asteroids[1];
    smlAsteroid = asteroids[2];
  }

  public void OnCollisionEnter2D(Collision2D col)
  {
    if (col.gameObject.tag == "Bullet")
    {
      //decrease asteroid count and award player points
      generator.DecreaseAsteroidCount();
      int pointsValue = (int)value;
      scoreRef.ChangeScore(pointsValue);

      //if colliding with bullet, split player up
      if (value < ObstacleValue.LRG_UFO)
      {
        SplitUp();
      }

      //create explosion particle effect
      Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
      Destroy(gameObject);
    }

    //destroy self and don't split when hitting player to avoid player spawning into asteroid
    if (col.gameObject.tag == "Player")
    {
      generator.DecreaseAsteroidCount();
      Destroy(gameObject);
    }
  }
}

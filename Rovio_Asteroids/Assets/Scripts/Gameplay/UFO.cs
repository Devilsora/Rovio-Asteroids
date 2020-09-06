using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
  [Header("Laser Components")]
  [SerializeField] private GameObject laser;
  [SerializeField] private GameObject firingPoint;
  [SerializeField] private float laserSpeed;
  [SerializeField] private float laserShotTiming;
  private float shotTimingTimer;

  [Header("Movement Components")]
  private Rigidbody2D mRigidbody2D;
  [SerializeField] private float speed;
  [SerializeField] private bool startLeftOrRight; //starts on left if true
  [SerializeField] private ObjectWrap wrapper;
  private bool canChangeDirection;

  private float timeUntilDirectionChange;
  private float directionTimer;

  [Header("References")]
  private PlayerController playerRef;
  private ScoreTracker scoreRef;
  private ObstacleValue obstacleVal;
  [SerializeField] private GameObject explosion;
  [SerializeField] private AudioSource source;
  [SerializeField] private AudioSource shotSource;
  [SerializeField] private AudioClip shotSFX;

  
  void Start()
  {
    //get references to components or other objects
    wrapper = GetComponent<ObjectWrap>();
    source = GetComponent<AudioSource>();
    playerRef = FindObjectOfType<PlayerController>();
    mRigidbody2D = GetComponent<Rigidbody2D>();

    //setup audio for the lasers
    shotSource = firingPoint.GetComponent<AudioSource>();
    shotSource.clip = shotSFX;

    //disable object wrapper (don't want this wrapping at the start)
    wrapper.enabled = false;

  }

  // Update is called once per frame
  private void Update()
  {
    //make sure it's actually somewhere on the screen before changing direction
    if (!canChangeDirection)
    {
      if ((transform.position.x < 4 || transform.position.x > -4) &&
          (transform.position.y < 4 || transform.position.y > -4))
      {
        //when we can change direction, want to make sure UFO screen wraps properly
        canChangeDirection = true;
        wrapper.enabled = true;
      }
    }

    //change direction once timer is up
    directionTimer += Time.deltaTime;

    if (directionTimer >= timeUntilDirectionChange)
    {
      ChangeDirection();
      directionTimer = 0.0f;
    }

    //tick down shot timer
    shotTimingTimer += Time.deltaTime;

    //dumber A.I. - fires randomly and not aiming for the player
    if (obstacleVal == ObstacleValue.LRG_UFO)
    {
      if (shotTimingTimer >= laserShotTiming && canChangeDirection) //if it can change direction, we're on screen
      {
        //random firing
        float randAngle = Random.Range(0, 359);
        Vector2 firingAngle = ObstacleGenerator.randomVector2(randAngle);
        FireLaser(firingAngle);

        shotTimingTimer = 0f;
      }

    }
    //aggressive A.I. to hit the player more - shoots slightly in front or slightly behind the player
    else if (obstacleVal == ObstacleValue.SML_UFO)
    {
      //tracking player position and firing
      
      //predict where player is moving to 
      //make sure if we can change direction, since it means we're not shooting from offscreen
      if (shotTimingTimer >= laserShotTiming && canChangeDirection)
      {
        Vector2 firingAngle = predictedPosition(playerRef.GetPlayerPosition(), transform.position, playerRef.GetPlayerVelocity(), laserSpeed);
        Debug.Log("Firing angle: " + firingAngle);
        FireLaser(firingAngle);

        shotTimingTimer = 0f;
      }
    }
  }

  //fires a laser at the specified angle with laserspeed
  public void FireLaser(Vector2 angle)
  {
    shotSource.Play();
    GameObject spawnedLaser = Instantiate(laser, firingPoint.transform.position, Quaternion.identity);
    spawnedLaser.GetComponent<UFO_Laser>().AddVelocity(angle.normalized, laserSpeed);
  }

  //predicts the vector to shoot the bullet at for the more aggressive A.I.
  private Vector2 predictedPosition(Vector3 targetPosition, Vector3 shooterPosition, Vector3 targetVelocity, float projectileSpeed)
  {
    //get distance from player and the angle they're moving at
    Vector3 displacement = targetPosition - shooterPosition;
    float targetMoveAngle = Vector3.Angle(-displacement, targetVelocity) * Mathf.Deg2Rad;

    //if the target is stopped or if it is impossible for the projectile to catch up with the target (Sine Formula)
    if (targetVelocity.magnitude == 0.0f || targetVelocity.magnitude > projectileSpeed && Mathf.Sin(targetMoveAngle) / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude)
    {
      //can't figure out the angle if the player isn't moving, so default to returning the displacement vector
      return displacement;
    }

    //if we CAN figure out the angle, solve and return back the angle
    float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetVelocity.magnitude / projectileSpeed);
    return targetPosition + targetVelocity * displacement.magnitude / Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) * Mathf.Sin(shootAngle) / targetVelocity.magnitude;
  }

  public void ChangeDirection()
  {
    //easy AI can only move in the same direction, difficult AI can move either way
    float randomAngle;

    if (obstacleVal == ObstacleValue.LRG_UFO)
    {
      //only moves in the direction it started in
      if (startLeftOrRight)
      {
        //only moves left
        randomAngle = Random.Range(100, 260);
      }
      else
      {
        //only moves right
        randomAngle = Random.Range(80, -80);
      }
    }
    else
    {
      randomAngle = Random.Range(0, 359);
    }

    //set new direction and move in that new direction
    Vector2 newDirection = ObstacleGenerator.randomVector2(randomAngle);
    mRigidbody2D.AddForce(newDirection * speed, ForceMode2D.Impulse);
  }


  //psuedo constructor that sets all the info and references for the object
  //used by obstacle generator
  public void SetInfo(float speed, bool leftOrRight, float dirChangeTime, ObstacleValue val, float shotTiming, float shotSpeed, ScoreTracker sRef)
  {
    this.speed = speed;
    startLeftOrRight = leftOrRight;
    timeUntilDirectionChange = dirChangeTime;
    obstacleVal = val;
    laserShotTiming = shotTiming;
    laserSpeed = shotSpeed;
    scoreRef = sRef;
  }

  public void OnCollisionEnter2D(Collision2D col)
  {
    if (col.gameObject.tag == "Bullet")
    {
      //add point value to score tracker
      int pointsValue = (int)obstacleVal;
      scoreRef.ChangeScore(pointsValue);

      Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
      Destroy(gameObject);
    }

    if (col.gameObject.tag == "Player")
    {
      //destroys self so that when player respawns, they don't immediately run back into the obstacle
      Destroy(gameObject);
    }
  }
}

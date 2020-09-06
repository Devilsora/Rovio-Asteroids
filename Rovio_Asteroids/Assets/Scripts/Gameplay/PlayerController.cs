using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{

  //reference to score tracker
  public ScoreTracker scoreRef;

  [Header("Movement Info")]
  [SerializeField] private float moveSpeed;
  [SerializeField] private float rotateSpeed;
  private Rigidbody2D mRigidBody;

  //input from player
  private float vertInput;
  private float horizInput;

  [Header("Bullet Info")]
  [SerializeField] private float bulletSpeed;
  [SerializeField] private GameObject shotPoint;
  [SerializeField] public GameObject bulletPrefab;

  [Header(("Player States"))]
  [SerializeField] private int lives;
  private PolygonCollider2D col;
  private bool isInvincible;
  private bool midExplosion;

  [Header("FX")]
  [SerializeField] private GameObject thrustFX;
  [SerializeField] private AnimationClip explosionFX;

  [Header("SFX")]
  private AudioSource source;
  private AudioSource shotAudio;
  private AudioSource thrustAudio;
  [SerializeField] private AudioClip explosionSFX;
  [SerializeField] private AudioClip thrustSFX;
  [SerializeField] private AudioClip shotSFX;

  private Animator animator;

  void Start()
  {
    //assign all references
    col = GetComponent<PolygonCollider2D>();
    mRigidBody = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    source = GetComponent<AudioSource>();
    shotAudio = shotPoint.GetComponent<AudioSource>();
    shotAudio.clip = shotSFX;

    //assign audio to child objects
    thrustAudio = thrustFX.GetComponent<AudioSource>();
    thrustAudio.clip = thrustSFX;
    lives = 3;
  }

  // Update is called once per frame
  void Update()
  {
    //make sure player can't shoot while they're dying
    if (!midExplosion)
    {
      //handle player input
      MovementHandler();
      ShootingManager();
    }
    
  }

  void FixedUpdate()
  {
    mRigidBody.AddRelativeForce(Vector2.up * vertInput * moveSpeed * Time.deltaTime);
    transform.Rotate(-horizInput * rotateSpeed * Vector3.forward);  //using transform.rotate vs. rigidbody.addtorque since it's smoother
  }

  void MovementHandler()
  {
    vertInput = Input.GetAxis("Vertical");
    horizInput = Input.GetAxis("Horizontal");

    //make sure player can't add input while exploding
    if (!midExplosion)
    {
      //if moving at all, enable thrust effects
      if (vertInput != 0 || horizInput != 0)
      {
        thrustFX.SetActive(true);
      }
      else
      {
        thrustFX.SetActive(false);
      }
    }
    
    //debug tool for invincibility
    if (Debug.isDebugBuild)
      if (Input.GetKeyDown(KeyCode.I)) col.enabled = !col.enabled;

  }

  public Vector2 GetPlayerPosition()
  {
    return transform.position;
  }

  public Vector2 GetPlayerVelocity()
  {
    return mRigidBody.velocity;
  }

  //handle player shooting
  void ShootingManager()
  {
    if (Input.GetKeyDown(KeyCode.Space) && !isInvincible)
    {
      //shoots in direction of player nosecone
      GameObject newBullet = Instantiate(bulletPrefab, shotPoint.transform.position, shotPoint.transform.rotation);
      newBullet.GetComponent<Laser>().AddVelocity(Vector2.up, bulletSpeed);
      shotAudio.Play();
    }
  }

  void OnCollisionEnter2D(Collision2D col)
  {
    if (col.gameObject.tag == "Asteroid" || col.gameObject.tag == "UFO_Bullet" || col.gameObject.tag == "UFO")
    {
      //freeze current player movement and die if hit by an enemy object
      if (!isInvincible)
      {
        mRigidBody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        //determine if we've lost our last life or if we still have lives remaining
        if (lives <= 0)
        {
          PlayExplosion();
          StartCoroutine(PlayerDeath());
        }
        else
        {
          PlayExplosion();
          StartCoroutine(LoseLife());
        }
      }
      
    }
  }

  public void PlayExplosion()
  {
    animator.SetBool("PlayerHit", true);  //moves animator into player hit state
    source.clip = explosionSFX;
    midExplosion = true;

    //switch audio source to play explosion sound
    source.Play();
  }

  public void AddLives(int amount)
  {
    lives += amount;
  }

  public int GetLives()
  {
    return lives;
  }

  IEnumerator PlayerDeath()
  {
    yield return new WaitForSeconds(0.1f);
    Destroy(gameObject);
    //turns on lose screen in score tracker
  }

  IEnumerator LoseLife()
  {
    //turn on player invincibility
    isInvincible = true;
    lives--;
    thrustFX.SetActive(false);

    //turn off player collider and send message to score tracker to deduct life
    scoreRef.SendMessage("LoseLife");
    col.enabled = false;
    //play audio clip

    //wait for explosion animation to finish
    yield return new WaitForSeconds(2f);
    animator.SetBool("PlayerHit", false);

    //reset player position to center of screen, and reset velocity, and rotation 
    transform.position = new Vector2(0, 0);
    mRigidBody.velocity = Vector2.zero;
    mRigidBody.constraints = RigidbodyConstraints2D.None;
    transform.rotation = Quaternion.identity;


    //turn off player invincibility and reenable colliders
    isInvincible = false;
    midExplosion = false;
    col.enabled = true;

  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
      Debug.Log("in bullet awake");
      rb = GetComponent<Rigidbody2D>();
    }

    public void AddVelocity(Vector2 dir, float spd)
    {
      rb.AddRelativeForce(dir * spd);
    }

    public void OnBecameInvisible()
    {
      //make sure bullet gets destroyed going offscreen
      Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
      if (col.gameObject.tag == "Asteroid" || col.gameObject.tag == "UFO")
      {
        Destroy(gameObject);
      }
    }
}

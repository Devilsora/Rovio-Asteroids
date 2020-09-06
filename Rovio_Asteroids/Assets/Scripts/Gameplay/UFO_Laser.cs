using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//very much like the regular laser but for UFOs
//probably could have made laser a class and have different tag array of targets it works on
public class UFO_Laser : MonoBehaviour
{
  private Rigidbody2D rb;

  
  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  public void AddVelocity(Vector2 dir, float spd)
  {
    rb.AddRelativeForce(dir * spd);
    transform.up = dir;
  }

  public void OnBecameInvisible()
  {
    Destroy(gameObject);
  }

  public void OnCollisionEnter2D(Collision2D col)
  {
    if (col.gameObject.tag == "Player")
    {
      Destroy(gameObject);
    }
  }
}

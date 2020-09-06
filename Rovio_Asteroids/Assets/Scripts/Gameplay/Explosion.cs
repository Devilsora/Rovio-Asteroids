using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
  private AudioSource source;
  [SerializeField] private AudioClip clip;
  [SerializeField] private ParticleSystem ps;
  
    
    void Start()
    {
      source = GetComponent<AudioSource>();
      source.clip = clip;
      source.Play();
      ps = GetComponent<ParticleSystem>();
    }

    
    void Update()
    {
      //destroy object once audio clip is finished
      if (!source.isPlaying)
        Destroy(gameObject);
    }
}

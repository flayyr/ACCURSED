using System.Collections.Generic;
using UnityEngine;

public class ContactParticles : MonoBehaviour
{
    [Tooltip("Place all particles to play on entering contact with an entity")][SerializeField] List<ParticleSystem> playOnEnter;
    [Tooltip("Place all particles to play on exiting contact with an entity")][SerializeField] List<ParticleSystem> playOnExit;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (ParticleSystem x in playOnEnter)
            {
                x.Play();
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (ParticleSystem x in playOnExit)
            {
                x.Play();
            }
        }
    }
}

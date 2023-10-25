using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public ParticleSystem torpedoParticle;
    public List<ParticleCollisionEvent> collisionEvents;
    private TorpedoController tc;

    // Start is called before the first frame update
    void Start()
    {
        tc = GetComponentInParent<TorpedoController>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("OnParticleCollision entered with " + other.name);
        int numEvents = torpedoParticle.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numEvents; ++i)
        {
            Debug.Log("torpedo hit "+other.name+ " at " + collisionEvents[i].intersection);
            Instantiate(tc.explosionParticle, collisionEvents[i].intersection, Quaternion.identity);
            collisionEvents[i].colliderComponent.gameObject.GetComponentInParent<DamageManager>().TakeDamage(tc.damageAmount);
        }
    }

    private void OnParticleSystemStopped()
    {
        tc.torpedosPlaying = false;
    }
}

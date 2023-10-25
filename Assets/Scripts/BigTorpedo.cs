using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTorpedo : MonoBehaviour
{
    public GameObject target;

    public float speed = 15f;
    public float rotationSpeed = 3f;
    public float lifetimeSecs = 10f;

    public float explosionRadius = 6f;
    public float damageAmount = 2500f;

    private Quaternion lookRotation;
    private bool guidanceActive = false;
    private bool isExploded = false;
    private float distanceToTarget;
    private float lifetimeTimer = 0;

    public ParticleSystem torpedoParticle;
    public ParticleSystem explosionParticle;
    public AudioSource explosionAudio;
    private TrailRenderer trail;


    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetGuidanceActive", 1f);
        trail = GetComponentInChildren<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            Destroy(gameObject);
        if (!guidanceActive)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed); 
        } else
        {
            FlyTowards(target.transform.position);
        }
        distanceToTarget = (target.transform.position - transform.position).magnitude;
        if (!isExploded && (distanceToTarget < explosionRadius || lifetimeTimer >= lifetimeSecs))
        {
            isExploded = true;
            StartCoroutine(ExplosionEffect());
            ExplosionDamage();
        }
        lifetimeTimer += Time.deltaTime;
    }

    void FlyTowards(Vector3 position)
    {
        lookRotation = Quaternion.LookRotation(position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    void ExplosionDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.gameObject.GetComponentInParent<DamageManager>().TakeDamage(damageAmount);
        }
    }

    void SetGuidanceActive()
    {
        guidanceActive = true;
    }

    IEnumerator ExplosionEffect()
    {
        explosionParticle.Play();
        explosionAudio.Play();
        torpedoParticle.Stop();
        trail.enabled = false;
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }    
}

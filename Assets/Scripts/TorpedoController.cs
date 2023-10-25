using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoController : MonoBehaviour
{
    public ParticleSystem torpedoParticle;
    public ParticleSystem explosionParticle;
    public LayerMask layer;

    public GameObject player;
    public GameObject nearestTarget;
    public GameObject target;
    public GameObject laserObject;
    public SpawnManager spawnManager;
    public string targetTag = "Player";
    public float damageAmount = 500;
    public float firingAngleFromNormal = 25.0f;
    public float firingRange = 50.0f;
    public float fireCooldownSecs = 8.0f;
    public int salvoSize = 4;
    
    private float cooldownTimer = 0;
    private AudioSource fireAudio;
    public bool torpedosPlaying = false;
    private RaycastHit hit;

    void Start()
    {
        player = GameObject.Find("Player");
        nearestTarget = player;
        spawnManager = FindObjectOfType<SpawnManager>();
        fireAudio = torpedoParticle.GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        if (spawnManager.enemyCount > 0)
        {
            nearestTarget = GameObject.FindGameObjectWithTag("Player");
            if (transform.parent.CompareTag("Player"))
            {
                //nearestTarget = spawnManager.nearestEnemy.gameObject;
                targetTag = "Enemy";
                foreach (Enemy enemy in spawnManager.inRangeSet)
                {
                    if (!enemy.isTractored)
                    {
                        Vector3 vectorToTarget = enemy.transform.position - transform.position;
                        if (Vector3.Angle(transform.up, vectorToTarget) <= firingAngleFromNormal)
                        {
                            nearestTarget = enemy.gameObject;
                            break;
                        }
                    }
                }
            }
            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                Vector3 vectorToTarget = nearestTarget.transform.position - transform.position;
                float distanceToTarget = vectorToTarget.magnitude;
                if (distanceToTarget < (firingRange + spawnManager.playerRadius) && Vector3.Angle(transform.up, vectorToTarget) <= firingAngleFromNormal) //in firing arc
                {
                    for (int i = 0; i < salvoSize; ++i)
                    {
                        StartCoroutine(FireBurst(nearestTarget.transform.position));
                    }
                    cooldownTimer = fireCooldownSecs;
                }
            }
        }
        cooldownTimer -= Time.deltaTime;
    }

    void Fire(Vector3 targetPosition)
    {
        var ray = new Ray(transform.position, (targetPosition - transform.position).normalized);
        if (Physics.Raycast(ray, out hit, firingRange + spawnManager.playerRadius, layer))
        {
            if (!hit.transform.gameObject.CompareTag(targetTag))
                return;
            torpedoParticle.transform.LookAt(targetPosition, transform.up);
            torpedoParticle.Play();
            torpedosPlaying = true;
            fireAudio.Play();               
        }
    }

    IEnumerator FireBurst(Vector3 targetPosition)
    {
        
        float delay = Random.Range(0.2f, 0.4f);
        yield return new WaitForSeconds(delay);
        Fire(targetPosition);
            
    }
}

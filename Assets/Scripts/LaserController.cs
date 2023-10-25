using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public ParticleSystem laserStartParticles;
    public ParticleSystem laserEndParticles;
    public float lineLength = 30.0f;
    public LayerMask layer;

    public GameObject player;
    public GameObject nearestTarget;
    public GameObject target;
    public GameObject laserObject;
    public SpawnManager spawnManager;
    public string targetTag = "Player";
    public float damageAmount = 5;
    public float firingAngleFromNormal = 87.0f;
    public float firingRange = 30.0f;
    public float fireCooldownSecs = 3.0f;
    public float fireDurationSecs = 3.0f;
    public float jitterMagnitude = 1.0f;
    public bool isFiring;
    public bool isPlayerLaser;

    private float fireTimer;
    private float cooldownTimer;
    private Vector3 savedJitter;
    public AudioSource fireAudio;
    private LineRenderer line;
    private bool sfxIsPlaying = false;
    private bool startParticlesPlaying = false;
    private bool endParticlesPlaying = false;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        nearestTarget = player;
        spawnManager = FindObjectOfType<SpawnManager>();
        line = laserObject.GetComponent<LineRenderer>();
        fireAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
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
                    } else
                    {
                        StopFire();
                    }
                }
            }
            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                Vector3 vectorToTarget = nearestTarget.transform.position - transform.position;
                float distanceToTarget = vectorToTarget.magnitude;
                if (fireTimer < fireDurationSecs && distanceToTarget < (firingRange+spawnManager.playerRadius) && Vector3.Angle(transform.up, vectorToTarget) <= firingAngleFromNormal) //in firing arc
                {
                    if (!isFiring) //generate jitter if start of fire
                    {
                        savedJitter = getRandomTargetingJitter(jitterMagnitude);
                        fireTimer = 0;
                    }
                    Fire(nearestTarget.transform.position + savedJitter);
                    fireTimer += Time.deltaTime;
                }
                else //out of range or out of arc
                {
                    StopFire();
                    cooldownTimer = fireCooldownSecs;
                    fireTimer = 0;
                }
            }
        }
        cooldownTimer -= Time.deltaTime;
    }

    void Fire(Vector3 targetPosition)
    {
        var ray = new Ray(transform.position, (targetPosition - transform.position).normalized);
        if(Physics.Raycast(ray, out hit, lineLength, layer))
        {
            if (!hit.transform.gameObject.CompareTag(targetTag))
            {
                StopFire();
                return;
            }
            isFiring = true;
            line.enabled = true;
            line.SetPosition(1, hit.point);
            line.SetPosition(0, transform.position);
            laserEndParticles.transform.position = hit.point;
            if (!startParticlesPlaying)
            {
                laserStartParticles.Play(true);
                startParticlesPlaying = true;
            }
            if (!endParticlesPlaying)
            {
                laserEndParticles.Play(true);
                endParticlesPlaying = true;
            }
            if (!sfxIsPlaying)
            {
                fireAudio.volume = 1;
                fireAudio.Play();
                sfxIsPlaying = true;
            }
            hit.collider.gameObject.GetComponentInParent<DamageManager>().TakeDamage(damageAmount);
        }
    }

    void StopFire()
    {
        line.enabled = false;
        startParticlesPlaying = false;
        laserStartParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        endParticlesPlaying = false;
        laserEndParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        sfxIsPlaying = false;
        StartCoroutine(FadeAudioSource.StartFade(fireAudio, 0.5f, 0f));
        //fireAudio.Stop();
        isFiring = false;
    }

    Vector3 getRandomTargetingJitter(float magnitude)
    {
        return new Vector3(Random.value, Random.value, Random.value) * magnitude;
    }
}

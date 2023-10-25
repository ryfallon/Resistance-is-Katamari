using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTorpedoLauncher : MonoBehaviour
{
    public BigTorpedo torpedoPrefab;
    public GameObject player;
    public GameObject nearestTarget;
    public GameObject target;
    public SpawnManager spawnManager;
    public string targetTag = "Player";

    public float firingRange = 50.0f;
    public float fireCooldownSecs = 12.0f;

    private float cooldownTimer = 0;
    private AudioSource fireAudio;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        nearestTarget = player;
        spawnManager = FindObjectOfType<SpawnManager>();
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
                nearestTarget = spawnManager.nearestEnemy.gameObject;
                targetTag = "Enemy";
                foreach (Enemy enemy in spawnManager.inRangeSet)
                {
                    if (!enemy.isTractored)
                    {
                        nearestTarget = enemy.gameObject;
                        break;
                    }
                }
            }
            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                Vector3 vectorToTarget = nearestTarget.transform.position - transform.position;
                float distanceToTarget = vectorToTarget.magnitude;
                if (distanceToTarget < (firingRange + spawnManager.playerRadius)) //in range
                {
                    Fire(nearestTarget);
                    cooldownTimer = fireCooldownSecs;
                }
            }
        }
        cooldownTimer -= Time.deltaTime;
    }

    void Fire(GameObject target)
    {
        BigTorpedo torpedo = Instantiate(torpedoPrefab, transform.position, transform.rotation);
        torpedo.target = target;
        fireAudio.Play();
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeam : MonoBehaviour
{
    public SpawnManager spawnManager;
    public Enemy nearestTarget;
    public bool isActive = false;
    public bool isPlaying = false;
    public float range = 20f;
    public float pullSpeed = 2f;
    public ParticleSystem tractorEffect;
    public AudioSource fireAudio;

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
        tractorEffect = GetComponentInChildren<ParticleSystem>();
        fireAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float nearestDistance = 999f;
        if (!isActive)
        {
            foreach (Enemy enemy in spawnManager.inRangeSet)
            {
                if (enemy.shield.currentHitPoints == 0 && enemy.distanceToPlayer < nearestDistance)
                {
                    nearestTarget = enemy;
                    nearestDistance = enemy.distanceToPlayer;
                }
            }
        }
        if (nearestTarget != null)
        {
            if (!nearestTarget.isAssimilated && nearestTarget.distanceToPlayer <= range)
            {
                TractorIn(nearestTarget);
            }
            else if (isPlaying)
            {
                StopTractor();
                nearestTarget.isTractored = false;
            }
        } else if (isPlaying)
        {
            StopTractor();
        }
    }

    void TractorIn(Enemy enemy)
    {
        transform.LookAt(enemy.transform);
        if (!isPlaying)
        {
            Debug.Log("tractor started playing from !isPlaying");
            fireAudio.volume = 1;
            tractorEffect.Play();
            fireAudio.Play();
            isPlaying = true;
        }
        if (!enemy.isTractored)
        {
            enemy.isTractored = true;
            enemy.enemyRb.isKinematic = true;
            isActive = true;
        }
        enemy.transform.Translate((transform.position - enemy.transform.position).normalized * pullSpeed * Time.deltaTime);
    }

    void StopTractor()
    {
        Debug.Log("tractor stopped");
        tractorEffect.Stop();
        isPlaying = false;
        isActive = false;
        StartCoroutine(FadeAudioSource.StartFade(fireAudio, 0.5f, 0f));
    }
}

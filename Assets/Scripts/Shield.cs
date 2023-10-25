using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    public float currentHitPoints;
    public float maxHitPoints = 20000.0f;
    public float regenSpeed = 10.0f;
    public float regenCooldownSecs = 2.0f;
    public float regenTimer;
    public float bounceForce = 20.0f;
    public float bounceDamage = 50;

    public float currentMaxAlpha = 1.0f;
    public float fadeInSecs = 1.0f;
    public float fadeOutSecs = 2.0f;
    public float scaleFactor;

    public bool isActive = true;

    private Material material;
    private Collider shieldCollider;
    public Rigidbody parentRb;
    public Slider shieldSlider;
    public AudioSource hitAudio;
    private float currentAlpha = 0.0f;
    private bool isInFadeIn = false;
    private bool isInFadeOut = false;
    private bool sfxIsPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHitPoints = maxHitPoints;
        material = GetComponent<Renderer>().material;
        shieldCollider = GetComponent<Collider>();
        hitAudio = GetComponent<AudioSource>();
        UpdateHealth();
        UpdateMaterialAlpha(currentAlpha);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (regenTimer <= 0 && currentHitPoints < maxHitPoints)
            {
                sfxIsPlaying = false;
                currentHitPoints += regenSpeed;
                UpdateHealth();
            }
            if (!isInFadeIn && !isInFadeOut && currentAlpha > 0)
            {
                StartCoroutine(FadeOutObject());
            }
            if (regenTimer > 0)
                regenTimer -= Time.deltaTime;
        } else
        {
            UpdateHealth();
            UpdateMaterialAlpha(currentAlpha);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.gameObject.CompareTag("Shield"))
        {
            Vector3 awayVector = (other.gameObject.transform.position - transform.position).normalized;
            Rigidbody collisionRb = other.attachedRigidbody;
            //Debug.Log(this.ToString() + " hit rb " + collisionRb.ToString());
            if (collisionRb != null)
                collisionRb.AddForce(awayVector * bounceForce, ForceMode.Impulse);
            parentRb.AddForce(-awayVector * bounceForce, ForceMode.Impulse);
            other.gameObject.GetComponentInParent<DamageManager>().TakeDamage(bounceDamage);
            TakeDamage(bounceDamage);
        }
    }

    void UpdateHealth()
    {
        currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);
        currentMaxAlpha = currentHitPoints / maxHitPoints;
        if (shieldSlider != null)
            shieldSlider.value = currentMaxAlpha;
        if (currentHitPoints <= 0)
        {
            shieldCollider.enabled = false;
        } else
        {
            shieldCollider.enabled = true;
        }
    }

    void UpdateMaterialAlpha(float value)
    {
        if (!float.IsNaN(value))
            material.SetFloat("_MasterAlpha", Mathf.Clamp(value, 0.0f, 1.0f));
    }

    public void TakeDamage(float damageAmount)
    {
        if (isActive)
        {
            regenTimer = regenCooldownSecs;
            if (currentHitPoints > 0)
            {
                currentHitPoints -= damageAmount;
                UpdateHealth();
                if (!isInFadeIn)
                {
                    StartCoroutine(FadeInObject());
                }
                if (hitAudio != null && !sfxIsPlaying)
                {
                    sfxIsPlaying = true;
                    hitAudio.Play();
                }
            }
        } else
        {
            UpdateHealth();
            UpdateMaterialAlpha(currentAlpha);
        }
    }

    public IEnumerator FadeInObject()
    {
        float fadeInTimer = (currentAlpha / currentMaxAlpha) * fadeInSecs;
        while (fadeInTimer < fadeInSecs)
        {
            isInFadeIn = true;
            fadeInTimer += Time.deltaTime;
            currentAlpha = currentMaxAlpha * (fadeInTimer / fadeInSecs);
            UpdateMaterialAlpha(currentAlpha);
            yield return null;
        }
        isInFadeIn = false;
        
    }

    public IEnumerator FadeOutObject()
    {
        float fadeOutTimer = (currentAlpha / currentMaxAlpha) * fadeOutSecs;
        while (!isInFadeIn && fadeOutTimer > 0.0f)
        {
            isInFadeOut = true;
            fadeOutTimer -= Time.deltaTime;
            currentAlpha = currentMaxAlpha * (fadeOutTimer / fadeOutSecs);
            UpdateMaterialAlpha(currentAlpha);
            yield return null;
        }
        isInFadeOut = false;
    }
}

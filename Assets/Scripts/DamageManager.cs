using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageManager : MonoBehaviour
{
    public Shield shield;
    public PlayerController player;
    public Enemy enemy;
    public Slider healthSlider;
    private RectTransform sliderRect;

    public float currentHitPoints;
    public float maxHitPoints = 10000.0f;
    public float damageEffectThreshold = 0.2f;
    public float repairRate = 0;
    public ParticleSystem fireEffect;
    public AudioSource fireSfx;
    public DestructionManager dm;
    private bool isEffectPlaying = false;
    private bool isInDestroyAnimation = false;
    public float shrunkWidth = 40f;

    // Start is called before the first frame update
    void Start()
    {
        currentHitPoints = maxHitPoints;
        sliderRect = healthSlider.GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (enemy != null && enemy.isAssimilated && sliderRect.rect.width > shrunkWidth)
        {
            sliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, shrunkWidth);
        }
        if (currentHitPoints < maxHitPoints)
            currentHitPoints += repairRate * Time.deltaTime;
        healthSlider.value = currentHitPoints / maxHitPoints;
    }

    public void TakeDamage(float damageAmount)
    {
        if (shield.currentHitPoints > 0)
        {
            shield.TakeDamage(damageAmount);
        } else if (currentHitPoints > 0)
        {
            if (!isEffectPlaying && (currentHitPoints/maxHitPoints) < damageEffectThreshold)
            {
                fireEffect.Play(true);
                fireSfx.Play();
            }
            currentHitPoints -= damageAmount;
            healthSlider.value = currentHitPoints / maxHitPoints;
        }
        else
        {
            if (!isInDestroyAnimation)
            {
                FindObjectOfType<SpawnManager>().destroyedCount++;
                StartCoroutine(DestructionEffect());
            }
        }
    }

    void disableMeshRenderers()
    {
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false; 
        }
    }

    IEnumerator DestructionEffect()
    {
        isInDestroyAnimation = true;
        healthSlider.enabled = false;
        if (enemy != null)
        {
            enemy.enemyRb.drag = 0;
            enemy.isInDestroyAnimation = true;
        }
        if (player != null)
            player.isInDestroyAnimation = true;
        yield return new WaitForSeconds(2);
        dm.Play();
        yield return new WaitForSeconds(2);
        fireEffect.Stop();
        disableMeshRenderers();
        yield return new WaitForSeconds(2);
        Destroy(healthSlider);
        Destroy(gameObject);
    }
}

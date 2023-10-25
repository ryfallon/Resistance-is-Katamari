using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public float speed = 4.0f;
    public float thrustForce = 20.0f;
    public float maneuverForce = 5.0f;
    public float rotationSpeed = 1.0f;
    public float orbitDistance = 10.0f;
    public float distanceToPlayer;
    public float yLimit = 10.0f;
    public string shiptype;

    public Rigidbody enemyRb;
    private PlayerController player;
    public Shield shield;

    private Vector3 lookDirection;
    private Quaternion lookRotation;

    public Material sharedMaterial;
    public GameObject model;
    private Renderer[] modelRenderers;
    private TextMeshProUGUI displayText;

    FixedJoint joint;
    public bool useOrbit = true;
    public bool isAssimilated = false;
    public bool isInDestroyAnimation = false;
    public bool isTractored = false;


    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>();
        modelRenderers = model.GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isAssimilated)
        {
            distanceToPlayer = (player.transform.position - transform.position).magnitude;
            if (transform.position.y > yLimit)
            {
                enemyRb.AddForce(Vector3.down * maneuverForce);
            } else if (transform.position.y < -yLimit)
            {
                enemyRb.AddForce(Vector3.up * maneuverForce);
            }
        }
        if (!isTractored && !isAssimilated && enemyRb.isKinematic)
        {
            enemyRb.isKinematic = false;
            enemyRb.useGravity = false;
        }
        if (!isAssimilated && !isInDestroyAnimation && !isTractored)
        {
            shield.isActive = true;
            if (distanceToPlayer <= (orbitDistance + player.radius))
            {
                if (useOrbit)
                    Orbit(player);
            } else if (shield.currentHitPoints < (shield.maxHitPoints/4))
            {
                Retreat(player);
            } else
            {
                Chase(player);
            }
        } else
        {
            //shield.regenSpeed = 0;
            //shield.currentHitPoints = 0;
            shield.isActive = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isAssimilated && collision.gameObject.CompareTag("Player"))
        {
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collision.rigidbody;
            enemyRb.isKinematic = false;
            enemyRb.drag = 0;
            isAssimilated = true;
            player.AddToAssimilated(enemyRb.mass);
            gameObject.tag = "Player";
            gameObject.layer = 6; //ignore collisions with mesh
            foreach (Renderer renderer in modelRenderers)
            {
                renderer.material = sharedMaterial;
            }
        }
    }

    void FlyTowards(Vector3 heading)
    {
        lookRotation = Quaternion.LookRotation(heading);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        //transform.Translate(Vector3.forward * Time.deltaTime * speed);
        enemyRb.AddRelativeForce(Vector3.forward * thrustForce);
        enemyRb.AddRelativeForce(lookRotation.eulerAngles.normalized * maneuverForce);
    }

    void Chase(PlayerController target)
    {
        lookDirection = Vector3.Scale((target.transform.position - transform.position).normalized, new Vector3(1f,1f,0.5f));
        FlyTowards(lookDirection);
    }

    void Orbit(PlayerController target)
    {
        lookDirection = Vector3.Cross((target.transform.position - transform.position).normalized, Vector3.right);
        FlyTowards(lookDirection);
    }

    void Retreat(PlayerController target)
    {
        lookDirection = Vector3.Cross((transform.position - target.transform.position).normalized, Vector3.one - Vector3.up);
        FlyTowards(lookDirection);
    }

}

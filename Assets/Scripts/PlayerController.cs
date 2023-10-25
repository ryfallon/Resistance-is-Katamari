using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject mainCamera;
    public GameObject playerLaser;
    public SpawnManager spawnManager;
    public float speed = 25.0f;
    public bool isInDestroyAnimation = false;
    public int numberAssimilated = 0;
    public float massAssimilated = 0;
    public float startingMass = 10;
    public float startingSpeed = 250;
    public float radius = 3.5f;
    private float packingDensity = .55f;
    private float shipVolume = 12f;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        mainCamera = GameObject.Find("Main Camera");
        spawnManager = FindObjectOfType<SpawnManager>();
        //Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isInDestroyAnimation)
        {
            float forwardInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            playerRb.AddForce(Vector3.Scale(mainCamera.transform.forward, Vector3.one - Vector3.up) * speed * forwardInput);
            playerRb.AddForce(mainCamera.transform.right * speed * horizontalInput);
            if (spawnManager.nearestEnemy != null)
            {
                playerLaser.transform.LookAt(spawnManager.nearestEnemy.transform.position, transform.up);
                playerLaser.transform.Rotate(Vector3.right, 90);
            }
        }
    }

    public void AddToAssimilated(float mass)
    {
        numberAssimilated++;
        massAssimilated += mass;
        float scaleFactor = (startingMass + massAssimilated) / startingMass;
        speed = startingSpeed * scaleFactor;
        radius = CalculateRadius();
        spawnManager.playerRadius = radius;
    }

    public float CalculateRadius()
    {
        float volume = numberAssimilated * shipVolume / packingDensity;
        return Mathf.Sqrt(volume / Mathf.PI);
    }
}

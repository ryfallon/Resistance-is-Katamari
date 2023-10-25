using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new Vector3(0, 6, -10);
    public float turnSpeed = 5.0f;

    private float minZoom = 0.5f;
    private float maxZoom = 8.0f;
    private float zoomSpeed = 0.1f;
    private float currentZoom = 1.0f;

    public TextMeshProUGUI displayText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        float zoomValue = Input.mouseScrollDelta.y;
        //displayText.text = "scroll value: " + zoomValue + ", currentZoom: " + currentZoom;
        currentZoom -= zoomValue * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
        transform.position = player.transform.position + offset * currentZoom;
        transform.LookAt(player.transform);
    }
}

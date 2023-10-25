using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Vector3 forward = (transform.position - mainCamera.transform.position).normalized;
        //Vector3 up = Vector3.Cross(forward, mainCamera.transform.right);
        //transform.rotation = Quaternion.LookRotation(forward, up);
        transform.LookAt(mainCamera.transform);
    }
}

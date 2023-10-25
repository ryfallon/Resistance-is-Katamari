using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHealthbar : MonoBehaviour
{
    public GameObject parent;
    private Camera mainCamera;
    public Vector3 offset = new Vector3(0f, 2.5f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        transform.SetParent(GameObject.Find("Fixed Canvas").transform);
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        try
        {
            transform.position = mainCamera.WorldToScreenPoint(parent.transform.position + offset);
            if (parent == null)
                Destroy(gameObject);
        }
        catch (MissingReferenceException)
        {
            Destroy(gameObject);
        }
    }
}

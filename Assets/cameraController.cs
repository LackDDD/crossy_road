using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public GameObject chickenObject;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - chickenObject.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()//excutes after update
    {
        transform.position = chickenObject.transform.position + offset;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class driveLeftToRight : MonoBehaviour
{
    private GameObject startingPoint;
    private GameObject endingPoint;

    public float speed = 15.0f;

    
    
    
    // Start is called before the first frame update
    void Start()
    {
        startingPoint = GameObject.Find("starting-point");
        endingPoint = GameObject.Find("ending-point");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(this.transform.position.x + speed * Time.deltaTime,
            this.transform.position.y,this.transform.position.z);
        if(this.transform.position.x > endingPoint.transform.position.x)
        {
            this.transform.position = new Vector3(startingPoint.transform.position.x,
                this.transform.position.y, this.transform.position.z);
        }
    }
}

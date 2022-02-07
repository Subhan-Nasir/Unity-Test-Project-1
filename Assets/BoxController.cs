using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{

    public GameObject box;
    public GameObject forceLocation1;
    public GameObject forceLocation2;

    private Rigidbody rb;

    public float force;

    // Start is called before the first frame update
    void Start()
    {

        rb = box.GetComponent<Rigidbody>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.realtimeSinceStartup >=2){
            rb.AddForceAtPosition(force*box.transform.forward, forceLocation1.transform.forward);
            rb.AddForceAtPosition(force*box.transform.forward, forceLocation2.transform.forward);

            
        }
        
    }
}

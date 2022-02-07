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
    private Vector3 lastVelocity;
    private Vector3 acceleration;

    // Start is called before the first frame update
    void Start()
    {

        rb = box.GetComponent<Rigidbody>();
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.realtimeSinceStartup >=2){
            rb.AddForceAtPosition(force * new Vector3 (0,0,1), forceLocation1.transform.position);
            rb.AddForceAtPosition(force * new Vector3 (0,0,1), forceLocation2.transform.position);
            acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
            lastVelocity = rb.velocity;

            Debug.Log("Acccleration = " + acceleration);
            
            
        }
        
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.white;
        Gizmos.DrawRay(forceLocation1.transform.position,forceLocation1.transform.forward*force);
        Gizmos.DrawRay(forceLocation2.transform.position,forceLocation1.transform.forward*force);
        
    }


}

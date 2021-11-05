using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class no_wc_drive : MonoBehaviour{

    private Rigidbody rb;

    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float dampingRatio;

    private float maxLength;
    private float minLength;
    private float previousLength;
    private float springLength;
    private float springVelocity;
    private float springForce;
    private float dampingForce;

    private Vector3 suspensionForce;

    [Header("Wheel")]
    public float wheelRadius;



    // Start is called before the first frame update
    void Start(){

        rb = transform.root.GetComponent<Rigidbody>();
        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }

    

    // Update is called once per frame
    void FixedUpdate(){

        if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius)){
            previousLength = springLength;            
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);

            springVelocity = (previousLength - springLength) / Time.fixedDeltaTime;

            springForce = springStiffness * (restLength - springLength);
            dampingForce = dampingRatio * springVelocity;

            suspensionForce = (springForce + dampingForce) * transform.up;
            Debug.DrawRay(transform.position, -transform.up * (wheelRadius + springLength), Color.red);


            rb.AddForceAtPosition(suspensionForce, hit.point);

        }           


    }
}

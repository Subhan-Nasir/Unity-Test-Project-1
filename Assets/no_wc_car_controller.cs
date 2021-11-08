using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code adapted from: https://www.youtube.com/watch?v=x0LUiE0dxP0 

public class no_wc_car_controller : MonoBehaviour{
    public Rigidbody rb;

    public List<GameObject> springs;

    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float dampingCoefficient;
    
    private float minLength;
    private float maxLength;
    private float[] springLength = new float[4];
    private float previousLength;
    private float springVelocity;

    private float springForce;
    private float damperForce;
    private Vector3 suspensionForce;
    

    [Header("Wheel")]
    public float wheelRadius;

    void Start(){
        
        minLength = restLength - springTravel;        
        maxLength = restLength + springTravel;
        for (int i = 0; i<4; i++){
            springLength[i] = restLength;
        }

    }

    void FixedUpdate(){
        


        for(int i = 0; i<springs.Count; i++){
            
            bool contact = Physics.Raycast(springs[i].transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius);
            if(contact){

                previousLength = springLength[i];
                springLength[i] = hit.distance - wheelRadius;
                springLength[i] = Mathf.Clamp(springLength[i], minLength, maxLength);

                springVelocity = (springLength[i] - previousLength)/Time.fixedDeltaTime;

                springForce = springStiffness * (restLength - springLength[i]);
                damperForce = dampingCoefficient * springVelocity;
                suspensionForce = (springForce - damperForce) * hit.normal;

                rb.AddForceAtPosition(suspensionForce, hit.point);



                
            }           
        }
    }

    
    


    void OnDrawGizmosSelected(){

        for(int i = 0; i < springs.Count; i++){
        

            // Vector3 direction = -transform.up *(springLength[i]+wheelRadius);
            // Gizmos.DrawRay(springs[i].transform.position, direction);


            

            Ray ray = new Ray(springs[i].transform.position, -transform.up);           
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(springs[i].transform.position, 0.1f);

            Gizmos.DrawWireSphere( transform.TransformPoint(rb.centerOfMass),0.1f);
            Gizmos.color = Color.blue;
                        
            Gizmos.DrawLine(ray.origin, -springLength[i] * transform.up + springs[i].transform.position);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(-springLength[i] * transform.up + springs[i].transform.position, -springLength[i] * transform.up + springs[i].transform.position + transform.up * -wheelRadius);
            Gizmos.DrawSphere((springs[i].transform.position) + (springLength[i]+wheelRadius)*(-transform.up),0.1f);
        
            // Gizmos.color = Color.white;
            // Gizmos.DrawRay(spring.transform.position, -transform.up * (maxLength + wheelRadius));
        }
        
    }

}

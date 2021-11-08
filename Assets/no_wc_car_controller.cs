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
    
    private float minLength;
    private float maxLength;
    private float[] springLength = new float[4];

    private float springForce;
    private Vector3 suspensionForce;

    [Header("Wheel")]
    public float wheelRadius;

    void Start(){
        
        // rb = GetComponent<Rigidbody>();
        minLength = restLength - springTravel;        
        maxLength = restLength + springTravel;

    }

    void FixedUpdate(){
        Debug.Log($"Max length = {maxLength}, Min length = {minLength}");


        for(int i = 0; i<springs.Count; i++){
            
            bool contact = Physics.Raycast(springs[i].transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius);
            // Debug.Log(contact);
            if(contact){
                springLength[i] = hit.distance - wheelRadius;

                springForce = springStiffness * (restLength - springLength[i]);
                suspensionForce = springForce * hit.normal;

                rb.AddForceAtPosition(suspensionForce, hit.point);



                
            }           
        }
    }

    
    


    void OnDrawGizmosSelected(){

        for(int i = 0; i < springs.Count; i++){
        

            Vector3 direction = -transform.up *(springLength[i]+wheelRadius);
            Gizmos.DrawRay(springs[i].transform.position, direction);

            Ray ray = new Ray(springs[i].transform.position, -transform.up);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ray.origin, -springLength[i] * transform.up + springs[i].transform.position);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(-restLength * transform.up + springs[i].transform.position, -restLength * transform.up + springs[i].transform.position + transform.up * -wheelRadius);


        
            // Gizmos.color = Color.white;
            // Gizmos.DrawRay(spring.transform.position, -transform.up * (maxLength + wheelRadius));
        }
        
    }

}

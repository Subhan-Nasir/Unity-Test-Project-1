using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Code adapted from: https://www.youtube.com/watch?v=x0LUiE0dxP0 

public class RaycastController : MonoBehaviour{

    
    public Rigidbody rb;
    public List<GameObject> springs;
    public List<GameObject> wheelObjects;
    public List<GameObject> meshes;


    [Header("Centre of mass")]    
    public GameObject COM_Fidner;

    [Header("Suspension Settings")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float dampingCoefficient; 
    
    
    private Suspension[] suspensions = new Suspension[4];
    


    [Header("Wheel")]
    public float wheelRadius = 0.23f;
    public float wheelMass = 0.1f;    

        
    // Order of constants >>> D, C, B, E, c, m
    // New
    // private float[] longitudinalConstants = new float[]{1502, 0.1879f, 17.74f, 1.137f, 0.01305f, 2.173E-6f};
    // private float[] lateralConstants = new float[]{1596, 1.5f, 12, 0.4f};

    // Old
    // private float[] longitudinalConstants = new float[]{1617, 1.3915f, 12.626f, 0.3936f, 0.01305f, 2.173E-6f};
    // private float[] lateralConstants = new float[]{1617, 1.3915f, 12.626f, 0.3936f};

    // One at a time
    private float[] longitudinalConstants = new float[]{1502, 0.1879f, 17.74f, 1f, 0.01305f, 2.173E-6f};
    private float[] lateralConstants = new float[]{1617, 1.3915f, 12.626f, 0.3936f};


    
    private Wheel[] wheels = new Wheel[4];

    [Header("Steering")]
    public float steerAngle = 20f;
    public float steerSpeed = 10f;

    private float wheel_x;
    private float wheel_y;
    private float wheel_z;
    
    private float steerInput;
    private float wheelAngleLeft;
    private float wheelAngleRight;
    private float steerAngleLeft;
    private float steerAngleRight;

    
    [Header("Ackermann Steering")]
    public bool enableAckermannSteering = true;
    public float wheelBase = 1.5f;
    public float rearTrack = 1.085f;
    public float turnRadius = 3.14f;
    private Vector3 steeringForce;

    private NewControls keys;
    private float throttle;
    private float brake;
    private float accel;

    private float theTime = 0f;
    private bool timerOn = false;
    private bool speedReached = false;

    void OnValidate(){
        keys = new NewControls();
        rb.centerOfMass = COM_Fidner.transform.localPosition;     
                           
        
        for (int i = 0; i < 4; i++){
            suspensions[i] = new Suspension(i, restLength, springTravel, springStiffness, dampingCoefficient, wheelRadius);                     
            wheels[i] = new Wheel(i, wheelObjects[i], meshes[i], rb, wheelRadius, wheelMass, longitudinalConstants, lateralConstants);
            
        }
        
                
    }

    

    void OnEnable(){
        
        keys.Enable();
        
        
    }

    void OnDisable(){
        keys.Disable();
    }

    

    void Start(){
        //

        
        
    }

    void Update(){

        steerInput = keys.Track.Steering.ReadValue<float>();
        throttle = keys.Track.Throttle.ReadValue<float>();
        brake = keys.Track.Brake.ReadValue<float>();

        steerInput = Mathf.Clamp(steerInput, -1,1);
        throttle = Mathf.Clamp(throttle, 0,1);
        brake = Mathf.Clamp(brake, 0,1);
      
        
        if(throttle > brake){
            accel = throttle;
        }
        else{
            accel = -brake;
        }

        
        ApplySteering();     
         
    }

    void FixedUpdate(){
        
 
        for(int i = 0; i<springs.Count; i++){    

            bool contact = Physics.Raycast(springs[i].transform.position, -transform.up, out RaycastHit hit, 0.6f + 0.35f + wheelRadius);

            if(contact){            
                
                // Suspension force in the vertical direction.
                Vector3 suspensionForceVector = suspensions[i].getUpdatedForce(hit, Time.fixedDeltaTime);
                Vector3 wheelForceVector = wheels[i].getUpdatedForce(accel, hit, Time.fixedDeltaTime, suspensionForceVector.magnitude);            
                                
                rb.AddForceAtPosition(wheelForceVector + suspensionForceVector, hit.point); 

            }
            else{

            }
        }

        float carSpeed = rb.velocity.z;
        if(carSpeed > 0 & speedReached == false){
            timerOn = true;
        }

        if(timerOn == true){
            theTime += Time.fixedDeltaTime;
            
            if(carSpeed >= 26.8f){
                speedReached = true;
                timerOn = false;
            }
        }
        Debug.Log($"Timer = {theTime}");

    }
    

    void OnDrawGizmos(){

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere( transform.TransformPoint(rb.centerOfMass),0.2f);
        
        for(int i = 0; i < springs.Count; i++){
        
                        
            Gizmos.color = Color.blue;
            Ray ray = new Ray(springs[i].transform.position, -transform.up);           
            Gizmos.DrawLine(ray.origin, -suspensions[i].springLength * transform.up + springs[i].transform.position);

            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(-suspensions[i].springLength * transform.up + springs[i].transform.position, -suspensions[i].springLength * transform.up + springs[i].transform.position + transform.up * -wheelRadius);
            
        
            Gizmos.color = Color.white;
            Gizmos.DrawRay(wheels[i].wheelObject.transform.position, wheels[i].wheelObject.transform.right * (0.5f));
            Gizmos.DrawRay(wheels[0].wheelObject.transform.position, wheels[0].wheelObject.transform.forward * (0.5f));
            

            
        }
        
    }

    void ApplySteering(){

         // Applies Ackermann sterring if it is enabled.
        if(enableAckermannSteering){
            //Steering right
            if(steerInput > 0){
                steerAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack/2))) * steerInput;
                steerAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack/2))) * steerInput;

            }//Steering left            
            else if (steerInput < 0){
                steerAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack/2))) * steerInput;
                steerAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack/2))) * steerInput;
                

            } // Not steering
            else{
                steerAngleLeft = 0;
                steerAngleRight = 0;

            }
            
        }
        // If Ackermann steering is disabled, both wheels have the same steer angle.
        else{

            steerAngleLeft = steerAngle * steerInput;
            steerAngleRight = steerAngle * steerInput;           

        }

        wheelAngleLeft = Mathf.Lerp(wheelAngleLeft, steerAngleLeft, steerSpeed * Time.deltaTime);
        wheelAngleRight = Mathf.Lerp(wheelAngleRight, steerAngleRight, steerSpeed * Time.deltaTime);

        wheelObjects[0].transform.localRotation = Quaternion.Euler(
            wheelObjects[0].transform.localRotation.x, 
            wheelObjects[0].transform.localRotation.y + wheelAngleLeft,
            wheelObjects[0].transform.localRotation.z );
        
        wheelObjects[1].transform.localRotation = Quaternion.Euler(
            wheelObjects[1].transform.localRotation.x, 
            wheelObjects[1].transform.localRotation.y + wheelAngleRight,
            wheelObjects[1].transform.localRotation.z );

    }


    // Use these for the UI:
    public Suspension[] getSuspensions(){
        return suspensions;
    }

    public Wheel[] getWheels(){
        return wheels;
    }
   
    
}


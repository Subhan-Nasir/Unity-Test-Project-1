using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Code adapted from: https://www.youtube.com/watch?v=x0LUiE0dxP0 

public class RaycastController : MonoBehaviour{

    
    public Rigidbody rb;
    public bool enableTimer;
    public List<GameObject> springs;
    public List<GameObject> wheelObjects;
    public List<GameObject> meshes;


    [Header("Centre of mass")]    
    public GameObject COM_Fidner;

    [Header("Suspension Settings")]
    public float naturalLength;
    public float springTravel;
    public float springStiffness;
    public float bumpStiffness;
    public float dampingCoefficient; 
    
    
    private Suspension[] suspensions = new Suspension[4];
    
    [Header("Wheel")]
    public float wheelRadius = 0.23f;
    public float wheelMass = 5;    
       
    
    private Dictionary<string, float> lateralConstants = new Dictionary<string,float>(){
        {"B", 11.45f},
        {"C", 1.551f},
        {"D", 1790},
        {"E", 0.1859f},
        {"c", 0.00151f},
        {"m", 2.533E-7f}
    };

    // private Dictionary<string, float> longitudinalConstants = new Dictionary<string, float>(){
    //     {"B", 11.95f},
    //     {"C", 1.515f},
    //     {"D", 1609},
    //     {"E", 0.1497f},
    //     {"c", 0.001926f},
    //     {"m", 4.03E-7f}     
    // };
    
    private Dictionary<string, float> longitudinalConstants = new Dictionary<string, float>(){
        {"B", 11.93f},
        {"C", 1.716f},
        {"D", 1711},
        {"E", 0.3398f},
        {"c", 0.00179f},
        {"m", 3.62E-7f}     
    };
    
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
    private float userInput;

    private float theTime = 0f;
    private bool timerOn = false;
    private bool speedReached = false;

    [UPyPlot.UPyPlotController.UPyProbe]
    private float RL_springLength;

    void OnValidate(){
        keys = new NewControls();
        rb.centerOfMass = COM_Fidner.transform.localPosition;     
                           
        
        for (int i = 0; i < 4; i++){
            suspensions[i] = new Suspension(i, naturalLength, springTravel, springStiffness, bumpStiffness, dampingCoefficient, wheelRadius);                     
            wheels[i] = new Wheel(i, wheelObjects[i], rb, wheelRadius, wheelMass, longitudinalConstants, lateralConstants);
            
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
        // throttle = keys.Track.Throttle.ReadValue<float>();
        // brake = keys.Track.Brake.ReadValue<float>();

        // // 0 means not pressed, 1 means fully pressed
        // throttle = Mathf.Clamp(throttle, -0.336f,0.0895f); 
        // brake = Mathf.Clamp(brake, 0.8276286f,-0.6f);
        
        // throttle = (throttle - -0.336f)/(0.0895f - -0.336f);
        // brake = (brake - 0.8276286f)/(-0.6f - 0.8276286f);
        
        // // steer = Input.GetAxis("Horizontal");
        // // -1 means left and +1 means right. 0 means no steering
        // steerInput = keys.Track.Steering.ReadValue<float>();
        // steerInput = Mathf.Clamp(steerInput, -1,1);     


        steerInput = keys.Track.Steering.ReadValue<float>();
        throttle = keys.Track.Throttle.ReadValue<float>();
        brake = keys.Track.Brake.ReadValue<float>();

        steerInput = Mathf.Clamp(steerInput, -1,1);
        throttle = Mathf.Clamp(throttle, 0,1);
        brake = Mathf.Clamp(brake, 0,1);
      
        
        if(throttle > brake){
            userInput = throttle;
        }
        else{
            userInput = -brake;
        }

        
        ApplySteering();     
         
    }

    void FixedUpdate(){
        
 
        for(int i = 0; i<springs.Count; i++){    

            bool contact = Physics.Raycast(springs[i].transform.position, -transform.up, out RaycastHit hit, naturalLength + springTravel + wheelRadius);
            
            if(contact){            
                
                // Suspension force in the vertical direction.
                Vector3 suspensionForceVector = suspensions[i].getUpdatedForce(hit, Time.fixedDeltaTime);
                Vector3 wheelForceVector = wheels[i].getUpdatedForce(userInput, hit, Time.fixedDeltaTime, suspensions[i].forceVector.magnitude);            

                if(Time.realtimeSinceStartup >=0){                
                    rb.AddForceAtPosition(wheelForceVector + suspensionForceVector, hit.point + new Vector3 (0,0.1f,0)); 
                }

                if(i == 2){
                    RL_springLength = suspensions[i].springLength;
                }

                
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

        if(enableTimer == true){
            Debug.Log($"Timer = {theTime}");
        }

       
        
        

    }
    

    void OnDrawGizmos(){

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere( transform.TransformPoint(rb.centerOfMass),0.2f);
        
        for(int i = 0; i < springs.Count; i++){
        
                        
            // Gizmos.color = Color.blue;
            Ray ray = new Ray(springs[i].transform.position, -transform.up);           
            // Gizmos.DrawLine(ray.origin, -suspensions[i].springLength * transform.up + springs[i].transform.position);

            
            Gizmos.color = Color.yellow;
            // Gizmos.DrawLine(-suspensions[i].springLength * transform.up + springs[i].transform.position, -suspensions[i].springLength * transform.up + springs[i].transform.position + transform.up * -wheelRadius);
            
        
            Gizmos.color = Color.white;

            Gizmos.DrawRay(wheels[i].wheelObject.transform.position, wheels[i].wheelObject.transform.right * (wheels[i].lateralForce/1000));
            Gizmos.DrawRay(wheels[i].wheelObject.transform.position, wheels[i].wheelObject.transform.forward * (wheels[i].longitudinalForce/1000));
            Gizmos.DrawRay(wheels[i].wheelObject.transform.position, wheels[i].wheelObject.transform.up * wheels[i].verticalLoad/1000);
            
            // Gizmos.DrawRay(wheels[i].wheelObject.transform.position, wheels[i].forceVector/1000 );
            
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

        // wheelAngleLeft = Mathf.Lerp(wheelAngleLeft, steerAngleLeft, steerSpeed * Time.deltaTime);
        // wheelAngleRight = Mathf.Lerp(wheelAngleRight, steerAngleRight, steerSpeed * Time.deltaTime);
        wheelAngleLeft=steerAngleLeft;
        wheelAngleRight=steerAngleRight;


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
   
    public float getSteeringAngleL(){return steerAngleLeft;}
    public float getSteeringAngleR(){return steerAngleRight;}
    public float getAccel(){return userInput;}

}


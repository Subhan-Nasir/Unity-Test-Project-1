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

    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float dampingCoefficient;
  
    private Suspension[] suspensions = new Suspension[4];
    
    

    [Header("Wheel")]
    public float wheelRadius = 0.23f;
    public float wheelMass = 0.1f;    

    private float D = 1617f;
    private float C = 1.3915f;
    private float B = 12.626f;
    private float E = 0.3936f;
    
    
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


    private void Awake(){
        keys = new NewControls();
        rb.centerOfMass = COM_Fidner.transform.localPosition;

        for (int i = 0; i < 4; i++){
            suspensions[i] = new Suspension(i, restLength, springTravel, springStiffness, dampingCoefficient, wheelRadius);
            wheels[i] = new Wheel(i, wheelObjects[i], rb, wheelRadius, wheelMass, D, C, B, E);
        }
                
    }

    

    private void OnEnable(){
        keys.Enable();
    }

    private void OnDisable(){
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

            bool contact = Physics.Raycast(springs[i].transform.position, -transform.up, out RaycastHit hit, restLength + springTravel + wheelRadius);

            if(contact){            
                
                // Suspension force in the vertical direction.
                Vector3 suspensionForceVector = suspensions[i].getUpdatedForce(hit, Time.fixedDeltaTime);
                Vector3 wheelForceVector = wheels[i].getUpdatedForce(accel, hit);            
                                
                rb.AddForceAtPosition(wheelForceVector + suspensionForceVector, hit.point); 

            }
            else{

            }
        }
    }
    

    // void OnDrawGizmosSelected(){

    //     Gizmos.color = Color.white;
    //     Gizmos.DrawWireSphere( transform.TransformPoint(rb.centerOfMass),0.2f);

    //     for(int i = 0; i < springs.Count; i++){
        

    //         // Vector3 direction = -transform.up *(springLength[i]+wheelRadius);
    //         // Gizmos.DrawRay(springs[i].transform.position, direction);
          
    //         Gizmos.color = Color.white;
    //         // Gizmos.DrawSphere(springs[i].transform.position, 0.1f);

            
    //         Gizmos.color = Color.blue;
    //         Ray ray = new Ray(springs[i].transform.position, -transform.up);           
    //         Gizmos.DrawLine(ray.origin, -suspensions[i].springLength * transform.up + springs[i].transform.position);

    //         // Gizmos.color = Color.green;
    //         // if(i == 0 | i == 1){
    //         //     Gizmos.DrawRay(wheels[i].transform.position, wheelVelocitiesLS[i]);
    //         // }
            
            
            



            

    //         Gizmos.color = Color.yellow;
    //         Gizmos.DrawLine(-springLength[i] * transform.up + springs[i].transform.position, -springLength[i] * transform.up + springs[i].transform.position + transform.up * -wheelRadius);
    //         // Gizmos.DrawSphere((springs[i].transform.position) + (springLength[i]+wheelRadius)*(-transform.up),0.1f);
        
    //         Gizmos.color = Color.white;
    //         Gizmos.DrawRay(wheels[i].transform.position, wheels[i].transform.right * (0.5f));
    //         Gizmos.DrawRay(wheels[i].transform.position, wheels[i].transform.forward * (0.5f));
    //     }
        
    // }

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
    


    // Classes used
    public class Suspension{

        public float id;
        public float restLength;
        public float springTravel;
        public float springStiffness;
        public float dampingCoefficient;
        public float wheelRadius;
        
        public float minLength;
        public float maxLength;
        public float springLength;
        public float previousLength;
        public float springVelocity;

        public float springForce;
        public float damperForce;
        public Vector3 force = new Vector3();

        public Suspension(float id, float restLength, float springTravel, float springStiffness, float dampingCoefficient, float wheelRadius){
            this.id = id;
            this.restLength = restLength;
            this.springTravel = springTravel;
            this.springStiffness = springStiffness;
            this.dampingCoefficient = dampingCoefficient;
            this.wheelRadius = wheelRadius;

            this.minLength = restLength - springTravel;
            this.maxLength = restLength + springTravel;
            this.springLength = restLength;
            this.previousLength = restLength;
            

        }

        public Vector3 getUpdatedForce(RaycastHit hit, float timeDelta){
            previousLength = springLength;
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (springLength - previousLength)/timeDelta;
            springForce = springStiffness * (restLength - springLength);
            damperForce = dampingCoefficient * springVelocity;
            force = (springForce - damperForce) * hit.normal;
            
            
            return force;

        }



    }

    public class Wheel{

    public float id;
    public GameObject wheelObject;
    public Rigidbody rb;
    public float wheelRadius;
    public float wheelMass;  


    public float slipAngle;
    public Vector3 wheelVelocityLS;        
    public float longitudinalVelocty;
    public float lateralVelocity;

    public float lateralForce; //Sideways direction
    public float vertical_force; //Upwards direction
    public float longitudinalForce; // Forwards direction
    public Vector3 forceVector;

    public float D;
    public float C;
    public float B;
    public float E;

    public Wheel(float id, GameObject wheelObject, Rigidbody rb, float wheelRadius, float wheelMass, float D, float C, float B, float E){
        this.id = id;
        this.wheelObject = wheelObject;
        this.rb = rb;
        this.wheelRadius = wheelRadius;
        this.wheelMass = wheelMass;

        this.D = D;
        this.C = C;
        this.B = B; 
        this.E = E;      

    }

    public float tyreEquation(float slip, float D, float C, float B, float E){
        float force = D * Mathf.Sin( C * Mathf.Atan(B * slip - E * ( (B*slip) - Mathf.Atan(B*slip))));
        return force;  
    }

    public Vector3 getUpdatedForce(float accel, RaycastHit hit){

        wheelObject.transform.position = hit.point + hit.normal * wheelRadius;

        wheelVelocityLS = wheelObject.transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

        lateralVelocity = wheelVelocityLS.x;
        longitudinalVelocty = wheelVelocityLS.z;
                
        
        if(longitudinalVelocty < 0.2f){
            slipAngle = 0;
        }
        else{
            slipAngle = -Mathf.Atan(lateralVelocity / Mathf.Abs(longitudinalVelocty));
            
        }

        // Calculates driving force to the wheels.
        if(id == 2 | id == 3){            
            longitudinalForce = (accel * (100/0.23f));
        }
        else{            
            longitudinalForce = (accel * (10/0.23f));
        }     

        lateralForce = tyreEquation(slipAngle, D, C, B, E);
        forceVector = longitudinalForce * wheelObject.transform.forward + lateralForce * wheelObject.transform.right;

        return forceVector;


    }


}
    
}


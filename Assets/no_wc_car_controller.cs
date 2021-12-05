using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Code adapted from: https://www.youtube.com/watch?v=x0LUiE0dxP0 

public class no_wc_car_controller : MonoBehaviour{
    public Rigidbody rb;

    public List<GameObject> springs;
    public List<GameObject> wheels;
    public List<GameObject> meshes;


    [Header("Centre of mass")]    
    public GameObject COM_Fidner;

    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float dampingCoefficient;
    
    private float minLength;
    private float maxLength;
    private float[] springLength = new float[4];
    private float[] previousLength = new float[4];
    private float[] springVelocity = new float[4];

    private float[] springForce = new float[4];
    private float[] damperForce = new float[4];
    private Vector3[] suspensionForce = new Vector3[4];

    

    

    [Header("Wheel")]
    public float wheelRadius = 0.23f;
    public float wheelMass = 0.1f;    

    private float wheel_x;
    private float wheel_y;
    private float wheel_z;
    private Vector3[] wheelVelocitiesLS = new Vector3[4];

    private Vector3[] Fx = new Vector3[4];
    private Vector3[] Fy = new Vector3[4];
    private Vector3[] Fz = new Vector3[4];

    private float[] slipAngle = new float[4];
    private Vector3[] slip_vector = new Vector3[4];
    private float[] friction = new float[4];
    private float[] longitudinalVelocty = new float[4];
    private float[] lateralVelocity = new float[4];

    private float[] lateralForce = new float[4]; //Sideways direction
    private float[] vertical_force = new float [4]; //Upwards direction
    private float[] longitudinalForce = new float[4]; // Forwards direction

    [Header("Steering")]
    public float steerAngle = 20f;
    public float steerSpeed = 10f;

    
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

    private float D = 1617f;
    private float C = 1.3915f;
    private float B = 12.626f;
    private float E = 0.3936f;
   


   private float[] alpha = new float[4];
   private float[] omega = new float[4];
   private float[] theta = new float[4];

    private void Awake(){
        keys = new NewControls();
        rb.centerOfMass = COM_Fidner.transform.localPosition;

    }

    private void OnEnable(){
        keys.Enable();
    }

    private void OnDisable(){
        keys.Disable();
    }

    

    void Start(){
        
        minLength = restLength - springTravel;        
        maxLength = restLength + springTravel;

        // Initialise lists.
        for (int i = 0; i<4; i++){
            springLength[i] = restLength;
            wheelVelocitiesLS[i] = new Vector3 (0,0,0);
        }        

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
            
            bool contact = Physics.Raycast(springs[i].transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius);
            if(contact){               
                
                // Making the wheels spin
                alpha[i] = (longitudinalForce[i] * wheelRadius) / (0.5f * wheelMass * Mathf.Pow(wheelRadius, 2f));
                omega[i] = omega[i] + alpha[i] * Time.deltaTime;
                theta[i] = theta[i] + omega[i] * Time.deltaTime;

                meshes[i].transform.Rotate(Mathf.Rad2Deg * omega[i] * Time.deltaTime, 0,0);
                
                



                previousLength[i] = springLength[i];
                springLength[i] = hit.distance - wheelRadius;
                springLength[i] = Mathf.Clamp(springLength[i], minLength, maxLength);

                springVelocity[i] = (springLength[i] - previousLength[i])/Time.fixedDeltaTime;

                springForce[i] = springStiffness * (restLength - springLength[i]);
                damperForce[i] = dampingCoefficient * springVelocity[i];
                vertical_force[i] = springForce[i] - damperForce[i];

                // Suspension force in the vertical direction.
                Fy[i] = vertical_force[i] * hit.normal;

                
                // Makes wheel's model move up and down.
                wheels[i].transform.position = hit.point + hit.normal * wheelRadius;

                // Gets local velocity of the wheel at the contact point.                
                wheelVelocitiesLS[i] = wheels[i].transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));
                
 
                
                // Calculates driving force to the wheels.
                if(i == 2 | i == 3){
                    // longitudinalForce[i] = (accel * (100/0.23f)) - 10 * longitudinalVelocty[i];
                    longitudinalForce[i] = (accel * (100/0.23f));

                }
                else{
                    // longitudinalForce[i] = (accel * (10/0.23f)) - 10 * longitudinalVelocty[i];
                    // longitudinalForce = 0;
                    longitudinalForce[i] = (accel * (10/0.23f));
                }

                

                
                // Calculates slip angle in radians
                lateralVelocity[i] = wheelVelocitiesLS[i].x;
                longitudinalVelocty[i] = wheelVelocitiesLS[i].z;

                if(longitudinalVelocty[i] < 0.2f){
                    slipAngle[i] = 0;
                }
                else{
                    slipAngle[i] = -Mathf.Atan(lateralVelocity[i]/Mathf.Abs(longitudinalVelocty[i]));
                    
                }     

                // Calculates lateral force using the "magic equation".                
                lateralForce[i] = tyreEquation(slipAngle[i], D, C, B, E);
                     
                
                Debug.Log($"Wheel {i}: F = {lateralForce[i]}, Slip Angle = {Mathf.Rad2Deg * slipAngle[i]} deg, Longitudinal velocity = {wheelVelocitiesLS[i].z}, Lateral velocty = {wheelVelocitiesLS[i].x}");
                
                // Debug.DrawRay(wheels[i].transform.position, wheels[i].transform.right * (lateralForce[i]));

                

                Debug.DrawRay(rb.transform.position, rb.velocity);

                Fz[i] = longitudinalForce[i] * wheels[i].transform.forward;
                Fx[i] = lateralForce[i] * wheels[i].transform.right;  

                
                
                rb.AddForceAtPosition(Fx[i] + Fy[i] + Fz[i], hit.point);

                

                             
                
            }
            else{

            // wheel_x = wheels[i].transform.localPosition.x;
            // wheel_y = maxLength + wheelRadius;
            // wheel_z = wheels[i].transform.localPosition.z;

            // wheels[i].transform.localPosition = new Vector3 (wheel_x, wheel_y, wheel_z);

            }
            
            

        }
    }

    
    static float tyreEquation(float slipAngle, float D, float C, float B, float E){
        float Force = D * Mathf.Sin( C * Mathf.Atan(B * slipAngle - E * ( (B*slipAngle) - Mathf.Atan(B*slipAngle))));
        return Force;  
    }
    


    void OnDrawGizmosSelected(){

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere( transform.TransformPoint(rb.centerOfMass),0.2f);

        for(int i = 0; i < springs.Count; i++){
        

            // Vector3 direction = -transform.up *(springLength[i]+wheelRadius);
            // Gizmos.DrawRay(springs[i].transform.position, direction);
          
            Gizmos.color = Color.white;
            // Gizmos.DrawSphere(springs[i].transform.position, 0.1f);

            
            Gizmos.color = Color.blue;
            Ray ray = new Ray(springs[i].transform.position, -transform.up);           
            Gizmos.DrawLine(ray.origin, -springLength[i] * transform.up + springs[i].transform.position);

            // Gizmos.color = Color.green;
            // if(i == 0 | i == 1){
            //     Gizmos.DrawRay(wheels[i].transform.position, wheelVelocitiesLS[i]);
            // }
            
            
            



            

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(-springLength[i] * transform.up + springs[i].transform.position, -springLength[i] * transform.up + springs[i].transform.position + transform.up * -wheelRadius);
            // Gizmos.DrawSphere((springs[i].transform.position) + (springLength[i]+wheelRadius)*(-transform.up),0.1f);
        
            Gizmos.color = Color.white;
            Gizmos.DrawRay(wheels[i].transform.position, wheels[i].transform.right * (0.5f));
            Gizmos.DrawRay(wheels[i].transform.position, wheels[i].transform.forward * (0.5f));
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

        wheels[0].transform.localRotation = Quaternion.Euler(
            wheels[0].transform.localRotation.x, 
            wheels[0].transform.localRotation.y + wheelAngleLeft,
            wheels[0].transform.localRotation.z );
        
        wheels[1].transform.localRotation = Quaternion.Euler(
            wheels[1].transform.localRotation.x, 
            wheels[1].transform.localRotation.y + wheelAngleRight,
            wheels[1].transform.localRotation.z );

    }
}

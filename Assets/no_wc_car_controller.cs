using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code adapted from: https://www.youtube.com/watch?v=x0LUiE0dxP0 

public class no_wc_car_controller : MonoBehaviour{
    public Rigidbody rb;

    public List<GameObject> springs;
    public List<GameObject> wheels;


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
    private float previousLength;
    private float springVelocity;

    private float springForce;
    private float damperForce;
    private Vector3 suspensionForce;

    private float f_x; //Sideways direction
    private float f_y; //Upwards direction
    private float f_z; // Forwards direction

    private Vector3 Fx;
    private Vector3 Fy;
    private Vector3 Fz;

    [Header("Wheel")]
    public float wheelRadius;

    private float wheel_x;
    private float wheel_y;
    private float wheel_z;
    private Vector3[] wheelVelocitiesLS = new Vector3[4];

    [Header("Steering")]
    public float steerAngle = 30f;
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



    void Awake(){
        rb.centerOfMass = COM_Fidner.transform.localPosition;

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
        steerInput = Input.GetAxis("Horizontal");
        ApplySteering();     

                


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
                f_y = springForce - damperForce;
                suspensionForce = f_y * hit.normal;

                

                wheels[i].transform.position = hit.point + hit.normal * wheelRadius;
                wheelVelocitiesLS[i] = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

                Fz = (Input.GetAxis("Vertical") * springForce) * wheels[i].transform.forward;
                
                // if( i == 0 | i == 2){  // wheel is on the left of car

                //     Fx = (wheelVelocitiesLS[i].x * springForce) * -wheels[i].transform.right; // apply force inwards
                // }
                // else { // wheel is on the right of car
                //     Fx = (wheelVelocitiesLS[i].x * springForce) * -wheels[i].transform.right; // apply force inwards
                // }
                

                

                rb.AddForceAtPosition(suspensionForce + Fz, hit.point);

                             
                
            }
            else{

            // wheel_x = wheels[i].transform.localPosition.x;
            // wheel_y = maxLength + wheelRadius;
            // wheel_z = wheels[i].transform.localPosition.z;

            // wheels[i].transform.localPosition = new Vector3 (wheel_x, wheel_y, wheel_z);

            }
            
            

        }
    }

    
    


    void OnDrawGizmosSelected(){

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere( transform.TransformPoint(rb.centerOfMass),0.2f);

        for(int i = 0; i < springs.Count; i++){
        

            // Vector3 direction = -transform.up *(springLength[i]+wheelRadius);
            // Gizmos.DrawRay(springs[i].transform.position, direction);


            

                      
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(springs[i].transform.position, 0.1f);

            
            Gizmos.color = Color.blue;
            Ray ray = new Ray(springs[i].transform.position, -transform.up);           
            Gizmos.DrawLine(ray.origin, -springLength[i] * transform.up + springs[i].transform.position);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(-springLength[i] * transform.up + springs[i].transform.position, -springLength[i] * transform.up + springs[i].transform.position + transform.up * -wheelRadius);
            Gizmos.DrawSphere((springs[i].transform.position) + (springLength[i]+wheelRadius)*(-transform.up),0.1f);
        
            // Gizmos.color = Color.white;
            // Gizmos.DrawRay(spring.transform.position, -transform.up * (maxLength + wheelRadius));
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Toby

//Sandeep

// Code adapted from:
// https://www.youtube.com/watch?v=x0LUiE0dxP0&list=PLcbsEpz1iFyjjddSqLxnnGSJthfCcmsav
// https://www.youtube.com/watch?v=mnAEeE3FcvA&list=PLi-ukGVOag_2oMT2_Nmq_EbqOaS-s7xr6
// https://www.youtube.com/watch?v=BwL3Dm8GJtQ

// Comment for git kraken by naveen

// change after deleting meta file

public class drive_script : MonoBehaviour
{

    public WheelCollider[] wheel_colliders;
    public GameObject[] wheels;

    [Header("Drivetrain")]
    public float max_torque = 100;
    public bool all_wheel_drive = true;

    private float torque;

    [Header("Standard Steering")]
    public float max_steer_angle = 30;

    [Header("Ackermann steering")]
    public bool Ackermann_Steering = true;
    public float wheel_base = 1.5f;
    public float rear_track = 1.085f;
    public float turn_radius = 3.14f;


    [Header("Anti-roll bars")]
    public bool enable_anti_roll_bars = true;
    public float AntiRoll = 5000.0f;



    private float steer_angle_left;
    private float steer_angle_right;

    private float accel;

    private float throttle;    
    private float brake;
    private float steer;
    

    private Rigidbody car;
    private NewControls controls;

    private float theTime = 0f;
    private bool timerOn = false;
    private bool speedReached = false;
    public bool enableTimer;

    private void Awake(){
        controls = new NewControls();

    }

    private void OnEnable(){
        controls.Enable();
    }

    private void OnDisable(){
        controls.Disable();
    }


    // Start is called before the first frame update
    void Start(){
        car = GetComponent<Rigidbody> ();      
        
    }


    // FixedUpdate is called once per frame
    void FixedUpdate(){
        
        throttle = controls.Track.Throttle.ReadValue<float>();        
        brake = controls.Track.Brake.ReadValue<float>();
        steer = controls.Track.Steering.ReadValue<float>();

        throttle = Mathf.Clamp(throttle, 0,1); 
        brake = Mathf.Clamp(brake, 0,1);
        steer = Mathf.Clamp(steer, -1,1);

        Debug.Log($"Throttle input = {throttle}, Brake input = {brake}, Steering input: {steer}");
        

        // 0 means not pressed, 1 means fully pressed
        

        if (throttle > brake){
            accel = throttle;
        }
        else{
            accel = -brake;
        }

        // Calls the function to drive the car
        drive(accel,steer);

        float carSpeed = car.velocity.z;
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

    // Function to drive the the car (accerlerate/decelerate and steer)
    void drive(float accel, float steer){  

        torque = accel*max_torque;    

       
        // Apply torque to wheels based on all wheel drive or rear wheel drive
        apply_torque_to_wheels(torque);
        
        // Calls the steering function to apply steering for the given input
        apply_steering(steer);


        // Applies force from the anti-roll bars if they are enabled.   
        apply_anti_roll_bars(wheel_left: wheel_colliders[2], wheel_right: wheel_colliders[3]);


        // Makes the 3D model of the wheels spin;
        align_wheel_meshes();
        

    }


    void apply_torque_to_wheels(float torque){

        if (all_wheel_drive == true){

            //Apply torque to all 4 wheels
            for(int i = 0; i<4; i++){
                wheel_colliders[i].motorTorque = torque;                
            }

        }
        else{

            // Apply torque to only the rear wheels
            wheel_colliders[2].motorTorque = torque;
            wheel_colliders[3].motorTorque = torque;
        }

    }

    // Function for steering the car.
    void apply_steering(float steer){

        // Applies Ackermann sterring if it is enabled.
        if(Ackermann_Steering){
            //Steering right
            if(steer > 0){
                steer_angle_left = Mathf.Rad2Deg * Mathf.Atan(wheel_base / (turn_radius + (rear_track/2))) * steer;
                steer_angle_right = Mathf.Rad2Deg * Mathf.Atan(wheel_base / (turn_radius - (rear_track/2))) * steer;

            }//Steering left            
            else if (steer < 0){
                steer_angle_left = Mathf.Rad2Deg * Mathf.Atan(wheel_base / (turn_radius - (rear_track/2))) * steer;
                steer_angle_right = Mathf.Rad2Deg * Mathf.Atan(wheel_base / (turn_radius + (rear_track/2))) * steer;
                

            } // Not steering
            else{
                steer_angle_left = 0;
                steer_angle_right = 0;

            }
            wheel_colliders[0].steerAngle = steer_angle_left;
            wheel_colliders[1].steerAngle = steer_angle_right;
        }
        // If Ackermann steering is disabled, both wheels have the same steer angle.
        else{
            wheel_colliders[0].steerAngle = steer * max_steer_angle;
            wheel_colliders[1].steerAngle = steer * max_steer_angle;

        }
    }


    // Function to apply forces from the anti-roll bars.
    void apply_anti_roll_bars(WheelCollider wheel_left, WheelCollider wheel_right){
        

        if(enable_anti_roll_bars == true){

            WheelHit hit;
            float travelL = 1.0f;
            float travelR = 1.0f;


            bool groundedL = wheel_left.GetGroundHit (out hit);
            if (groundedL) {
                travelL = (-wheel_left.transform.InverseTransformPoint (hit.point).y - wheel_left.radius) / wheel_left.suspensionDistance;
            }

            bool groundedR = wheel_right.GetGroundHit (out hit);
            if (groundedR) {
                travelR = (-wheel_right.transform.InverseTransformPoint (hit.point).y - wheel_right.radius) / wheel_right.suspensionDistance;
            }

            float antiRollForce = (travelL - travelR) * AntiRoll;

            if (groundedL){
                car.AddForceAtPosition (wheel_left.transform.up * -antiRollForce, wheel_left.transform.position);            
            }
                

            if (groundedR){
                car.AddForceAtPosition (wheel_right.transform.up * antiRollForce, wheel_right.transform.position);
            }

        }       
			

    }

    void align_wheel_meshes(){

        for(int i = 0; i<4; i++){      
            
            Quaternion q; 
            Vector3 pos;
            wheel_colliders[i].GetWorldPose(out pos, out q);
            wheels[i].transform.position = pos;
            wheels[i].transform.rotation = q;

        }
    }

}

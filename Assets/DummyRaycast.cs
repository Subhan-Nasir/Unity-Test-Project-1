using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRaycast : MonoBehaviour{

    public Rigidbody rb;
    public List<GameObject> springs;
    public List<GameObject> wheelObjects;

    public float restLength = 2;
    public float springTravel = 1;
    public float stiffness;
    public float damping;
    public float wheelRadius;
    public float wheelMass;
    public float steerAngle;
    public float steerSpeed;
    public bool enableAckermannSteering;
    public float wheelBase;
    public float rearTrack;
    public float turnRadius;

    private float minLength;
    private float maxLength;
    private float springVelocity;
    
    private float[] previousLengths = new float[4];
    private float[] springLengths = new float[4];
    private Wheel[] wheels = new Wheel[4];

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
    



    private NewControls keys;
    private float throttle;
    private float brake;
    private float steerInput;
    private float userInput;
    private float steerAngleLeft;
    private float steerAngleRight;
    private float wheelAngleLeft;
    private float wheelAngleRight;

    void OnValidate(){
        keys = new NewControls();
    }

    void OnEnable(){
        keys.Enable();
    }

    void OnDisable(){
        keys.Disable();
    }



    // Start is called before the first frame update
    void Start(){
        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
        for(int i = 0; i<springs.Count; i++){
            previousLengths[i] = restLength;
            springLengths[i] = restLength;
            wheels[i] = new Wheel(i, 
                wheelObjects[i],                
                rb,
                wheelRadius, 
                wheelMass,
                longitudinalConstants,
                lateralConstants);
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
            userInput = throttle;
        }
        else{
            userInput = -brake;
        }

        
        ApplySteering(); 


    }

    // Update is called once per frame
    void FixedUpdate(){
        for(int i = 0; i<springs.Count; i++){
            bool contact = Physics.Raycast(springs[i].transform.position, -transform.up, out RaycastHit hit, restLength + springTravel + wheelRadius);
            if(contact){
                
                previousLengths[i] = springLengths[i];
                springLengths[i] = hit.distance - wheelRadius;
                springLengths[i] = Mathf.Clamp(springLengths[i], minLength, maxLength);
                springVelocity = (springLengths[i] - previousLengths[i])/Time.fixedDeltaTime;
                float springForce = stiffness * (restLength - springLengths[i]);
                float damperForce = damping * springVelocity;
                float force = springForce - damperForce;
                Vector3 suspensionForceVector = (springForce - damperForce) * hit.normal;

                Vector3 wheelForceVector = wheels[i].getUpdatedForce(userInput, hit, Time.fixedDeltaTime, force);



                rb.AddForceAtPosition(suspensionForceVector + wheelForceVector, hit.point);
                wheelObjects[i].transform.position = hit.point + hit.normal * wheelRadius;
                
            }

        }
        
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.white;
        for(int i = 0; i<springs.Count; i++){
            Gizmos.DrawRay(springs[i].transform.position, -springLengths[i]*springs[i].transform.up);
            Gizmos.DrawRay(wheelObjects[i].transform.position, wheelObjects[i].transform.forward);
            Gizmos.DrawRay(wheelObjects[i].transform.position, wheelObjects[i].transform.right);    
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



}

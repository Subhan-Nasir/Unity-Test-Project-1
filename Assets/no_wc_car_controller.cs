using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class no_wc_car_controller : MonoBehaviour{
    public List<GameObject> springs;
    public List<GameObject> wheels;
    private Dictionary<GameObject, GameObject> points = new Dictionary<GameObject, GameObject>();
    private Rigidbody rb;

    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float dampingCoefficient;
    public float wheelRadius;

    private float minLength;
    private float maxLength;
    private float previousLength;
    private float springLength;
    private float springVelocity;
    private float springForce;
    private float damperForce;

    private Vector3 suspensionForce;

    private Vector3 force;
    private float dampingForce;
    private float damping;





    // Start is called before the first frame update
    void Start(){
        
        rb = transform.root.GetComponent<Rigidbody>();

        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;

        for(int i = 0; i < springs.Count; i++){
            points.Add(springs[i], wheels[i]);
        }
         
    }

    // Update is called once per frame
    void FixedUpdate(){
        foreach(GameObject spring in springs){

            GameObject wheel;
            bool found = points.TryGetValue(spring, out wheel);

            RaycastHit hit;
            if(Physics.Raycast(spring.transform.position, -transform.up, out hit, maxLength + wheelRadius)){

                // Debug.DrawRay(spring.transform.position, -transform.up *(springLength+wheelRadius),Color.green);


                previousLength = springLength;
                springLength = hit.distance - wheelRadius;
                springLength = Mathf.Clamp(springLength, minLength, maxLength);
                springVelocity = (previousLength - springLength) / Time.fixedDeltaTime;
                springForce = springStiffness * (restLength - springLength);
                dampingForce = dampingCoefficient * springVelocity;

                suspensionForce = (springForce + damperForce) * hit.normal;
                Debug.Log(springLength);
                rb.AddForceAtPosition(suspensionForce, hit.point);
                
                // if(found){
                //     wheel.transform.localPosition = new Vector3 (wheel.transform.localPosition.x, (wheelRadius - hit.distance), wheel.transform.localPosition.z);

                // }
            }
            // else if (found){
            //     wheel.transform.localPosition = new Vector3 (wheel.transform.localPosition.x, (wheelRadius - maxLength), wheel.transform.localPosition.z);


            // }
            
        }
        
    }


    void OnDrawGizmosSelected(){

        
        foreach(GameObject spring in springs){

            // Vector3 direction = -transform.up *(springLength+wheelRadius);
            // Gizmos.DrawRay(spring.transform.position, direction);

            Ray ray = new Ray(spring.transform.position, -transform.up);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ray.origin, -restLength * transform.up + spring.transform.position);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(-restLength * transform.up + spring.transform.position, -restLength * transform.up + spring.transform.position + transform.up * -wheelRadius);


        }
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    


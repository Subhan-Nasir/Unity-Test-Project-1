using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel{
    
    public float id;
    public GameObject wheelObject;
    public GameObject wheelMesh;
    public Rigidbody rb;
    public float wheelRadius;
    public float wheelMass;  


    public float slipAngle;
    public float slipRatio;
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

    public float alpha;
    public float omega;

    public Wheel(float id, GameObject wheelObject, GameObject wheelMesh, Rigidbody rb, float wheelRadius, float wheelMass, float D, float C, float B, float E){
        this.id = id;
        this.wheelObject = wheelObject;
        this.wheelMesh = wheelMesh;
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

    public Vector3 getUpdatedForce(float accel, RaycastHit hit, float timeDelta){

        wheelObject.transform.position = hit.point + hit.normal * wheelRadius;

        wheelVelocityLS = wheelObject.transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

        lateralVelocity = wheelVelocityLS.x;
        longitudinalVelocty = wheelVelocityLS.z;
                
        
        if(Mathf.Abs(longitudinalVelocty) < 0.2f){
            slipAngle = 0;
        }
        else{
            slipAngle = -Mathf.Atan(lateralVelocity / Mathf.Abs(longitudinalVelocty));
            
        }

        // Calculates driving force to the wheels.
        // if(id == 2 | id == 3){            
        //     longitudinalForce = (accel * (100/0.23f));
        // }
        // else{            
        //     longitudinalForce = (accel * (10/0.23f));
        // }

        if(id == 2 | id == 3){
            alpha = (accel * 100) * wheelRadius / 0.5f * wheelMass * Mathf.Pow(wheelRadius, 2);
            omega += alpha * timeDelta;            
            slipRatio = (longitudinalVelocty - omega * wheelRadius)/(omega * wheelRadius);
            longitudinalForce =  -tyreEquation(slipRatio, D, C, B, E);

            if(float.IsNaN(longitudinalForce)){
                longitudinalForce = 0;
            }

        }
        else{
            omega = longitudinalVelocty / wheelRadius;

        }

        wheelMesh.transform.Rotate(Mathf.Rad2Deg * omega * timeDelta, 0, 0, Space.Self);     

        lateralForce = tyreEquation(slipAngle, D, C, B, E);
        forceVector = longitudinalForce * wheelObject.transform.forward + lateralForce * wheelObject.transform.right;

        return forceVector;


    }


}
    


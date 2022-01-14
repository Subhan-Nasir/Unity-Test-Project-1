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

    public float D_long;
    public float C_long;
    public float B_long;
    public float E_long;
    public float c_long;
    public float m_long;

    public float D_lat;
    public float C_lat;
    public float B_lat;
    public float E_lat;


    public float alpha;
    public float omega;

    public Wheel(float id, GameObject wheelObject, GameObject wheelMesh, Rigidbody rb, float wheelRadius, float wheelMass, float[] longitudinalConstants, float[] lateralConstants){
        this.id = id;
        this.wheelObject = wheelObject;
        this.wheelMesh = wheelMesh;
        this.rb = rb;
        this.wheelRadius = wheelRadius;
        this.wheelMass = wheelMass;

        this.D_long = longitudinalConstants[0];
        this.C_long = longitudinalConstants[1];
        this.B_long = longitudinalConstants[2];
        this.E_long = longitudinalConstants[3];
        this.c_long = longitudinalConstants[4];
        this.m_long = longitudinalConstants[5];


        this.D_lat = lateralConstants[0];
        this.C_lat = lateralConstants[1];
        this.B_lat = lateralConstants[2];
        this.E_lat = lateralConstants[3];
              

    }

    public float tyreEquation(float slip, float D, float C, float B, float E){
        float force = D * Mathf.Sin( C * Mathf.Atan(B * slip - E * ( (B*slip) - Mathf.Atan(B*slip))));
        return force;  
    }

    public float complexTyreEquation(float slip, float verticalLoad, float D, float C, float B, float E, float c, float m){

        float force = (c*verticalLoad - m*Mathf.Pow(verticalLoad, 2)) * (D * Mathf.Sin( C * Mathf.Atan(B * slip - E * ( (B*slip) - Mathf.Atan(B*slip)))));
        return force;


    }

    public Vector3 getUpdatedForce(float accel, RaycastHit hit, float timeDelta, float verticalLoad){

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

        

        if(id == 2 | id == 3){
            alpha = (accel * 100) * wheelRadius / 0.5f * wheelMass * Mathf.Pow(wheelRadius, 2);
            omega += alpha * timeDelta;            
            slipRatio = (longitudinalVelocty - omega * wheelRadius)/Mathf.Abs(omega * wheelRadius);
            // longitudinalForce =  -tyreEquation(slipRatio, D_long, C_long, B_long, E_long);
            longitudinalForce = -complexTyreEquation(slipRatio, verticalLoad, D_long, C_long, B_long, E_long, c_long, m_long);

            if(float.IsNaN(longitudinalForce)){
                longitudinalForce = 0;
            }

        }
        else{
            omega = longitudinalVelocty / wheelRadius;

        }

        // Debug.Log($"wheel id = {id}: Force multiplier = {c_long*verticalLoad - m_long*Mathf.Pow(verticalLoad, 2)}");
        Debug.Log($"Wheel id = {id}: Longitudinal Force = {longitudinalForce}, Vertical Load = {verticalLoad}");
        wheelMesh.transform.Rotate(Mathf.Rad2Deg * omega * timeDelta, 0, 0, Space.Self);     

        lateralForce = tyreEquation(slipAngle, D_lat, C_lat, B_lat, E_lat);
        forceVector = longitudinalForce * wheelObject.transform.forward + lateralForce * wheelObject.transform.right;

        return forceVector;


    }


}
    


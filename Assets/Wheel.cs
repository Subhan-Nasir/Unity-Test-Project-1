using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

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
    public float c_lat;
    public float m_lat;


    public float alpha;
    public float omega;
    private List<float> slipRatioList = new List<float>();
    private List<float> longForceList = new List<float>();

    public Wheel(float id, GameObject wheelObject, GameObject wheelMesh, Rigidbody rb, float wheelRadius, float wheelMass, Dictionary<string, float> longitudinalConstants, Dictionary<string, float> lateralConstants){
        this.id = id;
        this.wheelObject = wheelObject;
        this.wheelMesh = wheelMesh;
        this.rb = rb;
        this.wheelRadius = wheelRadius;
        this.wheelMass = wheelMass;


        this.B_long = longitudinalConstants["B"];
        this.C_long = longitudinalConstants["C"];
        this.D_long = longitudinalConstants["D"];      
        this.E_long = longitudinalConstants["E"];
        this.c_long = longitudinalConstants["c"];
        this.m_long = longitudinalConstants["m"];


        this.B_lat = lateralConstants["B"];
        this.C_lat = lateralConstants["C"];
        this.D_lat = lateralConstants["D"];
        this.E_lat = lateralConstants["E"];
        this.c_lat = lateralConstants["c"];
        this.m_lat = lateralConstants["m"];
              

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
                
        
        if(Mathf.Abs(longitudinalVelocty) < 0.0001f){
            slipAngle = 0;
        }
        else{
            slipAngle = -Mathf.Atan(lateralVelocity / Mathf.Abs(longitudinalVelocty));
            
        }

        

        

        // if(id == 2 | id == 3){
        //     alpha = (accel * 100) * wheelRadius / 0.5f * wheelMass * Mathf.Pow(wheelRadius, 2);
        //     omega += alpha * timeDelta;            
        //     slipRatio = (longitudinalVelocty - omega * wheelRadius)/Mathf.Abs(omega * wheelRadius);
        //     // longitudinalForce =  -tyreEquation(slipRatio, D_long, C_long, B_long, E_long);
        //     longitudinalForce = -complexTyreEquation(slipRatio, verticalLoad, D_long, C_long, B_long, E_long, c_long, m_long);

        //     if(float.IsNaN(longitudinalForce)){
        //         longitudinalForce = 0;
        //     }

        // }
        // else{
        //     omega = longitudinalVelocty / wheelRadius;

        // }


        // FOR BRAKING
        if( accel < 0  & omega > 0){
            alpha = (accel * 250) * wheelRadius / 0.5f * wheelMass * Mathf.Pow(wheelRadius, 2);
            omega += alpha * timeDelta;  
            omega = Mathf.Clamp(omega, 0, 1000000);
        
            if(longitudinalVelocty > omega * wheelRadius){
                slipRatio = (omega * wheelRadius - longitudinalVelocty)/Mathf.Abs(longitudinalVelocty);               

            }     
            else if (longitudinalVelocty <= omega * wheelRadius){
                slipRatio = 0;
            }
            


            // longitudinalForce =  -tyreEquation(slipRatio, D_long, C_long, B_long, E_long);
            longitudinalForce = complexTyreEquation(slipRatio, verticalLoad, D_long, C_long, B_long, E_long, c_long, m_long);

        }
        else if(accel < 0 & (longitudinalVelocty <= 0.01f & omega == 0)){
            alpha = 0;
            omega += alpha * timeDelta;
            omega = Mathf.Clamp(omega, 0, 1000000);
            // rb.isKinematic = true;        
            
        }        
        else if(accel >= 0 ){
            rb.isKinematic = false;
            alpha = (accel * 100) * wheelRadius / 0.5f * wheelMass * Mathf.Pow(wheelRadius, 2);
            omega += alpha * timeDelta;
            omega = Mathf.Clamp(omega, 0, 1000000);
            
            if( id == 2 | id == 3){
                
                if(longitudinalVelocty < omega * wheelRadius){
                    slipRatio = (longitudinalVelocty - omega * wheelRadius)/Mathf.Abs(omega * wheelRadius);               

                }                
                else if (longitudinalVelocty >= omega * wheelRadius){
                    slipRatio = 0;
                }


                // longitudinalForce =  -tyreEquation(slipRatio, D_long, C_long, B_long, E_long);
                longitudinalForce = -complexTyreEquation(slipRatio, verticalLoad, D_long, C_long, B_long, E_long, c_long, m_long);
            }
            else{
                omega = longitudinalVelocty/wheelRadius;
            }

        }

                  
        
        if(float.IsNaN(longitudinalForce)){
            longitudinalForce = 0;
        }



        
        wheelMesh.transform.Rotate(Mathf.Rad2Deg * omega * timeDelta, 0, 0, Space.Self);     

        lateralForce = complexTyreEquation(slipAngle, verticalLoad, D_lat, C_lat, B_lat, E_lat, c_lat, m_lat);
        forceVector = longitudinalForce * wheelObject.transform.forward + lateralForce * wheelObject.transform.right;

        Debug.Log($"Wheel ID: {id}, alpha = {alpha}, Vertical Load = {verticalLoad}, F_long = {longitudinalForce}, F_lat = {lateralForce}, omega(deg/s) = {Mathf.Rad2Deg * omega}, RPM = {9.5493f * omega}, Slip Ratio = {slipRatio}, slip angle (deg) = {Mathf.Rad2Deg *slipAngle}  ");


        // Writes data to csv file.
        // if(id == 3){
        //     slipRatioList.Add(slipRatio);
        //     longForceList.Add(longitudinalForce);

        //     string filePath = "C:/Users/subha/Documents/Unity Projects/Test Project 1/Test Project 1/Data/RECORDED_FILE.csv";

        //     StreamWriter writer = new StreamWriter(filePath);
                
                
        //     for (int i = 0; i < slipRatioList.Count; ++i){
        //         writer.WriteLine(Time.realtimeSinceStartup + "," + longForceList[i] + "," + slipRatioList[i]);  
                          
        
        //     }

        //     writer.Flush();
        //     writer.Close();
                
        // }





        return forceVector;


    }


}
    


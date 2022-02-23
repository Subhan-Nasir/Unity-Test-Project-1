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
    public float momentOfInertia;
    public float totalDrivetrainInertia;  
    public float rrCoefficient = 0.0003f;


    public float slipAngle;
    public float slipRatio;
    public Vector3 wheelVelocityLS;        
    public float longitudinalVelocity;
    public float lateralVelocity;
    public float wheelTorque;
    public float engineTorque;
    public float brakingTorque;
    public float rollingResistance;
    public float torque;
    public float brakeBias;

    public float lateralForce; //Sideways direction    
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

    public float fLongLimit;
    public float fLatLimit;
    public float fLongDynamicLimit;
    public float fLatDynamicLimit;


    public float alpha;
    public float omega;

    public float verticalLoad;
    


    public Wheel(float id, GameObject wheelObject, GameObject wheelMesh, Rigidbody rb, float wheelRadius, float wheelMass, float brakeBias, float totalDrivetrainInertia, Dictionary<string, float> longitudinalConstants, Dictionary<string, float> lateralConstants){
        this.id = id;
        this.wheelObject = wheelObject;
        this.wheelMesh = wheelMesh;
        this.rb = rb;
        this.wheelRadius = wheelRadius;
        this.wheelMass = wheelMass;
        this.momentOfInertia = 0.5f * wheelMass * Mathf.Pow(wheelRadius, 2);
        this.brakeBias = brakeBias;
        this.totalDrivetrainInertia = totalDrivetrainInertia;


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

    public float tyreEquation(float slip, float B, float C, float D, float E){
        float force = D * Mathf.Sin( C * Mathf.Atan(B * slip - E * ( (B*slip) - Mathf.Atan(B*slip))));
        return force;  
    }

    public float complexTyreEquation(float slip, float fLimit, float C, float B, float E){

        float force = fLimit * Mathf.Sin( C * Mathf.Atan(B * slip - E * ( (B*slip) - Mathf.Atan(B*slip))));
        return force;

    }



    public float tyreCurvePeak(float c, float m, float D, float verticalLoad){
        return (c*verticalLoad - m*Mathf.Pow(verticalLoad, 2))*D;
    }

    public float dynamicPeakLongitudinal(float fLat, float fLongLimit, float fLatLimit){
        float dynamicPeak = Mathf.Abs(fLongLimit) * Mathf.Sqrt(1 - Mathf.Pow(fLat/fLatLimit, 2) );
        return dynamicPeak;

    }

    public float dynamicPeakLateral(float fLong, float fLongLimit, float fLatLimit){
        float dynamicPeak = Mathf.Abs(fLatLimit) * Mathf.Sqrt(1 - Mathf.Pow(fLong/fLongLimit, 2) );
        return dynamicPeak;

    }

    public float calculateSlipRatio( float velocity, float omega, float radius){
        float slipRatio;
        float wR = omega * radius;

        if(Mathf.Abs(wR) >= Mathf.Abs(velocity)){
            slipRatio = (wR - velocity)/Mathf.Abs(wR);                
        }            
        else{
            slipRatio = (wR - velocity)/Mathf.Abs(velocity);
        }

        // slipRatio = Mathf.Clamp(slipRatio, -1,1);
        return slipRatio;
    }

    public float calculateSlipAngle(float vLong, float vLat, float threshold){
        float slipAngle;
        if(Mathf.Abs(vLong) < threshold){
            slipAngle = 0;
        }
        else{
            slipAngle = -Mathf.Atan(vLat / Mathf.Abs(vLong));
            
        }
        return slipAngle;
    }

    public float getRollingResistance(float verticalLoad, float omega, float radius, float coefficient){
        return (verticalLoad * omega * radius * coefficient);
    }
    

    public Vector3 getUpdatedForce(float userInput, float currentGearRatio, float finalDriveRatio, float primaryGearRatio, RaycastHit hit, float timeDelta, float verticalLoad){
        
        this.verticalLoad = verticalLoad;
        // Debug.Log(userInput);
        if(verticalLoad < 0){
            verticalLoad = 0;
        }
       
        
        wheelObject.transform.position = hit.point + hit.normal * wheelRadius;
        wheelVelocityLS = wheelObject.transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

        lateralVelocity = wheelVelocityLS.x;
        longitudinalVelocity = wheelVelocityLS.z;              
               
        fLongLimit = 0.7f*tyreCurvePeak(c_long, m_long, D_long, verticalLoad);
        fLatLimit = 0.7f*tyreCurvePeak(c_lat, m_lat, D_lat, verticalLoad);
        
        fLongDynamicLimit = dynamicPeakLongitudinal(lateralForce, fLongLimit, fLatLimit);
        fLatDynamicLimit = dynamicPeakLateral(longitudinalForce, fLongLimit, fLatLimit);

        // if((id == 2 | id == 3) & userInput >=0){
        //     engineTorque = 100 * userInput;
            
        // }
        // else{
        //     engineTorque = 0;
        // }


        if(userInput <0){
            if(Mathf.Abs(longitudinalVelocity) > 1){
                if(id == 2 | id == 3){
                    brakingTorque = 400* userInput * (1 - brakeBias);
                }
                else{
                    brakingTorque = 400 * userInput * brakeBias;
                }

                
            }
            else if (Mathf.Abs(longitudinalVelocity) <= 1){
                brakingTorque = 400 * userInput ;

                if(id == 2 | id == 3){
                    brakingTorque = 400* userInput * longitudinalVelocity * (1 - brakeBias);
                }
                else{
                    brakingTorque = 400 * userInput * longitudinalVelocity * brakeBias;
                }
            }
            
            
        }
        else{
            brakingTorque = 0;
        }
     
        rollingResistance = -getRollingResistance(verticalLoad, omega, wheelRadius, rrCoefficient);

        // torque = engineTorque -getRollingResistance(verticalLoad, omega, wheelRadius, rrCoefficient) + brakingTorque - longitudinalForce*wheelRadius;
        
        torque = wheelTorque + rollingResistance + brakingTorque - longitudinalForce*wheelRadius;

        
        
        if(id == 2| id == 3){
            alpha = torque / totalDrivetrainInertia;
        }
        else{
            alpha = torque/momentOfInertia;
        }  
         

        omega += alpha * timeDelta;

        // if(userInput < 0){
            
        //     if(longitudinalVelocity > 0){
                
        //         omega = Mathf.Clamp(omega, (1/9.5493f) * 1650/(currentGearRatio * finalDriveRatio * primaryGearRatio), (1/9.5493f) * 14000/(currentGearRatio * finalDriveRatio * primaryGearRatio));
                
                
        //     }
        //     else if (longitudinalVelocity < 0){
        //         omega = Mathf.Clamp(omega, -10000000, 0);
        //     }          
            

        // }
        // else{
        //     omega = Mathf.Clamp(omega, 0, (1/9.5493f) * 14000/(currentGearRatio * finalDriveRatio * primaryGearRatio));
        // }   

        
        if(userInput >=0){
            if(id == 2 | id == 3){
                omega = Mathf.Clamp(omega, (1/9.5493f) * 1650/(currentGearRatio * finalDriveRatio * primaryGearRatio), (1/9.5493f) * 14000/(currentGearRatio * finalDriveRatio * primaryGearRatio));
            }
            else{
                omega = Mathf.Clamp(omega, -100000000f,10000000000f);
            }
            
        }
        else{
            omega = Mathf.Clamp(omega, 0,10000000000f);
        }
            
            
                
                
        
        
           
            
             

        wheelMesh.transform.Rotate(Mathf.Rad2Deg * omega * timeDelta, 0, 0, Space.Self); 

        
        slipRatio = calculateSlipRatio(longitudinalVelocity, omega, wheelRadius);        
        longitudinalForce = complexTyreEquation(slipRatio, fLongDynamicLimit, C_long, B_long, E_long);        

        slipAngle = calculateSlipAngle(longitudinalVelocity, lateralVelocity, threshold: 0.1f);     
        lateralForce = complexTyreEquation(slipAngle, fLatDynamicLimit, C_lat, B_lat, E_lat);
        

        

        if(float.IsNaN(longitudinalForce)){
            longitudinalForce = 0;
        }
        if(float.IsNaN(lateralForce)){
            lateralForce = 0;
        }

        forceVector = longitudinalForce * wheelObject.transform.forward + lateralForce * wheelObject.transform.right;
        
       
        // Debug.Log($"Wheel id = {id}, Longitudinal Velocity = {longitudinalVelocity}, wR = {omega*wheelRadius}, slip ratio = {slipRatio} ");
        
        // Debug.Log($" Wheel id = {id}, Limits = ({fLongLimit},{fLatLimit}), Dynamic Limits = ({fLongDynamicLimit},{fLatDynamicLimit}), Forces = ({longitudinalForce},{lateralForce}), Load = {verticalLoad}");
        // Debug.Log($"Wheel id = {id}, Longitudinal Velocity = {longitudinalVelocity}, Lateral Velocity = {lateralVelocity}, slip ratio = {slipRatio}, slip angle = {slipAngle}");


        return forceVector;


    }

    

}
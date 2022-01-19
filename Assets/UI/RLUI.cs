using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RLUI : MonoBehaviour{
    public GameObject carObject;
    
    [Header("UI")]
    public Text RPMLabel; // The label that displays the RPM;
    public Text steerAngleLabel;
    public Text torqueLabel;
    public Text forceLabel;
    public Text slipLabel;
    public Text omegaLabel;
    private int i;
    void Update(){
    RaycastController new_car = carObject.GetComponent<RaycastController>();
    
    
        Wheel[] wheelsList = new_car.getWheels();     

        i=2   ;

        if (RPMLabel != null){
           RPMLabel.text = Math.Round((9.5453*wheelsList[i].omega),2).ToString();
        }  

        // if (steerAngleLabel != null){
        //     steerAngleLabel.text = Math.Round(new_car.getSteeringAngleL(),2).ToString();
        // }  

        if (torqueLabel != null){
            torqueLabel.text = (0.5f*Mathf.Pow(wheelsList[i].wheelRadius,2)*wheelsList[i].wheelMass*wheelsList[i].alpha/(wheelsList[i].wheelRadius)).ToString();
        }  

        if (forceLabel != null){
            forceLabel.text = "(" + Math.Round(wheelsList[i].lateralForce,2).ToString()+ ","+ Math.Round(wheelsList[i].longitudinalForce,2).ToString()+")";
        }  

        if (slipLabel != null){
            slipLabel.text = "(" + Math.Round(wheelsList[i].slipAngle,2).ToString()+ ","+ Math.Round(wheelsList[i].slipRatio,2).ToString()+")";
        }     

        if (omegaLabel != null){
            omegaLabel.text = Math.Round(wheelsList[i].omega,2).ToString();
        }          
     
    }
      
  
}

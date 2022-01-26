using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RRUI : MonoBehaviour{
    public GameObject carObject;
    
    [Header("UI")]
    public Text RPMLabel; // The label that displays the RPM;
    public Text steerAngleLabel;
    public Text torqueLabel;
    public Text forceLabel;
    public Text slipLabel;
    public Text omegaLabel;
    public Text VLoadlabel;
    private int i;
    void Update(){
    RaycastController new_car = carObject.GetComponent<RaycastController>();
    
    
        Wheel[] wheelsList = new_car.getWheels();
        Suspension[] suspensionList = new_car.getSuspensions();     

        i=3   ;

        if (RPMLabel != null){
           RPMLabel.text = Math.Round((9.5453*wheelsList[i].omega),2).ToString();
        }  

        // if (steerAngleLabel != null){
        //     steerAngleLabel.text = Math.Round(new_car.getSteeringAngleL(),2).ToString();
        // }  

        if (torqueLabel != null){
            torqueLabel.text = wheelsList[i].torque.ToString();
        }  

        if (forceLabel != null){
            forceLabel.text = "(" + Math.Round(wheelsList[i].lateralForce,2).ToString()+ ","+ Math.Round(wheelsList[i].longitudinalForce,2).ToString()+")";
        }  

        if (slipLabel != null){
            slipLabel.text = "(" + Math.Round(Mathf.Rad2Deg*wheelsList[i].slipAngle,2).ToString()+ ","+ Math.Round(wheelsList[i].slipRatio,2).ToString()+")";
        }     

        if (omegaLabel != null){
            omegaLabel.text = Math.Round(Mathf.Rad2Deg*wheelsList[i].omega,2).ToString();
        }          
        if (VLoadlabel != null){
            VLoadlabel.text = suspensionList[i].force.magnitude.ToString();
        }            
     
    }
      
  
}

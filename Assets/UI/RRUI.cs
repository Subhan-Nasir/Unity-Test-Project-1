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


    private float nextUpdate=0.069f;
    void Update(){
        if(Time.time>=nextUpdate){
            nextUpdate=Time.time+0.069f;
        
            RaycastController new_car = carObject.GetComponent<RaycastController>();
    
    
            Wheel[] wheelsList = new_car.getWheels();
            Suspension[] suspensionList = new_car.getSuspensions();     

            i=3   ;

            if (RPMLabel != null){
            RPMLabel.text = Math.Round((9.5453*wheelsList[i].omega),0).ToString();
            }  

            if (steerAngleLabel != null){
                steerAngleLabel.text = 0.ToString() + "º";
            }  

            if (torqueLabel != null){
                torqueLabel.text = Math.Round(wheelsList[i].torque,1).ToString();
            }  

            if (forceLabel != null){
                forceLabel.text = "(" + Math.Round(wheelsList[i].lateralForce,1).ToString()+ ","+ Math.Round(wheelsList[i].longitudinalForce,1).ToString()+")";
            }  

            if (slipLabel != null){
                slipLabel.text = "(" + Math.Round(Mathf.Rad2Deg*wheelsList[i].slipAngle,1).ToString()+ ","+ Math.Round(wheelsList[i].slipRatio,1).ToString()+")";
            }     

            if (omegaLabel != null){
                omegaLabel.text = Math.Round(Mathf.Rad2Deg*wheelsList[i].omega,1).ToString();
            }      

            if (VLoadlabel != null){
                VLoadlabel.text = Math.Round(wheelsList[i].verticalLoad,0).ToString();
            }                
        }
    }
}

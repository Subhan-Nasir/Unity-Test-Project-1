using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WheelUI : MonoBehaviour{
    public GameObject carObject;
    
    [Header("UI")]
    public Text RPMLabel; // The label that displays the RPM;
    public Text steerAngleLabel;
    public Text torqueLabel;
    public Text forceLabel;
    public Text slipLabel;
    public Text omegaLabel;
    void Update(){
    //no_wc_car_controller car = carObject.GetComponent<no_wc_car_controller>();
    RaycastController new_car = carObject.GetComponent<RaycastController>();
    
    
        private Wheel[] wheelsList = new_car.getWheels();        

        if (omegaLabel != null){
            omegaLabel.text = wheelsList[0].omega.ToString();
        }          
     
    }
      
  
}

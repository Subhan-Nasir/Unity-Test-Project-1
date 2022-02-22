using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class VehicleUI : MonoBehaviour
{
    public Rigidbody target;

    [Header("UI")]
    public Text speedLabel; // The label that displays the speed;
    public Text accelerationLabel;
    private float speed = 0.0f;
    private float acceleration = 0.0f;
    private float lastVelocity;
    private void Update()
    {
        // 3.6f to convert in kilometers
        // ** The speed must be clamped by the car controller **
        speed = target.velocity.magnitude;
        acceleration = (speed - lastVelocity) / Time.deltaTime;
        lastVelocity = speed;
        
        

        if (speedLabel != null)
            speedLabel.text = (Math.Round(speed*2.237, 1)).ToString() + " mph";

       if (accelerationLabel != null)
            accelerationLabel.text = Math.Round(acceleration,2)+ "m/s^2";              
    }
}
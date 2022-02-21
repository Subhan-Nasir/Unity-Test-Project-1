using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Tachometer : MonoBehaviour
{
    public Rigidbody target;
    public float minRPM = 0.0f;
    public float maxRPM = 0.0f; 
    public float minRPMArrowAngle;
    public float maxRPMArrowAngle;

    private Transform RPMLabelTemplateTransform;

    [Header("UI")]
    public Text speedLabel; // The label that displays the RPM;
    public RectTransform arrow; // The arrow in the Tachometer
    public Text gearLabel;

    private float rpm = 0.0f;
    private int gear =0;
    private float speed = 0.0f;

    private void Awake() {
        RPMLabelTemplateTransform= transform.Find("RPMLabelTemplate");
        RPMLabelTemplateTransform.gameObject.SetActive(false);

        CreateRPMLabels();
    }


    private void Update()
    {
        RaycastController carController=target.GetComponent<RaycastController>();
        rpm=carController.getEngineRPM()/1000;
        gear=carController.getCurrentGear();
        //rpm = GameObject.Find("Raycast Reworked").GetComponent<RaycastController>().engineRPM/1000;
       // gear=GameObject.Find("Raycast Reworked").GetComponent<RaycastController>().currentGear;
        speed=target.velocity.magnitude*2.237f;

        if (speedLabel != null)
            speedLabel.text = Math.Round(speed,1).ToString();
        if (arrow != null)
            arrow.localEulerAngles =
                new Vector3(0, 0, Mathf.Lerp(minRPMArrowAngle, maxRPMArrowAngle, rpm / maxRPM));
        if (gearLabel!=null)
            gearLabel.text=gear.ToString();
    }

    private void CreateRPMLabels(){
        int labelAmount=14;
        float totalAngleSize= minRPMArrowAngle-maxRPMArrowAngle;
        float rpmNormalised= rpm/maxRPM;

        for (int i=0; i<=labelAmount; i++){
            Transform RPMLabelTransform = Instantiate(RPMLabelTemplateTransform,transform);
            float labelRPMNormalised= (float)i/labelAmount;
            RPMLabelTransform.eulerAngles = new Vector3(0,0,minRPMArrowAngle-labelRPMNormalised * totalAngleSize);
            RPMLabelTransform.Find("RPMLabelText").GetComponent<Text>().text=Mathf.RoundToInt(labelRPMNormalised * maxRPM).ToString();
            //RPMLabelTransform.Find("RPMLabelText").eulerAngles= Vector3.zero;
            RPMLabelTransform.gameObject.SetActive(true);
        }
    }
}

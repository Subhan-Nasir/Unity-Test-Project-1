using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Suspension{

    public float id;
    public float naturalLength;
    public float springTravel;
    public float springStiffness;       
    public float dampingCoefficient;
    public float bumpStiffness; 
    public float bumpTravel;
    public float wheelRadius;
        
    public float minLength;
    public float maxLength;
    public float springLength;
    public float previousLength;
    public float springVelocity;

    public float springForce;
    public float damperForce;
    public float force;
    public Vector3 forceVector; 
    public bool contact;

    public Suspension(float id, float restLength, float springTravel, float springStiffness, float dampingCoefficient, float bumpStiffness, float bumpTravel, float wheelRadius){
        this.id = id;
        this.naturalLength = restLength;
        this.springTravel = springTravel;
        this.springStiffness = springStiffness;        
        this.dampingCoefficient = dampingCoefficient;
        this.bumpStiffness = bumpStiffness;
        this.bumpTravel = bumpTravel;
        this.wheelRadius = wheelRadius;
        
        this.minLength = restLength - springTravel;
        this.maxLength = restLength + springTravel;
        this.springLength = restLength;
        this.previousLength = restLength;
        
    
    }

    

    public Vector3 getUpdatedForce(RaycastHit hit, float timeDelta, bool contact){
        this.contact = contact;
        previousLength = springLength;
        springLength = hit.distance - wheelRadius;
        springLength = Mathf.Clamp(springLength, minLength - bumpTravel, maxLength);
        springVelocity = (springLength - previousLength)/timeDelta;

        if(springLength < minLength){
            springForce = springStiffness * (naturalLength - springLength) + bumpStiffness * (minLength - springLength);
                       
        }
        else{
            springForce = springStiffness * (naturalLength - springLength);
        }

        // springForce = springStiffness * (naturalLength - springLength);  
        
        
        damperForce = dampingCoefficient * springVelocity;
        force = springForce - damperForce;
        forceVector = (springForce - damperForce) * hit.normal;

        
        

        // Debug.Log($"Spring id = {id}, Rest Length ={restLength}, current length = {springLength}, suspension force = {springForce - damperForce}");
        
        // Debug.Log($"Suspension {id}: force = {force}, force vector = {forceVector}, spring force = {springForce}, damper force = {damperForce}, length = {springLength}, velocity = {springVelocity}");
        return forceVector;

    }


    
}

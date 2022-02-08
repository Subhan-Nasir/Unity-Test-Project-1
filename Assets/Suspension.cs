using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Suspension{

    public float id;
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float dampingCoefficient;
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

    public Suspension(float id, float restLength, float springTravel, float springStiffness, float dampingCoefficient, float wheelRadius){
        this.id = id;
        this.restLength = restLength;
        this.springTravel = springTravel;
        this.springStiffness = springStiffness;
        this.dampingCoefficient = dampingCoefficient;
        this.wheelRadius = wheelRadius;
        
        this.minLength = restLength - springTravel;
        this.maxLength = restLength + springTravel;
        this.springLength = restLength;
        this.previousLength = restLength;
        
    
    }

    

    public Vector3 getUpdatedForce(RaycastHit hit, float timeDelta){
        previousLength = springLength;
        springLength = hit.distance - wheelRadius;
        springLength = Mathf.Clamp(springLength, minLength, maxLength);
        springVelocity = (springLength - previousLength)/timeDelta;
        
        springForce = springStiffness * (restLength - springLength);
        damperForce = dampingCoefficient * springVelocity;
        force = springForce - damperForce;
        forceVector = (springForce - damperForce) * hit.normal;

        
        

        // Debug.Log($"Spring id = {id}, Rest Length ={restLength}, current length = {springLength}, suspension force = {springForce - damperForce}");
        
        // Debug.Log($"Suspension {id}: force = {force}, force vector = {forceVector}, spring force = {springForce}, damper force = {damperForce}, length = {springLength}, velocity = {springVelocity}");
        return forceVector;

    }


    
}

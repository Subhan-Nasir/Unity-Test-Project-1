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
    public Vector3 force = new Vector3();

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
        force = (springForce - damperForce) * hit.normal;
        
        
        return force;

    }


    
}

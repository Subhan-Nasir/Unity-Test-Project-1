using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_controller : MonoBehaviour
{
    public Transform car;
    public float smoothing = 5f;
    Vector3 offSet;
    public float z;

    // Use this for initialization
    void Awake()
    {
        offSet = transform.position - car.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 camPos = car.position + offSet;
        camPos.z = camPos.z - z;
        transform.position = Vector3.Lerp(transform.position, camPos, smoothing * Time.deltaTime);

    }
}

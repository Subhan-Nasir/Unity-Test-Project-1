using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCarSound : MonoBehaviour
{
    private float audioPitch;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
       audioSource=GetComponent<AudioSource>(); 
    }

    // Update is called once per frame
    void Update()
    {
      audioSource.pitch=(7*RaycastController.cc.getEngineRPM()/12350)+0.06478f;  
    }
}

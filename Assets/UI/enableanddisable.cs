using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableanddisable : MonoBehaviour
{
    public GameObject UIElement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void whenButtonClick(){
        if (UIElement.activeInHierarchy == true)
            UIElement.SetActive(false);
        else
            UIElement.SetActive(true);

    }
}

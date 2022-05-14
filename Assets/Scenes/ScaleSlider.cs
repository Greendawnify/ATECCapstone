using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class ScaleSlider : MonoBehaviour
{

    ARSessionOrigin arOrign;
    // Start is called before the first frame update
    void Start()
    {
        arOrign = FindObjectOfType<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSlider(Slider slider) {
        arOrign.transform.localScale = new Vector3(slider.value, slider.value, slider.value);
    }
}

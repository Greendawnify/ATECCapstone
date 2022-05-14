using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTilingOffset : MonoBehaviour
{
    Renderer rend;                              // the render on this object

    bool change = false;                        // if it is time to chnage the offset
    Vector2 changeVector;                       // value that is used to chnage the offset

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // changing the texture offset
        if (change)
        {
            changeVector += new Vector2(0f, 0.02f);
            rend.material.SetTextureOffset("_MainTex", changeVector);
        }
    }

    /// <summary>
    /// if the object is on start chnaging offset process and reset the offset
    /// </summary>
    private void OnEnable()
    {
        change = true;
        if(rend != null)
            rend.material.SetTextureOffset("_MainTex", Vector2.zero);
    }

    /// <summary>
    /// when obj is turned off toggle off processand reset the texture off set
    /// </summary>
    private void OnDisable()
    {
        change = false;
        if (rend != null) {
            rend.material.SetTextureOffset("_MainTex", Vector2.zero);
        }
    }
}

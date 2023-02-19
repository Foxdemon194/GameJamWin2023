using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSon : MonoBehaviour
{
    bool started = false;

    // Update is called once per frame
    void LateUpdate()
    {
        if (!started)
        {
            GetComponentInParent<AudioManagerScript>().Play("MainTheme");
            started = true;
            Debug.Log("I did my thing");
        }
        else
            return;
    }
}

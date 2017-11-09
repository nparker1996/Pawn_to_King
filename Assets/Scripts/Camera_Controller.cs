using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour {

    [SerializeField] private GameObject sideCamera;
    [SerializeField] private GameObject whiteCamera;
    [SerializeField] private GameObject blackCamera;
    private int cameraCount = 0; //which camera is enabled

    // Use this for initialization
    void Start () {
        whiteCamera.GetComponent<Camera>().enabled = false;
        blackCamera.GetComponent<Camera>().enabled = false;
        sideCamera.GetComponent<Camera>().enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C)) //change camera
        {
            cameraCount++;//increment camera
            cameraCount %= 3;

            //resets all cameras
            whiteCamera.GetComponent<Camera>().enabled = false;
            blackCamera.GetComponent<Camera>().enabled = false;
            sideCamera.GetComponent<Camera>().enabled = false;

            switch (cameraCount)
            {
                case 1: //white camera
                    whiteCamera.GetComponent<Camera>().enabled = true;
                    break;
                case 2: //black camera
                    blackCamera.GetComponent<Camera>().enabled = true;
                    break;
                default: //side camera
                    sideCamera.GetComponent<Camera>().enabled = true;
                    break;
            }

        }
    }
}

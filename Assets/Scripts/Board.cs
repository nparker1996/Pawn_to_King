using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    private GameObject worldController;

    // Use this for initialization
    void Start() {
        worldController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update() {

    }

    void OnMouseDown()
    {
        worldController.GetComponent<Game>().removeSelected();
    }
}

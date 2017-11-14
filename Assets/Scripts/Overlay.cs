using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlay : MonoBehaviour {

    private GameObject worldController;
    private int x; //x location of a piece
    private int y; //y location of a piece

    // Use this for initialization
    void Start ()
    {
        worldController = GameObject.FindGameObjectWithTag("GameController");
    }
	
	// Update is called once per frame
	void Update () {}

    void OnMouseDown()
    {
        worldController.GetComponent<Game>().clickedTile(x, y);
    }

    public int getX()//return x location
    {
        return x;
    }

    public int getY()//return y location
    {
        return y;
    }

    public int[] getLocation() //return location of the piece
    {
        return new int[] { x, y };
    }

    public void setLocation(int X, int Y) //sets the location of the piece
    {
        x = X;
        y = Y;
        this.gameObject.transform.position = new Vector3(X, 0, Y);
    }
}

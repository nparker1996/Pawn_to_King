using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Agent {

	// Use this for initialization
	void Start ()
    {
        type = "Human";
    }
	
	// Update is called once per frame
	void Update () {}

    public Human(Game theGame, bool team) : base(theGame, team) { }

    public override void turn() //nothing happens
    {
        printTurn();
    }
}

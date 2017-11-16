using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neural_Network_AI : Agent
{

    private double MOD_NUMBER_MOVES = 40;

    // Use this for initialization
    void Start() {
        type = "Neural Network";
    }

    public Neural_Network_AI(Game theGame, bool team) : base(theGame, team)
    {
        type = "Neural Network";
    }

	// Update is called once per frame
	void Update () {
		
	}

    public override void turn() { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable_Modifier_AI : Agent {

	// Use this for initialization
	void Start () {
        type = "Variable";
    }

    public Variable_Modifier_AI(Game theGame, bool team) : base(theGame, team)
    {
        type = "Variable";
    }

    // Update is called once per frame
    void Update () {
		
	}

    public override void turn() { }
}

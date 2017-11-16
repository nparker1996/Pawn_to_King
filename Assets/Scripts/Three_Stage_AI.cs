using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Three_Stage_AI : Agent
{

    int stage;

    private double MOD_OPENING_MOVEMENT = 1; //the more movement the piece will have, the better
    private double MOD_OPENING_KING_CASTLE = 2; //if a king can castle, that is good
    private double MOD_OPENING_CONTROL_CENTER = .5; //control the center of the board
    private double MOD_OPENING_PAWN_GUARD = 1; //pawns defend each other

    // Use this for initialization
    void Start () {
        type = "Three Stage";
        stage = 0;
    }

    public Three_Stage_AI(Game theGame, bool team) : base(theGame, team)
    {
        type = "Three Stage";
        stage = 0;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public override void turn() { }

}

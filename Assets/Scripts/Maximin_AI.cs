using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maximin_AI : Agent {

    private int Alpha_Beta_Depth = 12;
    private int IDDFS = 2; //Iterative Deeper Depth-First Search
    private int nullMoveDepth = 2;

    private double MOD_ADD_TAKE_PIECE = 1; //the multiplier for the value of being able to take an enemy piece
    private double MOD_MINUS_TAKE_PIECE = 2; //the multiplier for the value of being able to take an ally piece
    private double MOD_ADD_PIECE = .5; //the modifier for an ally piece existing
    private double MOD_MINUS_PIECE = .5; //the modifier for an enemy piece existing
    private double MOD_CONTROL_CENTER = .5;

    private Piece[] killerPieces;
    private int[][] killerMoves;

    // Use this for initialization
    void Start () {
        type = "Maximin";
        killerPieces = new Piece[Alpha_Beta_Depth];
        killerMoves = new int[Alpha_Beta_Depth][];
    }

    public Maximin_AI(Game theGame, bool team) : base(theGame, team)
    {
        type = "Maximin";
        killerPieces = new Piece[Alpha_Beta_Depth];
        killerMoves = new int[Alpha_Beta_Depth][];
    }

    // Update is called once per frame
    void Update () {
		
	}

    public override void turn()
    {
        printTurn();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maximin_AI : Agent {

    //private Game testingGame;

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
       
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="piece">the piece that is being moved for this node</param>
    /// <param name="tile">the location the piece is getting moved to</param>
    /// <param name="level">the depth that the node is at. Used for killerMove. ex. sources = 0</param>
    /// <param name="depth">how far down must be searched</param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <param name="maximizingPlayer">the player that wants the best score possible</param>
    /// <param name="currentPlayer">the current player being evaluated</param>
    /// <param name="nullMove">use or not use null-move heuristic</param>
    /// <returns></returns>
    private double alphaBeta(Piece piece, int[] tile, int level, int depth, double alpha, double beta, bool maximizingPlayer, bool currentPlayer, bool nullMove)//alpha-beta pruning method
    {
        return 0.0f;
    }

    private double calculateHeuristicValue(bool theTeam)
    {
        return 0;
    }

    private bool nullMoveHeuristic(int level, double alpha, double beta, bool maximizingPlayer, bool currentPlayer)//figures out if null-move heuristic is nessary here
    {
        return false;
    }

}

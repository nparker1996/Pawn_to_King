using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_AI : Agent
{

	// Use this for initialization
	void Start () {
        type = "Random";
    }

    public Random_AI(Game theGame, bool team) : base(theGame, team)
    {
        type = "Random";
    }

    // Update is called once per frame
    void Update () {
        List<Piece> listOfPieces = new List<Piece>();
        List<int[]> listOfMoves = new List<int[]>();
        foreach (Piece p in pieces)//puts all moves into List to pick from
        {
            List<int[]> possibleMoves = game.getPossibleMoves(p);
            possibleMoves = game.willMakeCheck(p, possibleMoves);// checks to see if piece moves to a spot will it chose a check?
            if (possibleMoves.Count > 0)
            {
                foreach (int[] tile in possibleMoves)//foreach tile that the piece can move to, calcute value
                {
                    listOfPieces.Add(p);
                    listOfMoves.Add(tile);
                }
            }
        }
        printTurn();
        //choose move
        if (listOfPieces.Count > 0)
        {
            int r = new System.Random().Next(listOfPieces.Count);
            //Console.WriteLine(" (" + listOfPieces[r].getX() + ", " + listOfPieces[r].getY() + ") at" + " (" + listOfMoves[r][0] + ", " + listOfMoves[r][1] + ")");
            game.clickedPieceAI(listOfPieces[r], listOfMoves[r][0], listOfMoves[r][1]);
        }
        else
        {
            game.check = true;
            game.checkmate = true;
            game.updateText(team);
            return;
        }
    }

    public override void turn() { }
}

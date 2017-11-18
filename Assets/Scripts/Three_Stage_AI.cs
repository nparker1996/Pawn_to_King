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

    public override void turn() {
        stageChange();//determine if stage of the game needs to be switched
        Dictionary<Piece, List<double[]>> optimalChoices = new Dictionary<Piece, List<double[]>>(); //each piece with its moves and utilites (value) 
        double highestValue = -1000; // the highest value that has been returned
        printTurn();

        //////////////////////////////////////////////////////
        ///for each move, figure out what opponent could do///
        foreach (Piece p in pieces)
        {
            List<int[]> possibleMoves = game.getPossibleMoves(p, game.board);
            possibleMoves = game.willMakeCheck(p, possibleMoves, game.board);// checks to see if piece moves to a spot will it chose a check?
            if (possibleMoves.Count > 0)
            {
                List<double[]> movesAndValues = new List<double[]>(); //the place the piece can move and the value it has
                foreach (int[] move in possibleMoves)
                {
                    double value = 0;
                    Piece[,] temptBoard = new Piece[8, 8]; //makes a copy of board
                    System.Array.Copy(game.board, temptBoard, 64);
                    List<int[]> canMove = new List<int[]>();
                    int oldX = p.getX();
                    int oldY = p.getY();
                    temptBoard[p.getX(), p.getY()] = null;
                    temptBoard[move[0], move[1]] = p;
                    p.setLocation(move[0], move[1], false);

                    switch (stage)
                    {
                        case 0:
                            value = opening(p, move, temptBoard);
                            break;
                        case 2:
                            value = endGame(p, move, temptBoard);
                            break;
                        case 1:
                        default:
                            value = middle(p, move, temptBoard);
                            break;
                    }

                    p.setLocation(oldX, oldY, false);

                    Debug.Log(value + " : " + "(" + p.getX() + ", " + p.getY() + ") at" + " (" + move[0] + ", " + move[1] + ") " + p.getType());
                    if (value > highestValue)
                    {
                        highestValue = value;
                    }
                    movesAndValues.Add(new double[] { move[0], move[1], value });
                }
                optimalChoices.Add(p, movesAndValues);
            }
        }

        ////////////////////////////////////////////////
        ///determine the move with highest move value///
        List<Piece> bestMovesPieces = new List<Piece>();
        List<double[]> bestMovesMove = new List<double[]>();
        Debug.Log("Best Moves");
        foreach (Piece p in pieces)
        {
            if (optimalChoices.ContainsKey(p))
            {
                foreach (double[] d in optimalChoices[p])
                {
                    if (d[2] >= highestValue)
                    {
                        Debug.Log(" (" + p.getX() + ", " + p.getY() + ") at" + " (" + d[0] + ", " + d[1] + ")");
                        bestMovesPieces.Add(p);
                        bestMovesMove.Add(d);
                    }
                }
            }
        }
        if (bestMovesPieces.Count > 0)
        {
            int r = new System.Random().Next(bestMovesPieces.Count);
            //Move piece to spot with highest move value

            game.clickedPieceAI(bestMovesPieces[r], (int)bestMovesMove[r][0], (int)bestMovesMove[r][1]);
        }
        else
        {
            game.check = true;
            game.checkmate = true;
            game.updateText(team);
            return;
        }
    }

    private void stageChange() //determine if stage of the game needs to be switched
    {
        switch (stage)
        {
            case 0://opening --> middle
                if (game.whiteTeam.pieces.Count < 16 || game.blackTeam.pieces.Count < 16 || game.check)//a piece as been taken or there is a check
                {
                    stage = 1;
                }
                break;
            case 1://middle --> end game
                if (pieces.Count < 8)
                {
                    stage = 2;
                }
                break;
            case 2: //end game
            default:
                return;
        }
    }

    private double opening(Piece p, int[] m, Piece[,] temptBoard)//the first few moves of the game, stage of the game in which players develop their pieces, get their king to safety, and attempt to control the center
    {
        double v = 0;

        List<int[]> moves = game.getPossibleMoves(p, temptBoard);

        if (moves.Count / p.getValue() > 1) //the more movement it above a point, the better
        {
            v += ((int)(moves.Count / p.getValue())) * MOD_OPENING_MOVEMENT;
        }

        if (p.getType() == 5)//king to castle
        {
            if ((p.getX() == 1 || p.getX() == 5))
            {
                v += MOD_OPENING_KING_CASTLE;
            }
        }

        foreach (int[] move in moves)//each move that the piece could move
        {
            if (p.getType() != 0)
            {
                if ((move[0] == 3 || move[0] == 4) && (move[1] == 3 || move[1] == 4)) //will control the center 4 tiles
                {
                    v += MOD_OPENING_CONTROL_CENTER;
                }
            }




        }

        if (p.getType() == 0)//piece is a pawn
        {
            v += game.pawnTakePiece(p, false, temptBoard).Count * MOD_OPENING_PAWN_GUARD;
        }


        return v;
    }

    private double middle(Piece p, int[] m, Piece[,] temptBoard)//body of game, when players begin to attack each other, and defend
    {
        double v = 0;

        List<int[]> moves = game.getPossibleMoves(p, temptBoard);
        foreach (int[] move in moves)//each move that the piece could move
        {





        }

        return v;
    }

    private double endGame(Piece p, int[] m, Piece[,] temptBoard)//the last few moves, when most of the pieces are off of the board
    {
        double v = 0;

        List<int[]> moves = game.getPossibleMoves(p, temptBoard);
        foreach (int[] move in moves)//each move that the piece could move
        {





        }

        return v;
    }




}

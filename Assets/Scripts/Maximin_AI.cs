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
        Dictionary<Piece, List<double[]>> optimalChoices = new Dictionary<Piece, List<double[]>>(); //each piece with its moves and utilites (value) 
        double highestValue = -1000; // the highest value that has been returned
        double highestShallowValue = -1000;//used to evalutes shallow searches to optimize what ones should be used

        printTurn();

        ///////////////////////////////////////////////
        ///Figures outs all possible moves of pieces///
        foreach (Piece p in pieces)
        {
            List<int[]> possibleMoves = game.getPossibleMoves(p, game.board);
            possibleMoves = game.willMakeCheck(p, possibleMoves, game.board);// checks to see if piece moves to a spot will it chose a check?
            if (possibleMoves.Count > 0)
            {
                List<double[]> movesAndValues = new List<double[]>(); //the place the piece can move and the value it has
                foreach (int[] move in possibleMoves)
                {
                    double alphBetaShallowValue = alphaBeta(p, move, 0, IDDFS, -1000, 1000, team, team, true);
                    if (alphBetaShallowValue >= highestShallowValue)
                    {
                        double alphBetaValue = alphaBeta(p, move, 0, Alpha_Beta_Depth, -1000, 1000, team, team, true);
                        movesAndValues.Add(new double[] { move[0], move[1], alphBetaValue });
                        Debug.Log(alphBetaValue + " : " + "(" + p.getX() + ", " + p.getY() + ") at" + " (" + move[0] + ", " + move[1] + ") " + p.getType());
                        if (alphBetaValue > highestValue)
                        {
                            highestValue = alphBetaValue;
                            highestShallowValue = alphBetaShallowValue;
                        }
                    }
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
        //killerPieces = new Piece[Alpha_Beta_Depth];
        //killerMoves = new int[Alpha_Beta_Depth][];
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
        int oldX = piece.getX();
        int oldY = piece.getY();
        Piece[,] temptBoard = new Piece[8, 8]; //makes a copy of board
        System.Array.Copy(game.board, temptBoard, 64);
        temptBoard[piece.getX(), piece.getY()] = null;
        temptBoard[tile[0], tile[1]] = piece; //move piece
        piece.setLocation(tile[0], tile[1], false);

        double heuristicValue = 0;//the value of the current situation

        //end of the line
        if (depth == 0)//gone as far as needed
        {
            heuristicValue = calculateHeuristicValue(currentPlayer, temptBoard);
            piece.setLocation(oldX, oldY, false);
            return heuristicValue;
        }
        else if (game.getPossibleMovesForTeam(currentPlayer, false, temptBoard).Count <= 0)
        {
            if (game.checkForChecks(currentPlayer, piece, temptBoard))
            {
                heuristicValue = calculateHeuristicValue(currentPlayer, temptBoard);
                piece.setLocation(oldX, oldY, false);
                return heuristicValue;
            }
        }
        if (nullMove)
        {//if false, then already inside a nullMoveHeuristic
            if (nullMoveHeuristic(level, alpha, beta, maximizingPlayer, currentPlayer, temptBoard))
            {
                heuristicValue = calculateHeuristicValue(currentPlayer, temptBoard);
                piece.setLocation(oldX, oldY, false);
                return heuristicValue;
            }
        }
        //else if (game.checkForCheckmates(currentPlayer)) //the game ends
        //{
        //    heuristicValue = calculateHeuristicValue(currentPlayer, temptBoard) - 10;
        //    piece.setLocation(oldX, oldY, false);
        //    return heuristicValue;
        //}
        //else if (game.checkForCheckmates(!currentPlayer)) //the game ends
        //{
        //    heuristicValue = calculateHeuristicValue(currentPlayer, temptBoard) + 10;
        //    piece.setLocation(oldX, oldY, false);
        //    return heuristicValue;
        //}
        //else if (game.checkForStalemates(currentPlayer) || game.checkForStalemates(!currentPlayer)) //the game ends
        //{
        //    heuristicValue = calculateHeuristicValue(currentPlayer, temptBoard) - 5;
        //    piece.setLocation(oldX, oldY, false);
        //    return heuristicValue;
        //}

        //doing more calculations
        if (maximizingPlayer == currentPlayer)//want to maximinize
        {
            double v = -1000;
            for (int i = 0; i < pieces.Count; i++)
            {
                Piece p = pieces[i];
                List<int[]> possibleMoves = game.getPossibleMoves(p, temptBoard);
                possibleMoves = game.willMakeCheck(p, possibleMoves, temptBoard);// checks to see if piece moves to a spot will it chose a check?
                if (possibleMoves.Count > 0)
                {

                    foreach (int[] move in possibleMoves)
                    {
                        //killer heuristic
                        if (killerPieces[level] != null && depth < Alpha_Beta_Depth)//not the first time
                        {
                            if (killerPieces[level] == p)//it is the same piece
                            {
                                if (killerMoves[level][0] == move[0] && killerMoves[level][1] == move[1])//it is moving to the same position
                                {
                                    heuristicValue = calculateHeuristicValue(currentPlayer, temptBoard);
                                    piece.setLocation(oldX, oldY, false);
                                    //Debug.Log("beta killer move : " + depth + " : (" + p.getX() + ", " + p.getY() + ")");
                                    return heuristicValue;
                                }
                            }
                        }


                        v = System.Math.Max(v, alphaBeta(p, move, level + 1, depth - 1, alpha, beta, maximizingPlayer, !maximizingPlayer, nullMove));
                        alpha = System.Math.Max(alpha, v);
                        if (beta <= alpha)
                        {
                            //Debug.Log("beta break : "+ depth + " : (" + p.getX() + ", " + p.getY() + ")");
                            killerPieces[level] = p;
                            killerMoves[level] = move;


                            i = pieces.Count;
                            break; //beta cut off
                        }
                    }
                }
            }
            piece.setLocation(oldX, oldY, false);
            return v;
        }
        else//want to minimize
        {
            double v = 1000;
            List<Piece> enemyPieces = game.getTeamPieces(currentPlayer, temptBoard);
            for (int i = 0; i < enemyPieces.Count; i++)
            {
                Piece p = enemyPieces[i];
                List<int[]> possibleMoves = game.getPossibleMoves(p, temptBoard);
                possibleMoves = game.willMakeCheck(p, possibleMoves, temptBoard);// checks to see if piece moves to a spot will it chose a check?
                if (possibleMoves.Count > 0)
                {


                    foreach (int[] move in possibleMoves)
                    {
                        //killer heuristic
                        if (killerPieces[level] != null && depth < Alpha_Beta_Depth)//not the first time
                        {
                            if (killerPieces[level] == p)//it is the same piece
                            {
                                if (killerMoves[level][0] == move[0] && killerMoves[level][1] == move[1])//it is moving to the same position
                                {
                                    heuristicValue = calculateHeuristicValue(currentPlayer, temptBoard);
                                    piece.setLocation(oldX, oldY, false);
                                    //Debug.Log("alpha killer move : " + depth + " : (" + p.getX() + ", " + p.getY() + ")");
                                    return heuristicValue;
                                }
                            }
                        }


                        v = System.Math.Min(v, alphaBeta(p, move, level + 1, depth - 1, alpha, beta, maximizingPlayer, maximizingPlayer, nullMove));
                        beta = System.Math.Min(beta, v);
                        if (beta <= alpha)
                        {
                            //Debug.Log("alpha break : " + depth + " : (" + p.getX() + ", " + p.getY() + ")");
                            killerPieces[level] = p;
                            killerMoves[level] = move;

                            i = pieces.Count;
                            break; //alpha cut off
                        }
                    }
                }
            }
            piece.setLocation(oldX, oldY, false);
            return v;
        }
    }

    private double calculateHeuristicValue(bool theTeam, Piece[,] temptBoard)
    {
        //add
        List<Piece> allyListOfPieces = game.getTeamPieces(theTeam, temptBoard);
        double heuristicValue = 0;//the value of the current situation
        foreach (Piece p in allyListOfPieces)
        {
            heuristicValue += p.getValue() * MOD_ADD_PIECE; //for a piece simply being there

            List<int[]> possibleMoves = game.getPossibleMoves(p, temptBoard);
            possibleMoves = game.willMakeCheck(p, possibleMoves, temptBoard);// checks to see if piece moves to a spot will it chose a check?
            if (possibleMoves.Count > 0)
            {
                foreach (int[] move in possibleMoves)
                {
                    if (temptBoard[move[0], move[1]] != null) //enemy on tile aka can take a piece next turn
                    {
                        heuristicValue += (temptBoard[move[0], move[1]].getValue() * MOD_ADD_TAKE_PIECE);
                    }
                    if ((move[0] == 3 || move[0] == 4) && (move[1] == 3 || move[1] == 4)) //will control the center 4 tiles
                    {
                        heuristicValue += MOD_CONTROL_CENTER;
                    }
                }
            }
        }

        //minus
        List<Piece> enemyListOfPieces = game.getTeamPieces(!theTeam, temptBoard);
        foreach (Piece p in enemyListOfPieces)
        {
            heuristicValue -= (p.getValue() * MOD_MINUS_PIECE); //for a piece simply being there

            List<int[]> possibleMoves = game.getPossibleMoves(p, temptBoard);
            possibleMoves = game.willMakeCheck(p, possibleMoves, temptBoard);// checks to see if piece moves to a spot will it chose a check?
            if (possibleMoves.Count > 0)
            {
                foreach (int[] move in possibleMoves)
                {
                    if (temptBoard[move[0], move[1]] != null) //enemy on tile aka can take a piece next turn
                    {
                        heuristicValue -= (temptBoard[move[0], move[1]].getValue() * MOD_MINUS_TAKE_PIECE);
                    }
                }
            }
        }

        return heuristicValue;
    }

    private bool nullMoveHeuristic(int level, double alpha, double beta, bool maximizingPlayer, bool currentPlayer, Piece[,] temptBoard)//figures out if null-move heuristic is nessary here
    {
        List<Piece> enemyPieces = game.getTeamPieces(currentPlayer, temptBoard);
        for (int i = 0; i < enemyPieces.Count; i++)
        {
            Piece p = enemyPieces[i];
            List<int[]> possibleMoves = game.getPossibleMoves(p, temptBoard);
            possibleMoves = game.willMakeCheck(p, possibleMoves, temptBoard);// checks to see if piece moves to a spot will it chose a check?
            if (possibleMoves.Count > 0)
            {
                foreach (int[] move in possibleMoves)
                {
                    double v = alphaBeta(p, move, level + 1, nullMoveDepth, alpha, beta, maximizingPlayer, currentPlayer, false);
                    if (maximizingPlayer == currentPlayer)//try to maximize
                    {
                        if (v < beta)
                        {
                            return true;
                        }
                    }
                    else//try to minimize
                    {
                        if (v > alpha)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

}

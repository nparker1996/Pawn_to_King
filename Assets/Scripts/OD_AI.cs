using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OD_AI : Agent {

    private double MOD_ADDITIONAL_REASON = 1; //how much we should take into account the positives of movement, keep at 1
    private double MOD_ENEMY_MOVEMENT = 1; //how much we should take into account the negatives of movement, keep at 1
    private double MOD_AR_TAKE_PIECE = 2; //how much should taking a piece add //1
    private double MOD_AR_DISTANCE_KING = .05; //how far away a piece is from the enemy king, affects positive //.05
    private double MOD_AR_NOT_MOVED = 0.01; //how much a piece hasnt moved should affect positive //0
    private double MOD_AR_ALLY_GUARD = .05; //how much allies are guarding a spot for positive //.25
    private double MOD_AR_MORE_MOVEMENT = .01; //how much should have more space to move should affect the positive //.01
    private double MOD_AR_CHECK_KING = 1; //how much checking a king should affect positive //1
    private double MOD_AR_CONTROL_CENTER = .05; //how much controlling the center 4 tiles should be //.05

    private double MOD_EM_STAY_STILL = 1; //how much is it for a piece to stay still //1
    private double MOD_EM_ADDITIONAL_REASON = 0; //how much the additional movements of a enemy piece should affect negative //.5 //0
    private double MOD_EM_TAKE_PIECE = 1; //how much the enemy taking a piece should affect the negative //1
    private double MOD_EM_TAKE_MOVED_PIECE = 1; //how much taking the piece that is moving to the tile should affect negative //1    
    private double MOD_EM_MOVE_COUNT = .01;

    // Use this for initialization
    void Start() {
        type = "Optimal Decision";
    }

    // Update is called once per frame
    void Update() {

    }
    public OD_AI(Game theGame, bool team) : base(theGame, team)
    {
        type = "Optimal Decision";
    }

    public override void turn()
    {
        Dictionary<Piece, List<double[]>> optimalChoices = new Dictionary<Piece, List<double[]>>(); //each piece with its moves and utilites (value) 
        double highestValue = -1000; // the highest value that has been returned

        ///////////////////////////////////////////////
        ///Figures outs all possible moves of pieces///
        foreach (Piece p in pieces)
        {
            List<int[]> possibleMoves = game.getPossibleMoves(p, game.board);
            possibleMoves = game.willMakeCheck(p, possibleMoves, game.board);// checks to see if piece moves to a spot will it chose a check?
            if (possibleMoves.Count > 0)
            {
                List<double[]> movesAndValues = new List<double[]>(); //the place the piece can move and the value it has
                foreach (int[] tile in possibleMoves)//foreach tile that the piece can move to, calculate value
                {
                    double allyGuardTotal = 0;//takes one away because the piece that is moving there will also be in allyGuardTiles
                    List<int[]> allyGuardTiles = game.getPossibleMovesForTeam(p.getTeam(), false, game.board);
                    foreach (int[] allyGuard in allyGuardTiles) //are allies guarding the spot that piece is moving to?
                    {
                        if (tile[0] == allyGuard[0] && tile[1] == allyGuard[1])
                        {
                            allyGuardTotal += MOD_AR_ALLY_GUARD;
                        }
                    }

                    //value of moving
                    double addition = (AdditionalReasons(p, tile, true) * MOD_ADDITIONAL_REASON) + allyGuardTotal;

                    if (game.board[tile[0], tile[1]] != null)//enemy piece on tile, aka would take piece then get value of that piece
                    {
                        movesAndValues.Add(new double[] { tile[0], tile[1], (game.board[tile[0], tile[1]].getValue() * MOD_AR_TAKE_PIECE) + addition });
                    }
                    else
                    {
                        movesAndValues.Add(new double[] { tile[0], tile[1], addition });
                    }
                }
                optimalChoices.Add(p, movesAndValues);
            }
        }
        printTurn();


        //////////////////////////////////////////////////////
        ///for each move, figure out what opponent could do///
        foreach (Piece p in pieces)
        {
            if (optimalChoices.ContainsKey(p))//if the piece is within the 
            {
                double stayStill = AdditionalReasons(p, new int[] { p.getX(), p.getY() }, false);//the value of not moving
                foreach (double[] tile in optimalChoices[p])
                {
                    if (game.board[(int)tile[0], (int)tile[1]] != null)
                    {
                        if (game.board[(int)tile[0], (int)tile[1]].getTeam() != p.getTeam())
                        {
                            stayStill += game.board[(int)tile[0], (int)tile[1]].getValue();
                        }
                    }
                }
                stayStill *= MOD_EM_STAY_STILL;

                foreach (double[] tile in optimalChoices[p])
                {
                    //minus average of enemy move value
                    double eCalc = calculateEnemyMovesAverage(p, tile) * MOD_ENEMY_MOVEMENT;
                    //Debug.Log((tile[2] - (eCalc + stayStill)) + " = " + tile[2] + " - (" + eCalc + " + " + stayStill + ") : " + "(" + p.getX() + ", " + p.getY() + ") at" + " (" + tile[0] + ", " + tile[1] + ") " + p.getType());
                    tile[2] -= (eCalc + stayStill);

                    if (tile[2] > highestValue)
                    {
                        highestValue = tile[2];
                    }

                }
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

    double AdditionalReasons(Piece piece, int[] tile, bool notMoved) //adds additional values to makes choses more valuable
    {
        double addition = 0;
        int oldX = piece.getX();
        int oldY = piece.getY();
        Piece[,] temptBoard = new Piece[8, 8]; //makes a copy of board
        System.Array.Copy(game.board, temptBoard, 64);
        List<int[]> canMove = new List<int[]>();

        temptBoard[piece.getX(), piece.getY()] = null;
        temptBoard[tile[0], tile[1]] = piece;
        piece.setLocation(tile[0], tile[1], false);

        if (piece.getType() != 0)
        {//closer to enemy king, the better
            Piece eKing;
            if (piece.getTeam()) { eKing = game.blackTeam.king; }
            else { eKing = game.whiteTeam.king; }

            double distance = ((tile[0] - eKing.getX()) * (tile[0] - eKing.getX())) + ((tile[1] - eKing.getY()) * (tile[1] - eKing.getY()));
            distance = System.Math.Floor(System.Math.Sqrt(distance));
            addition += ((11 - distance) * MOD_AR_DISTANCE_KING);
        }
        else //pawns want to go to the end
        {
            if (piece.getTeam())//white
            {
                addition += ((7 - piece.getY()) * MOD_AR_DISTANCE_KING);
            }
            else//black
            {
                addition += (piece.getY() * MOD_AR_DISTANCE_KING);
            }
        }

        if (!piece.getMoved() && notMoved) //piece has not moved yet
        {
            addition += MOD_AR_NOT_MOVED;
        }

        List<int[]> possibleMoves = game.getPossibleMoves(piece, temptBoard);
        foreach (int[] move in possibleMoves)
        {
            if (piece.getType() != 5)
            {
                addition += MOD_AR_MORE_MOVEMENT; //the more room that that piece will have to move, the better
            }
            if (temptBoard[move[0], move[1]] != null) // checks to see if the piece moves, will it make a check?
            {
                if (temptBoard[move[0], move[1]].getTeam() != piece.getTeam()) //is an enemy
                {
                    //addition += temptBoard[move[0], move[1]].getValue() *  MOD_AR_WOULD_TAKE_PIECE;
                    if (temptBoard[move[0], move[1]].getType() == 5) //is the enemy king
                    {
                        addition += temptBoard[move[0], move[1]].getValue() * MOD_AR_CHECK_KING;
                    }
                }
            }
            if (piece.getType() != 0)
            {
                if ((move[0] == 3 || move[0] == 4) && (move[1] == 3 || move[1] == 4)) //will control the center 4 tiles
                {
                    addition += MOD_AR_CONTROL_CENTER;
                }
            }
        }

        if (piece.getType() == 0)//pawn special control center
        {
            foreach (int[] move in game.pawnTakePiece(piece, true, temptBoard))
            {
                if ((move[0] == 3 || move[0] == 4) && (move[1] == 3 || move[1] == 4)) //will control the center 4 tiles
                {
                    addition += MOD_AR_CONTROL_CENTER;
                }
            }
        }
        piece.setLocation(oldX, oldY, false);
        return addition;
    }

    double calculateEnemyMovesAverage(Piece p, double[] tile)//calculates the average value of all possible moves the enemy could make next turn 
    {
        double total = 0;
        int count = 0;
        int oldX = p.getX();
        int oldY = p.getY();
        Piece[,] temptBoard = new Piece[8, 8]; //makes a copy of board
        System.Array.Copy(game.board, temptBoard, 64);
        temptBoard[p.getX(), p.getY()] = null;
        temptBoard[(int)tile[0], (int)tile[1]] = p; //move piece
        p.setLocation((int)tile[0], (int)tile[1], false);
        List<Piece> enemyTeam = game.getTeamPieces(!p.getTeam(), temptBoard);
        

        foreach (Piece enemy in enemyTeam)
        {
            double eHighest = -1000;
            double eCurrent = 0;
            List<int[]> eMoves = game.getPossibleMoves(enemy, temptBoard);
            eMoves = game.willMakeCheck(enemy, eMoves, temptBoard);// checks to see if piece moves to a spot will it chose a check?
            if (eMoves.Count > 0)
            {
                foreach (int[] eTile in eMoves)
                {
                    //value of move
                    eCurrent = AdditionalReasons(enemy, eTile, true) * MOD_EM_ADDITIONAL_REASON;

                    if (temptBoard[eTile[0], eTile[1]] != null)//enemy piece on tile, aka would take piece then get value of that piece
                    {
                        eCurrent += temptBoard[eTile[0], eTile[1]].getValue() * MOD_EM_TAKE_PIECE;
                        if (tile[0] == eTile[0] && tile[1] == eTile[1])//if they are hitting the piece, then less likely to move there
                        {
                            eCurrent += p.getValue() * MOD_EM_TAKE_MOVED_PIECE;
                        }
                    }

                    if (eCurrent > eHighest)//only takes into account the highest rated move that the piece could move
                    {
                        eHighest = eCurrent;
                    }
                }
                if (eHighest > 0)
                {
                    total += eHighest;
                    count++;
                }
            }
        }
        if (count == 0)
        {
            count = 1;
        }
        //Debug.Log((total / count) + " : " + total + " / " + count +  " : " + " (" + p.getX() + ", " + p.getY() + ", " +p.getType() + ") -->" + " (" + tile[0] + ", " + tile[1] + ") ");
        p.setLocation(oldX, oldY, false);
        return (total / count) + (p.getMoveCount() * MOD_EM_MOVE_COUNT);
    }

}

///Notes
///piece sacrific themselves to check the king

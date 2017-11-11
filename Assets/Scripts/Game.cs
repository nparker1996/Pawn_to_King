using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    private UnityEngine.UI.Button REF_UI_BUTTON;

    [SerializeField]
    private GameObject REF_W_KING;
    [SerializeField]
    private GameObject REF_W_QUEEN;
    [SerializeField]
    private GameObject REF_W_ROOK;
    [SerializeField]
    private GameObject REF_W_BISHOP;
    [SerializeField]
    private GameObject REF_W_KNIGHT;
    [SerializeField]
    private GameObject REF_W_PAWN;
    [SerializeField]
    private GameObject REF_B_KING;
    [SerializeField]
    private GameObject REF_B_QUEEN;
    [SerializeField]
    private GameObject REF_B_ROOK;
    [SerializeField]
    private GameObject REF_B_BISHOP;
    [SerializeField]
    private GameObject REF_B_KNIGHT;
    [SerializeField]
    private GameObject REF_B_PAWN;


    public Agent whiteTeam; //white team
    public Agent blackTeam; //black team
    public Piece[,] board; //width, height
    public Piece selectedPiece; //the piece that is currently selected
    public bool whoseTurn; // true = white, false = black
    public bool check; //king is checked
    public bool checkmate; //king is checkmated
    public bool stalemate; //it is a stalemate
    public int turnCount; //the current turn
    public List<string[]> log; //the record of moves
    public string[] currentLog;
    //Button[] fromPawn = new Button[4];
    private bool PromotePawn = true;

    // Use this for initialization
    void Start()
    {
        //setup all references
        REF_UI_BUTTON = GameObject.Find("Button_Start").GetComponent<UnityEngine.UI.Button>();

        board = new Piece[8, 8];
    }

    // Update is called once per frame
    void Update()
    {
        if (!checkmate)
        {
            if (whoseTurn == whiteTeam.team && whiteTeam.type != "Human") //white AI
            {
                whiteTeam.turn();
            }
            else if (whoseTurn == blackTeam.team && blackTeam.type != "Human") //black AI
            {
                blackTeam.turn();
            }
        }
    }

    private void newGame()//create a new game
    {
        //switch () { }
        //REF_UI_BUTTON.GetComponentInChildren<Text>().text = "Restart";
        //timer.Enabled = true;
        whoseTurn = true;
        check = false;
        checkmate = false;
        stalemate = false;
        turnCount = 0;
        resetBoard();
        makeBoard();
        updateText(true);
    }

    private void clickedPiece()
    {

    }

    private void clickedTile() //when you click a tile that is highlighted
    {

    }

    public void ClickedPieceAI(Piece piece, int xx, int yy) //click piece method for the AI and move it
    {

    }

    private void RemoveSelected() //removes the selectedPiece to clear the board
    {

    }

    public void resetGame() //for neural network and variable modification AI //DONE
    {
        //timer.Enabled = true;
        whoseTurn = true;
        check = false;
        checkmate = false;
        stalemate = false;
        turnCount = 0;
        resetBoard();
        makeBoard();
        updateText(true);
    }

    void resetBoard() //reset board to starting positions //DONE
    {
        //deletes everything on the board
        for (int i = 0; i < 8; i++)//x
        {
            for (int j = 0; j < 8; j++)//y
            {
                if (board[i, j] != null)//piece on it
                {
                    Destroy(board[i, j].gameObject); //destroys the object that represents the piece
                    board[i, j] = null;
                }
            }
        }

        whiteTeam.pieces = new List<Piece>(); //resets white list of pieces
        blackTeam.pieces = new List<Piece>(); //resets black list of pieces

        //Repopulates White Side
        for (int pawnNum = 0; pawnNum <= 7; pawnNum++) //create all 8 white pawns
        {
            addPieceToBoard(pawnNum, 6, true, 0);
        }

        addPieceToBoard(0, 7, true, 3);//(0,7) white rook
        addPieceToBoard(1, 7, true, 1);//(1,7) white knight
        addPieceToBoard(2, 7, true, 2);//(2,7) white bishop
        addPieceToBoard(3, 7, true, 4);//(3,7) white queen
        addPieceToBoard(4, 7, true, 5);//(4,7) white king
        whiteTeam.king = board[4, 7]; //assign king piece
        addPieceToBoard(5, 7, true, 2);//(5,7) white bishop
        addPieceToBoard(6, 7, true, 1);//(6,7) white knight
        addPieceToBoard(7, 7, true, 3);//(7,7) white rook

        //Repopulates Black Side
        for (int pawnNum = 0; pawnNum <= 7; pawnNum++) //create all 8 black pawns
        {
            addPieceToBoard(pawnNum, 1, false, 0);
        }

        addPieceToBoard(0, 0, false, 3);//(0,7) black rook
        addPieceToBoard(1, 0, false, 1);//(1,7) black knight
        addPieceToBoard(2, 0, false, 2);//(2,7) black bishop
        addPieceToBoard(3, 0, false, 4);//(3,7) black queen
        addPieceToBoard(4, 0, false, 5);//(4,7) black king
        blackTeam.king = board[4, 0]; //assign king piece
        addPieceToBoard(5, 0, false, 2);//(5,7) black bishop
        addPieceToBoard(6, 0, false, 1);//(6,7) black knight
        addPieceToBoard(7, 0, false, 3);//(7,7) black rook
    }

    void addPieceToBoard(int x, int y, bool team, int pieceType)//add a piece to location on the board //DONE
    {
        board[x, y] = Instantiate(REF_B_ROOK).GetComponent<Piece>();
        if (team)//white side
        {
            switch (pieceType) //create instatiation of piece
            {
                case 1: //knight
                    board[x, y] = Instantiate(REF_W_KNIGHT).GetComponent<Piece>();
                    break;
                case 2: //bishop
                    board[x, y] = Instantiate(REF_W_BISHOP).GetComponent<Piece>();
                    break;
                case 3: //rook
                    board[x, y] = Instantiate(REF_W_ROOK).GetComponent<Piece>();
                    break;
                case 4: //queen
                    board[x, y] = Instantiate(REF_W_QUEEN).GetComponent<Piece>();
                    break;
                case 5: //king
                    board[x, y] = Instantiate(REF_W_KING).GetComponent<Piece>();
                    break;
                default: //pawn
                    board[x, y] = Instantiate(REF_W_PAWN).GetComponent<Piece>();
                    break;
            }
            whiteTeam.pieces.Add(board[x, y]); //adds piece to team
        }
        else//black team
        {
            switch (pieceType) //create instatiation of piece
            {
                case 1: //knight
                    board[x, y] = Instantiate(REF_B_KNIGHT).GetComponent<Piece>();
                    break;
                case 2: //bishop
                    board[x, y] = Instantiate(REF_B_BISHOP).GetComponent<Piece>();
                    break;
                case 3: //rook
                    board[x, y] = Instantiate(REF_B_ROOK).GetComponent<Piece>();
                    break;
                case 4: //queen
                    board[x, y] = Instantiate(REF_B_QUEEN).GetComponent<Piece>();
                    break;
                case 5: //king
                    board[x, y] = Instantiate(REF_B_KING).GetComponent<Piece>();
                    break;
                default: //pawn
                    board[x, y] = Instantiate(REF_B_PAWN).GetComponent<Piece>();
                    break;
            }
            blackTeam.pieces.Add(board[x, y]); //adds piece to team
        }
        board[x, y].setLocation(x, y); //set location on board of the piece
    }

    void makeBoard() //does final touches to pieces' PictureBox///MIGHT NOT NEED
    {

    }

    void nextTurn()//moves to the next turn //DONE
    {
        //checks for checks, checkmates, and stalemates
        bool checkCheck = false; //checks everything for a check, it's annoy, I know

        List<Piece> pOT = getTeamPieces(whoseTurn);
        foreach (Piece p in pOT)
        {
            if (p == selectedPiece || p.getType() == Piece.TYPE_BISHOP || p.getType() == Piece.TYPE_ROOK || p.getType() == Piece.TYPE_QUEEN)//piece is the one that was moved, or due to the move, another piece could check the king
            {
                if (checkForChecks(!whoseTurn, p)) //check for checks
                {
                    checkCheck = true;
                    break;
                }
            }
        }

        if (checkCheck) //check for checks
        {
            check = true;
            if (checkForCheckmates(!whoseTurn))//check for checkmates
            {
                checkmate = true;
            }
        }
        else//not a check
        {
            check = false;
            if (checkForStalemates(!whoseTurn))//check for stalemates
            {
                stalemate = true;
            }
        }
        selectedPiece = null; //no longer selected
        whoseTurn = !whoseTurn;//switches whos turn it is
        if (whoseTurn)//has moved to the next turn.
        {
            turnCount++;//advance one turn
        }

        //clears the piece of possible moves
        foreach (Piece piece in getTeamPieces(whoseTurn))
        {
            piece.getMoves().Clear();
            if (piece.getPawnDoubleMove())
            {
                piece.setPawnDoubleMove(false);
            }
        }

        updateText(whoseTurn);
    }

    public void updateText(bool whoIsCheckmated)//updates the text on the screen
    {

    }

    public List<int[]> getPossibleMoves(Piece piece)  //get locations piece can move, 
    {
        List<int[]> possibleMoves = new List<int[]>(); //of possible moves piece can make
        switch (piece.getType())
        {
            case Piece.TYPE_PAWN: //pawn
                possibleMoves.Add(pawnStartMove(piece));
                possibleMoves.Add(pawnNormalMove(piece));
                possibleMoves.AddRange(pawnTakePiece(piece, true));
                possibleMoves.Add(pawnEnPassant(piece));
                break;
            case Piece.TYPE_KNIGHT: //knight
                possibleMoves.AddRange(knightNormalMove(piece));
                break;
            case Piece.TYPE_BISHOP: //bishop
                possibleMoves.AddRange(bishopNormalMove(piece));
                break;
            case Piece.TYPE_ROOK: //rook
                possibleMoves.AddRange(rookNormalMove(piece));
                break;
            case Piece.TYPE_QUEEN: //queen
                possibleMoves.AddRange(queenNormalMove(piece));
                break;
            case Piece.TYPE_KING: //king
                possibleMoves.AddRange(kingCastling(piece));
                possibleMoves.AddRange(kingNormalMove(piece));
                break;
            default:
                break;
        }
        for (int i = 0; i < possibleMoves.Count; i++) //gets rid of unnessary tiles
        {
            if (possibleMoves[i] == null) //checks for nulls
            {
                possibleMoves.Remove(possibleMoves[i]);//gets rid of null tiles
                i--;
            }
        }
        return possibleMoves;
    }

    public bool somethingOnTile(int x, int y, bool sameTeamOnly, bool team) //checks to see if a piece is on a tile, if sameTeam is true then also if enemy piece is there //DONE
    {
        if (board[x, y] != null)//something on the spot
        {
            if (sameTeamOnly)//team matters
            {
                if (board[x, y].getTeam() == team) //the other team's piece
                {
                    return false;//can move there
                }
                return true;//same team's piece, can't move there
            }
            return true;//doesn't matter the team, can't move there
        }
        return false;//can move there
    }

    private bool teamOnTile(int x, int y, bool team)//checks to see if a team is on a tile //DONE
    {
        if (board[x, y] != null)
        {
            if (board[x, y].getTeam() == team)
            {
                return true;
            }
        }
        return false;
    }

    void movePieceToTile(Piece piece, int X, int Y) //moves a piece to a tile
    {

    }

    public bool tileWithinBoard(int x, int y) //tile within the board //DONE
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return false;
        }
        return true;
    }

    public List<int[]> willMakeCheck(Piece piece, List<int[]> tiles) // checks to see if the piece moves, will it make a check?
    {
        return new List<int[]>();
    }

    public bool checkForChecks(bool teamBeingChecked, Piece pieceChecking)//checks for a check
    {
        return false;
    }

    public bool checkForCheckmates(bool teamBeingChecked)//checks for a checkmates
    {
        return false;
    }

    public bool checkForStalemates(bool teamToCheck)//checks to see if there is a stalemate
    {
        return false;
    }

    List<int[]> interceptionTiles(Piece piece)//figures out the tiles that can be moved to to stop piece from being taken
    {
        return new List<int[]>();
    }

    List<int[]> containsSameTiles(List<int[]> smallerList, List<int[]> largerList)//returns the tiles that are the same
    {
        return new List<int[]>();
    }

    public List<Piece> getTeamPieces(bool team) //DONE
    {
        if (team)
        {
            return whiteTeam.pieces;
        }
        else
        {
            return blackTeam.pieces;
        }
    }

     ///Piece movements///

        //Pawn//
        int[] pawnStartMove(Piece pawn)//pawn moves two if haven't moved yet
        {
            if (!pawn.getMoved()) //pawn has not moved
            {
                if (pawn.getTeam())//white team
                {
                    if (tileWithinBoard(pawn.getX(), pawn.getY() - 1) && tileWithinBoard(pawn.getX(), pawn.getY() - 2))
                    {
                        if (somethingOnTile(pawn.getX(), pawn.getY() - 1, false, false))//if there is a piece on the tile infront of it
                        {
                            return null;
                        }
                        else if (somethingOnTile(pawn.getX(), pawn.getY() - 2, false, false)) //there is nothing 2 spots infront
                        {
                            return null;
                        }
                        return new int[] { pawn.getX(), pawn.getY() - 2 }; //nothing there
                    }
                    return null;
                }
                else//back team
                {
                    if (tileWithinBoard(pawn.getX(), pawn.getY() + 1) && tileWithinBoard(pawn.getX(), pawn.getY() + 2))
                    {
                        if (somethingOnTile(pawn.getX(), pawn.getY() + 1, false, false))//if there is a piece on the tile infront of it
                        {
                            return null;
                        }
                        else if (somethingOnTile(pawn.getX(), pawn.getY() + 2, false, false)) //there is nothing 2 spots infront
                        {
                            return null;
                        }
                        return new int[] { pawn.getX(), pawn.getY() + 2 }; //nothing there or enemy piece
                    }
                    return null;
                }
            }
            return null;
        }

        int[] pawnEnPassant(Piece pawn) //pawn special move of en passant
        {
            if(pawn.getTeam() && pawn.getY() == 3)//white and on row 3
            {
                if (tileWithinBoard(pawn.getX() - 1, pawn.getY())) //within the board, tile to left
                {
                    if (board[pawn.getX() - 1, pawn.getY()] != null)//there is a piece on the tile
                    {
                        if(board[pawn.getX() - 1, pawn.getY()].getType() == 0 && board[pawn.getX() - 1, pawn.getY()].getTeam() != pawn.getTeam() && board[pawn.getX() - 1, pawn.getY()].getPawnDoubleMove())//piece is a pawn, not on the same team, and just moved two spaces
                        {
                            return new int[] { pawn.getX() - 1, pawn.getY() - 1 };
                        }
                    }
                }
                if(tileWithinBoard(pawn.getX() + 1, pawn.getY()))//within the board, tile to right
                {
                    if (board[pawn.getX() + 1, pawn.getY()] != null)//there is a piece on the tile
                    {
                        if (board[pawn.getX() + 1, pawn.getY()].getType() == 0 && board[pawn.getX() + 1, pawn.getY()].getTeam() != pawn.getTeam() && board[pawn.getX() + 1, pawn.getY()].getPawnDoubleMove())//piece is a pawn, not on the same team, and just moved two spaces
                        {
                            return new int[] { pawn.getX() + 1, pawn.getY() - 1 };
                        }
                    }
                }
            }
            else if(!pawn.getTeam() && pawn.getY() == 4)//black and on 4
            {
                if (tileWithinBoard(pawn.getX() - 1, pawn.getY())) //within the board, tile to left
                {
                    if (board[pawn.getX() - 1, pawn.getY()] != null)//there is a piece on the tile
                    {
                        if (board[pawn.getX() - 1, pawn.getY()].getType() == 0 && board[pawn.getX() - 1, pawn.getY()].getTeam() != pawn.getTeam() && board[pawn.getX() - 1, pawn.getY()].getPawnDoubleMove())//piece is a pawn, not on the same team, and just moved two spaces
                        {
                            return new int[] { pawn.getX() - 1, pawn.getY() + 1 };
                        }
                    }
                }
                if (tileWithinBoard(pawn.getX() + 1, pawn.getY()))//within the board, tile to right
                {
                    if (board[pawn.getX() + 1, pawn.getY()] != null)//there is a piece on the tile
                    {
                        if (board[pawn.getX() + 1, pawn.getY()].getType() == 0 && board[pawn.getX() + 1, pawn.getY()].getTeam() != pawn.getTeam() && board[pawn.getX() + 1, pawn.getY()].getPawnDoubleMove())//piece is a pawn, not on the same team, and just moved two spaces
                        {
                            return new int[] { pawn.getX() + 1, pawn.getY() + 1 };
                        }
                    }
                }
            }

            return null;
        }

        int[] pawnNormalMove(Piece pawn) //pawn moves one space forward
        {
            if (pawn.getTeam())//white team
            {
                if (pawn.getY() - 1 >= 0)//still on board
                {
                    if (somethingOnTile(pawn.getX(), pawn.getY() - 1, false, false)) //if a piece in front
                    {
                        return null;
                    }
                    return new int[] { pawn.getX(), pawn.getY() - 1 }; //nothing there
                }
                return null;//out of bounds
            }
            else//black team
            {
                if (pawn.getY() + 1 < 8)//still on board
                {
                    if (somethingOnTile(pawn.getX(), pawn.getY() + 1, false, false)) //if a piece in front
                    {
                        return null;
                    }
                    return new int[] { pawn.getX(), pawn.getY() + 1 }; //nothing there
                }
                return null;//out of bounds
            }
        }

        public List<int[]> pawnTakePiece(Piece pawn, bool enemyTeam) //pawn move diagonly if there are enemies
        {
            List<int[]> possibleMoves = new List<int[]>();
            if (pawn.getTeam())//white team
            {
                if(tileWithinBoard(pawn.getX() - 1, pawn.getY() - 1))//within bounds, left
                {
                    if (teamOnTile(pawn.getX() - 1, pawn.getY() - 1, !pawn.getTeam()) && enemyTeam) //enemy piece on tile
                    {           
                        possibleMoves.Add(new int[] {pawn.getX() - 1, pawn.getY() - 1});
                    }
                    else if (teamOnTile(pawn.getX() - 1, pawn.getY() - 1, pawn.getTeam()) && !enemyTeam) //ally piece on tile
                    {
                        possibleMoves.Add(new int[] { pawn.getX() - 1, pawn.getY() - 1 });
                    }
                }
                if (tileWithinBoard(pawn.getX() + 1, pawn.getY() - 1))//within bounds, right
                {
                    if (teamOnTile(pawn.getX() + 1, pawn.getY() - 1, !pawn.getTeam()) && enemyTeam) //enemy piece on tile
                    {
                        possibleMoves.Add(new int[] { pawn.getX() + 1, pawn.getY() - 1 });                    
                    }
                    else if (teamOnTile(pawn.getX() + 1, pawn.getY() - 1, pawn.getTeam()) && !enemyTeam) //ally piece on tile
                    {
                        possibleMoves.Add(new int[] { pawn.getX() + 1, pawn.getY() - 1 });
                    }
                }
            }
            else//black team
            {
                if (tileWithinBoard(pawn.getX() - 1, pawn.getY() + 1))//within bounds, left
                {
                    if (teamOnTile(pawn.getX() - 1, pawn.getY() + 1, !pawn.getTeam()) && enemyTeam) //enemy piece on tile
                    {
                        possibleMoves.Add(new int[] { pawn.getX() - 1, pawn.getY() + 1 });                       
                    }
                    else if (teamOnTile(pawn.getX() - 1, pawn.getY() + 1, pawn.getTeam()) && !enemyTeam) //ally piece on tile
                    {
                        possibleMoves.Add(new int[] { pawn.getX() - 1, pawn.getY() + 1 });
                    }
                }
                if (tileWithinBoard(pawn.getX() + 1, pawn.getY() + 1))//within bounds, right
                {
                    if (teamOnTile(pawn.getX() + 1, pawn.getY() + 1, !pawn.getTeam()) && enemyTeam) //enemy piece on tile
                    { 
                        possibleMoves.Add(new int[] { pawn.getX() + 1, pawn.getY() + 1 });                        
                    }
                    else if (teamOnTile(pawn.getX() + 1, pawn.getY() + 1, pawn.getTeam()) && !enemyTeam) //enemy piece on tile
                    {
                        possibleMoves.Add(new int[] { pawn.getX() + 1, pawn.getY() + 1 });
                    }

                }
            }
            return possibleMoves;
        }

        void pawnPromotion(Piece pawn)
        {
            //Promotion
           
        }
        private void toQueen_Click() //pawn to Queen
        {
            selectedPiece.setType(4);
        //Controls.Remove(fromPawn[0]);
        //Controls.Remove(fromPawn[1]);
        //Controls.Remove(fromPawn[2]);
        //Controls.Remove(fromPawn[3]);
        PromotePawn = true;
            nextTurn();
        }
        private void toRook_Click() //pawn to Rook
        {
            selectedPiece.setType(3);
        //Controls.Remove(fromPawn[0]);
        //Controls.Remove(fromPawn[1]);
        //Controls.Remove(fromPawn[2]);
            //Controls.Remove(fromPawn[3]);
            PromotePawn = true;
            nextTurn();
        }
        private void toBishop_Click() //pawn to Bishop
        {
            selectedPiece.setType(2);
            //Controls.Remove(fromPawn[0]);
            //Controls.Remove(fromPawn[1]);
            //Controls.Remove(fromPawn[2]);
            //Controls.Remove(fromPawn[3]);
            PromotePawn = true;
            nextTurn();
        }
        private void toKnight_Click() //pawn to Knight
        {
            selectedPiece.setType(1);
            //Controls.Remove(fromPawn[0]);
            //Controls.Remove(fromPawn[1]);
            //Controls.Remove(fromPawn[2]);
            //Controls.Remove(fromPawn[3]);
            PromotePawn = true;
            nextTurn();
        }

        public int AI_PawnPromotion(Piece pawn, int xPos, int yPos)//looks at what the best move for AI is when promoting a pawn
        {
        return 1;
        }

        //Knight//
        List<int[]> knightNormalMove(Piece knight) //knight normal movement, L shape
        {
            List<int[]> possibleMoves = new List<int[]>();
            int xx = knight.getX();
            int yy = knight.getY();
            if (tileWithinBoard(xx-1,yy-2))//top left
            {
                if (!somethingOnTile(xx - 1, yy - 2, true, !knight.getTeam()))//enemy on spot or empty
                {
                    possibleMoves.Add(new int[] {xx-1,yy-2});
                }
            }
            if (tileWithinBoard(xx + 1, yy - 2))//top right
            {
                if (!somethingOnTile(xx + 1, yy - 2, true, !knight.getTeam()))//enemy on spot or empty
                {
                    possibleMoves.Add(new int[] { xx + 1, yy - 2 });
                }
            }

            if (tileWithinBoard(xx + 2, yy - 1))//right up
            {
                if (!somethingOnTile(xx + 2, yy - 1, true, !knight.getTeam()))//enemy on spot or empty
                {
                    possibleMoves.Add(new int[] { xx + 2, yy - 1 });
                }
            }
            if (tileWithinBoard(xx + 2, yy + 1))//right bottom
            {
                if (!somethingOnTile(xx + 2, yy + 1, true, !knight.getTeam()))//enemy on spot or empty
                {
                    possibleMoves.Add(new int[] { xx + 2, yy + 1 });
                }
            }

            if (tileWithinBoard(xx - 1, yy + 2))//down left
            {
                if (!somethingOnTile(xx - 1, yy + 2, true, !knight.getTeam()))//enemy on spot or empty
                {
                    possibleMoves.Add(new int[] { xx - 1, yy + 2 });
                }
            }
            if (tileWithinBoard(xx + 1, yy + 2))//down right
            {
                if (!somethingOnTile(xx + 1, yy + 2, true, !knight.getTeam()))//enemy on spot or empty
                {
                    possibleMoves.Add(new int[] { xx + 1, yy + 2 });
                }
            }

            if (tileWithinBoard(xx - 2, yy - 1))//left up
            {
                if (!somethingOnTile(xx - 2, yy - 1, true, !knight.getTeam()))//enemy on spot or empty
                {
                    possibleMoves.Add(new int[] { xx - 2, yy - 1 });
                }
            }
            if (tileWithinBoard(xx - 2, yy + 1))//left bottom
            {
                if (!somethingOnTile(xx - 2, yy + 1, true, !knight.getTeam()))//enemy on spot or empty
                {
                    possibleMoves.Add(new int[] { xx - 2, yy + 1 });
                }
            }
            return possibleMoves;
        }

        //Bishop//
        List<int[]> bishopNormalMove(Piece bishop) //bishop normal movement, diagonal
        {
            List<int[]> possibleMoves = new List<int[]>();
            possibleMoves.AddRange(spotsInDirection(bishop, -1, -1));
            possibleMoves.AddRange(spotsInDirection(bishop, -1,  1));
            possibleMoves.AddRange(spotsInDirection(bishop,  1, -1));
            possibleMoves.AddRange(spotsInDirection(bishop,  1,  1));
            return possibleMoves;
        }

        //Rook//
        List<int[]> rookNormalMove(Piece rook) //rook normal movement, left, right, up, down
        {
            List<int[]> possibleMoves = new List<int[]>();
            possibleMoves.AddRange(spotsInDirection(rook, -1, 0));
            possibleMoves.AddRange(spotsInDirection(rook,  1, 0));
            possibleMoves.AddRange(spotsInDirection(rook,  0,-1));
            possibleMoves.AddRange(spotsInDirection(rook,  0, 1));
            return possibleMoves;
        }


        List<int[]> spotsInDirection(Piece piece, int xDir, int yDir)
        {
            List<int[]> possibleMoves = new List<int[]>();
            int xx = piece.getX();
            int yy = piece.getY();
            for(int i = 1; true; i++)
            {
                if (tileWithinBoard(xx + (xDir * i), yy + (yDir * i)))
                {
                    if (!somethingOnTile(xx + (xDir * i), yy + (yDir * i), true, !piece.getTeam())) //tile is empty or has the opposite team on it
                    {
                        possibleMoves.Add(new int[] { xx + (xDir * i), yy + (yDir * i) });
                        if (teamOnTile(xx + (xDir * i), yy + (yDir * i), !piece.getTeam()))// if it is an enemy
                        {
                            break;
                        }
                    }
                    else//has the same team piece on it
                    {
                        break;
                    }
                }
                else //not inbounds
                {
                    break;
                }
            }
            return possibleMoves;
        }
        //Queen//
        List<int[]> queenNormalMove(Piece queen) //queen normal movement, left, right, up, down, diagonal
        {
            List<int[]> possibleMoves = new List<int[]>();
            possibleMoves.AddRange(bishopNormalMove(queen));
            possibleMoves.AddRange(rookNormalMove(queen));
            return possibleMoves;
        }

        //King//
        List<int[]> kingCastling(Piece king)//a king castling
        {
            List<int[]> possibleMoves = new List<int[]>();
            if (!check && !king.getMoved())//if the king is not checked and the king hasn't moved
            {
                if (board[0, king.getY()] != null)//left rook
                {
                    if(board[0, king.getY()].getType() == 3 && !board[0, king.getY()].getMoved())//piece is a rook and hasn't moved
                    {
                        if(board[1, king.getY()] == null && board[2, king.getY()] == null && board[3, king.getY()] == null){ //spots are open
                            possibleMoves.Add(new int[] { 2, king.getY() });
                        }
                    }
                }
                if (board[7, king.getY()] != null)//right rook
                {
                    if (board[7, king.getY()].getType() == 3 && !board[7, king.getY()].getMoved())//piece is a rook and hasn't moved
                    {
                        if (board[5, king.getY()] == null && board[6, king.getY()] == null)
                        { //spots are open
                            possibleMoves.Add(new int[] { 6, king.getY() });
                        }
                    }
                }
            }
            return possibleMoves;
        }

        List<int[]> kingNormalMove(Piece king) //move in any direction one tile
        {
            List<int[]> startMoves = new List<int[]>();
            int kX = king.getX();
            int kY = king.getY();
            int[,] spots = new int[,] { {-1,-1}, {-1,0}, {-1, 1}, {0, -1}, {0, 1} , {1, -1}, {1, 0}, {1, 1} }; //spots around the king
            for (int i = 0; i < spots.GetLength(0); i++)
            {
                int xx = kX + spots[i, 0];//x offset
                int yy = kY + spots[i, 1];//y offset
                if (tileWithinBoard(xx, yy))//within the board
                {
                    if (!somethingOnTile(xx, yy, true, !king.getTeam()))
                    {
                        startMoves.Add(new int[] {xx, yy});
                    }
                }
            }
            List<int[]> enemyMoves = getPossibleMovesForTeam(!king.getTeam(), true);
            for (int i = 0; i < enemyMoves.Count; i++)//get rid of tiles that are too far away to matter
            {
                int[] spot = enemyMoves[i];
                if (spot == null)
                {
                    enemyMoves.RemoveAt(i);
                    i--;
                }
                else if (spot[0] < king.getX() - 1 || spot[0] > king.getX() + 1 || spot[1] < king.getY() - 1 || spot[1] > king.getY() + 1)
                {
                    enemyMoves.RemoveAt(i);
                    i--;
                }
            }
            
            
            foreach (int[] enemySpot in enemyMoves)
            {
                for (int i = 0; i < startMoves.Count;i++)
                {
                    int[] spot = startMoves[i];
                    if (spot[0] == enemySpot[0] && spot[1] == enemySpot[1])//does not contain spot
                    {
                        startMoves.Remove(spot);
                        i--;
                    }
                }
            }
            return startMoves;
        }

        public List<int[]> getPossibleMovesForTeam(bool team, bool withKing) //gets all the possible moves a team  could make, with all square around their king //true = white, false = black
        {
            List<int[]> possibleMoves = new List<int[]>();
            for(int i = 0; i < 8; i++)//x
            {
                for(int j = 0; j < 8; j++)//y
                {
                    if (board[i, j] != null)//piece on it
                    {
                        if(board[i,j].getTeam() == team)//same team that is wanted
                        {
                            if(board[i,j].getType() == Piece.TYPE_KING)//king
                            {
                                if (withKing)//want to get spots for the king to move
                                {
                                    int[,] spots = new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } }; //spots around the king
                                    for (int k = 0; k < spots.GetLength(0); k++)
                                    {
                                        possibleMoves.Add(new int[] { i + spots[k, 0], j + spots[k, 1] });
                                    }
                                }
                            }
                            else if (board[i, j].getType() == Piece.TYPE_PAWN)//pawn
                            {
                                possibleMoves.AddRange(pawnTakePiece(board[i, j], true));
                            }
                            else//another piece
                            {
                                possibleMoves.AddRange(getPossibleMoves(board[i, j]));
                            }
                        }
                    }
                }
            }
            return possibleMoves;
        }

}

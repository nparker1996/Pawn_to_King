using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    private UnityEngine.UI.Button REF_UI_BUTTON_START;
    private UnityEngine.UI.Dropdown REF_UI_DROPDOWN_WHITE;
    private UnityEngine.UI.Dropdown REF_UI_DROPDOWN_BLACK;
    private UnityEngine.UI.Text REF_UI_LABEL_TURN;
    private UnityEngine.UI.Text REF_UI_TEXT_PAWN_PROMOTION;
    private UnityEngine.UI.Slider REF_UI_SLIDER_AI_TURN;

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
    [SerializeField]
    private GameObject REF_ENEMY_OVERLAY;
    [SerializeField]
    private GameObject REF_MOVE_OVERLAY;

    private float AITurnTimer = 0; //count up timer until the ai take turn

    private GameObject playerWhiteTeam; //object agent script is being held in
    private GameObject playerBlackTeam; //object agent script is being held in
    public Agent whiteTeam; //white team
    public Agent blackTeam; //black team
    public Piece[,] board; //width, height
    public Piece[,] prevBoard; //last turn's board
    public Piece selectedPiece; //the piece that is currently selected
    public bool whoseTurn; // true = white, false = black
    public bool check; //king is checked
    public bool checkmate; //king is checkmated
    public bool stalemate; //it is a stalemate
    public int turnCount; //the current turn
    public List<string> log; //the record of moves
    public string[] currentLog;

    private bool pieceTaken = false; //was a piece taken, used for notation
    private bool PromotePawn = true; //wait for player to choose what piece to promote to
    private int promoteTo = Piece.TYPE_PAWN; //what piece the pawn was promoted to, used for notation
    private int castleTo = 0; //what direction occurred in castling, for notation
    private int oldCol, oldRow; //what column or row the piece is in before moving, for notation

    List<GameObject> listOfOverlays;//list of overlays that can be clicked on to move to

    // Use this for initialization
    void Start()
    {
        //setup all references
        REF_UI_BUTTON_START = GameObject.Find("Button_Start").GetComponent<UnityEngine.UI.Button>();
        REF_UI_DROPDOWN_WHITE = GameObject.Find("Dropdown_White").GetComponent<UnityEngine.UI.Dropdown>();
        REF_UI_DROPDOWN_BLACK = GameObject.Find("Dropdown_Black").GetComponent<UnityEngine.UI.Dropdown>();
        REF_UI_LABEL_TURN = GameObject.Find("Text_Label_Turn").GetComponent<UnityEngine.UI.Text>();
        REF_UI_TEXT_PAWN_PROMOTION = GameObject.Find("Text_Pawn").GetComponent<UnityEngine.UI.Text>();
        REF_UI_TEXT_PAWN_PROMOTION.gameObject.SetActive(false);
        REF_UI_SLIDER_AI_TURN = GameObject.Find("Slider_AI").GetComponent<UnityEngine.UI.Slider>();

        listOfOverlays = new List<GameObject>();
        board = new Piece[8, 8];
        prevBoard = new Piece[8, 8];
    }

    // Update is called once per frame
    void Update()
    {
        if (!checkmate)
        {
            if (whiteTeam != null && blackTeam != null)
            {
                if (AITurnTimer >= REF_UI_SLIDER_AI_TURN.value)
                {
                    if (whoseTurn == whiteTeam.team && whiteTeam.type != "Human") //white AI
                    {
                        whiteTeam.turn();
                    }
                    else if (whoseTurn == blackTeam.team && blackTeam.type != "Human") //black AI
                    {
                        blackTeam.turn();
                    }
                    AITurnTimer = 0; //reset timer
                }
                AITurnTimer += Time.deltaTime;
            }
        }
    }

    public void newGame()//create a new game
    {
        Debug.Log("new Game");
        deleteOverlay();
        playerWhiteTeam = GameObject.Find("Player_White");
        playerBlackTeam = GameObject.Find("Player_Black");
        setAgent(playerWhiteTeam, REF_UI_DROPDOWN_WHITE.options[REF_UI_DROPDOWN_WHITE.value].text);
        setAgent(playerBlackTeam, REF_UI_DROPDOWN_BLACK.options[REF_UI_DROPDOWN_BLACK.value].text);
        whiteTeam = playerWhiteTeam.GetComponent<Agent>();
        blackTeam = playerBlackTeam.GetComponent<Agent>();
        whiteTeam.setTeam(true);
        blackTeam.setTeam(false);

        whoseTurn = true;
        check = false;
        checkmate = false;
        stalemate = false;
        turnCount = 0;
        resetBoard();
        updateText(true);
    }

    private void setAgent(GameObject team, string inputType) //sets the type of Agent for each AI 
    {
        if (team.GetComponent<Agent>() != null) //already has a AI script
        {
            DestroyImmediate(team.GetComponent<Agent>()); //removes the previous Agent
        }
        switch (inputType) //adds new Agent type
        {
            case "Optimal Decision":
                team.AddComponent<OD_AI>();
                break;
            case "Maximin":
                team.AddComponent<Maximin_AI>();
                break;
            case "Random":
                team.AddComponent<Random_AI>();
                break;
            case "Unsupervised Neural Network":
                team.AddComponent<NN_Unsupervised_AI>();
                break;
            case "Neural Network":
                team.AddComponent<Neural_Network_AI>();
                break;
            case "Three Stage":
                team.AddComponent<Three_Stage_AI>();
                break;
            case "Variable":
                team.AddComponent<Variable_Modifier_AI>();
                break;
            case "Human":
            default:
                team.AddComponent<Human>();
                break;
        }
    }

    public void clickedPiece(int xx, int yy) //the piece was clicked 
    {
        //Debug.Log(xx + " : " + yy);
        if (!checkmate && PromotePawn)//a checkmate has not occurred && if there is a pawn to promote, wait until it is promoted
        {
            if ((whoseTurn == whiteTeam.team && whiteTeam.type == "Human") || (whoseTurn == blackTeam.team && blackTeam.type == "Human")) //so only works if playing as a human
            {
                deleteOverlay();

                if (board[xx, yy].getTeam() == whoseTurn)//only click piece that is their turn
                {
                    selectedPiece = board[xx, yy];
                    oldCol = xx; oldRow = yy; //for move notation
                    List<int[]> moves = new List<int[]>();
                    if (selectedPiece.getMoves().Count == 0)//has not calculated possible spots
                    {
                        moves = getPossibleMoves(board[xx, yy], board);
                        moves = willMakeCheck(board[xx, yy], moves, board);// checks to see if piece moves to a spot will it chose a check?
                        selectedPiece.getMoves().AddRange(moves);
                    }
                    else//has already calculated possible spots
                    {
                        moves = selectedPiece.getMoves();
                    }
                    foreach (int[] tile in moves)
                    {
                        if (tile != null) //Tile exists
                        {
                            if (teamOnTile(tile[0], tile[1], !selectedPiece.getTeam(), board)) //enemy on tile
                            {
                                GameObject box = Instantiate(REF_ENEMY_OVERLAY);
                                box.GetComponent<Overlay>().setLocation(tile[0], tile[1]);
                                listOfOverlays.Add(box);
                                board[tile[0], tile[1]].setOverlayOn(true);

                            }
                            else //open tile
                            {
                                GameObject box = Instantiate(REF_MOVE_OVERLAY);
                                box.GetComponent<Overlay>().setLocation(tile[0], tile[1]);
                                listOfOverlays.Add(box);
                            }
                                
                        }
                    }
                }
            }
         }
       
    }

    public void clickedTile(int xx, int yy) //when you click a tile that is highlighted 
    {
        deleteOverlay();

        //castling
        if (selectedPiece.getType() == Piece.TYPE_KING && !selectedPiece.getMoved() && !check)//is a king and hasn't moved
        {
            if (xx == 2)//left, queenside
            {
                movePieceToTile(board[0, yy], 3, yy); //move left rook to proper location
                castleTo = 1;
            }
            else if (xx == 6)//right, kingside
            {
                movePieceToTile(board[7, yy], 5, yy); //move right rook to proper location
                castleTo = 2;
            }
        }

        //en passant or pawn double 
        if (selectedPiece.getType() == Piece.TYPE_PAWN)//is a pawn
        {
            //pawn double move
            if (System.Math.Abs(selectedPiece.getY() - yy) >= 2)
            {
                selectedPiece.setPawnDoubleMove(true);
            }

            //pawn en passant
            if (selectedPiece.getX() != xx)//aka moving diagnal
            {
                if (board[xx, yy] == null)
                {
                    if (board[xx, selectedPiece.getY()].getTeam())//white Team
                    {
                        whiteTeam.pieces.Remove(board[xx, selectedPiece.getY()]);
                        blackTeam.points += board[xx, selectedPiece.getY()].getValue();
                    }
                    else//black team
                    {
                        blackTeam.pieces.Remove(board[xx, selectedPiece.getY()]);
                        whiteTeam.points += board[xx, selectedPiece.getY()].getValue();
                    }
                    Destroy(board[xx, selectedPiece.getY()].gameObject); //destory piece being captured
                    board[xx, selectedPiece.getY()] = null;
                    pieceTaken = true;
                }
            }
        }

        movePieceToTile(selectedPiece, xx, yy);//moves the piece

        pawnPromotion();

        if (PromotePawn)//if there is a pawn to promote, wait until it is promoted
        {
            nextTurn();
        }
    }

    public void clickedPieceAI(Piece piece, int xx, int yy) //click piece method for the AI and move it 
    {
        if (!checkmate)//a checkmate has not occurred
        {
            deleteOverlay();

            if (piece.getTeam() == whoseTurn)//only click piece that is their turn
            {
                selectedPiece = piece;
                oldCol = piece.getX(); oldRow = piece.getY(); //for move notation
            }

            ///move piece///
            //castling
            if (selectedPiece.getType() == Piece.TYPE_KING && !selectedPiece.getMoved())//is a king and hasn't moved
            {
                if (xx == 2)//left, queenside
                {
                    movePieceToTile(board[0, yy], 3, yy);
                    castleTo = 1;
                }
                else if (xx == 6)//right, kingside
                {
                    movePieceToTile(board[7, yy], 5, yy);
                    castleTo = 2;
                }
            }

            //pawn promoting
            if (selectedPiece.getType() == Piece.TYPE_PAWN && ((yy == 7 && selectedPiece.getTeam()) || (yy == 0 && !selectedPiece.getTeam())))
            {
                int p = AI_PawnPromotion(selectedPiece, xx, yy, board);
                changePieceType(selectedPiece, p);
            }

            movePieceToTile(selectedPiece, xx, yy);//moves the piece
            nextTurn();
        }
    }

    private void deleteOverlay()
    {
        foreach (GameObject overlay in listOfOverlays) //destroys each overlay current
        {
            int[] loc = overlay.GetComponent<Overlay>().getLocation();
            if(board[loc[0], loc[1]] != null) { board[loc[0], loc[1]].setOverlayOn(false); } //makes piece not able to be clickedTile
            Destroy(overlay);
        }

        listOfOverlays.Clear();//clears list
    }

    public void removeSelected() //removes the selectedPiece to clear the board 
    {
        Debug.Log("Remove");
        selectedPiece = null;
        oldCol = -1; oldRow = -1; //for move notation
        deleteOverlay();
    }

    public void resetGame() //for neural network and variable modification AI 
    {
        whoseTurn = true;
        check = false;
        checkmate = false;
        stalemate = false;
        turnCount = 0;
        log.Clear();
        resetBoard();
        updateText(true);
    }

    void resetBoard() //reset board to starting positions 
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
            addPieceToBoard(pawnNum, 1, true, Piece.TYPE_PAWN);
        }

        addPieceToBoard(0, 0, true, Piece.TYPE_ROOK);//(0,7) white rook
        addPieceToBoard(1, 0, true, Piece.TYPE_KNIGHT);//(1,7) white knight
        addPieceToBoard(2, 0, true, Piece.TYPE_BISHOP);//(2,7) white bishop
        addPieceToBoard(4, 0, true, Piece.TYPE_KING);//(3,7) white king
        addPieceToBoard(3, 0, true, Piece.TYPE_QUEEN);//(4,7) white queen
        whiteTeam.king = board[4, 0]; //assign king piece
        addPieceToBoard(5, 0, true, Piece.TYPE_BISHOP);//(5,7) white bishop
        addPieceToBoard(6, 0, true, Piece.TYPE_KNIGHT);//(6,7) white knight
        addPieceToBoard(7, 0, true, Piece.TYPE_ROOK);//(7,7) white rook

        //Repopulates Black Side
        for (int pawnNum = 0; pawnNum <= 7; pawnNum++) //create all 8 black pawns
        {
            addPieceToBoard(pawnNum, 6, false, Piece.TYPE_PAWN);
        }

        addPieceToBoard(0, 7, false, Piece.TYPE_ROOK);//(0,7) black rook
        addPieceToBoard(1, 7, false, Piece.TYPE_KNIGHT);//(1,7) black knight
        addPieceToBoard(2, 7, false, Piece.TYPE_BISHOP);//(2,7) black bishop
        addPieceToBoard(4, 7, false, Piece.TYPE_KING);//(3,7) black king
        addPieceToBoard(3, 7, false, Piece.TYPE_QUEEN);//(4,7) black queen
        blackTeam.king = board[4, 7]; //assign king piece
        addPieceToBoard(5, 7, false, Piece.TYPE_BISHOP);//(5,7) black bishop
        addPieceToBoard(6, 7, false, Piece.TYPE_KNIGHT);//(6,7) black knight
        addPieceToBoard(7, 7, false, Piece.TYPE_ROOK);//(7,7) black rook

        System.Array.Copy(board, prevBoard, 64);
    }

    Piece addPieceToBoard(int x, int y, bool team, int pieceType)//add a piece to location on the board 
    {
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
        board[x, y].setLocation(x, y, true); //set location on board of the piece
        return board[x, y];
    }

    void nextTurn()//moves to the next turn 
    {
        //checks for checks, checkmates, and stalemates
        bool checkCheck = false; //checks everything for a check, it's annoying, I know

        List<Piece> pOT = getTeamPieces(whoseTurn, board);
        foreach (Piece p in pOT)
        {
            if (p == selectedPiece || p.getType() == Piece.TYPE_BISHOP || p.getType() == Piece.TYPE_ROOK || p.getType() == Piece.TYPE_QUEEN)//piece is the one that was moved, or due to the move, another piece could check the king
            {
                if (checkForChecks(!whoseTurn, p, board)) //check for checks
                {
                    checkCheck = true;
                    break;
                }
            }
        }

        if (checkCheck) //check for checks
        {
            check = true;
            if (checkForCheckmates(!whoseTurn, board))//check for checkmates
            {
                checkmate = true;
            }
        }
        else//not a check
        {
            check = false;
            if (checkForStalemates(!whoseTurn, board))//check for stalemates
            {
                stalemate = true;
            }
        }

        log.Add(getNotation(selectedPiece, oldCol, oldRow, selectedPiece.getX(), selectedPiece.getY(), board, prevBoard, castleTo, promoteTo, pieceTaken, check, checkmate)); //added notation to log
        Debug.Log(log[log.Count-1]);

        pieceTaken = false;
        promoteTo = Piece.TYPE_PAWN;
        castleTo = 0;
        selectedPiece = null; //no longer selected
        oldCol = -1; oldRow = -1; //for move notation
        whoseTurn = !whoseTurn;//switches whos turn it is
        if (whoseTurn)//has moved to the next turn.
        {
            turnCount++;//advance one turn
        }

        //clears the piece of possible moves
        foreach (Piece piece in getTeamPieces(whoseTurn, board))
        {
            piece.getMoves().Clear();
            if (piece.getPawnDoubleMove())
            {
                piece.setPawnDoubleMove(false);
            }
        }

        System.Array.Copy(board, prevBoard, 64);//updated for next turn

        updateText(whoseTurn);
    }

    public void updateText(bool whoIsCheckmated)//updates the text on the screen 
    {
        string labelText = "";
        if (stalemate)
        {
            labelText = "Stalemate, it is a tie...";
            Debug.Log("Stalemate, it is a tie...");
        }
        else if (checkmate)//there is a checkmate
        {
            if (whoIsCheckmated)//black team wins
            {
                labelText = "BLACK WINS!!";
                Debug.Log("BLACK WINS!!");
            }
            else//white team wins
            {
                labelText = "WHITE WINS!!";
                Debug.Log("WHITE WINS!!");
            }
        }
        else {
            labelText = "Turn: " + turnCount + "      ";
            if (whoseTurn)//white team
            {
                labelText += "White's move";
            }
            else//blac team
            {
                labelText += "Black's move";
            }
            if (check)
            {
                labelText += "     Check";
            }
        }
        REF_UI_LABEL_TURN.text = labelText;
    }

    public string getNotation(Piece piece, int oldX, int oldY, int newX, int newY, Piece[,] theBoard, Piece[,] thePrevBoard, int castle, int promote, bool takePiece, bool c, bool cMate) //return the annotation for the move made
    {
        string anno = "";

        //castling
        if(castle == 1) { return "O-O-O"; } //queenside castle
        else if(castle == 2) { return "O-O"; } //kingside castle

        //gets big letter for each piece
        if (promote == Piece.TYPE_PAWN)//a pawn was not promoted
        {
            switch (piece.getType())
            {
                case Piece.TYPE_KING:
                    anno += "K";
                    break;
                case Piece.TYPE_QUEEN:
                    anno += "Q";
                    break;
                case Piece.TYPE_ROOK:
                    anno += "R";
                    break;
                case Piece.TYPE_BISHOP:
                    anno += "B";
                    break;
                case Piece.TYPE_KNIGHT:
                    anno += "N";
                    break;
                default: //pawn or broken piece
                    break;
            }
        }

        //extra row/column info needed
        if((piece.getType() == Piece.TYPE_PAWN || promote != Piece.TYPE_PAWN) && takePiece)//pawns show their column if they are captured
        {
            anno += char.ConvertFromUtf32(oldX + 97); //ascii for a is 97 //x, column
        }
        else if(piece.getType() != Piece.TYPE_PAWN) //other pieces
        {
            bool colAdded = false;// keep track if the column was added, only needed if 3+ of the same on the same side exist
            bool rowAdded = false;// keep track if the row was added, only needed if 3+ of the same on the same side exist
            List<Piece> prevPieces = getListOfSamePiece(thePrevBoard[oldX, oldY], thePrevBoard);
            foreach (Piece p in prevPieces)
            {
                foreach(int[] t in getPossibleMoves(p, thePrevBoard)){
                    if(t[0] == newX && t[1] == newY) //if another piece of the same type and team can attack the same tile
                    {
                        if(p.getX() != oldX && !colAdded) //there is a column difference
                        {
                            anno += char.ConvertFromUtf32(oldX + 97); //ascii for a is 97 //x, column
                            colAdded = true;
                        }
                        else if(p.getY() != oldY && !rowAdded) //there is a row difference
                        {
                            anno += (oldY + 1).ToString(); //y, row
                            rowAdded = true;
                        }
                    }
                }
            }
        }


        //take piece
        if(takePiece) { anno += "x"; }

        //location moved to
        anno += char.ConvertFromUtf32(newX + 97); //ascii for a is 97 //x, column
        anno += (newY + 1).ToString(); //y, row

        //queen promotion
        switch (promote)
        {
            case Piece.TYPE_QUEEN:
                anno += "=Q";
                break;
            case Piece.TYPE_ROOK:
                anno += "=R";
                break;
            case Piece.TYPE_BISHOP:
                anno += "=B";
                break;
            case Piece.TYPE_KNIGHT:
                anno += "=N";
                break;
            default: //pawn or broken piece
                break;
        }

        //check or checkmate
        if (cMate) { anno += "#"; }
        else if(c) { anno += "+"; }

        return anno;
    }

    public List<int[]> getPossibleMoves(Piece piece, Piece[,] theBoard)  //get locations piece can move, 
    {
        List<int[]> possibleMoves = new List<int[]>(); //of possible moves piece can make
        switch (piece.getType())
        {
            case Piece.TYPE_PAWN: //pawn
                possibleMoves.Add(pawnStartMove(piece, theBoard));
                possibleMoves.Add(pawnNormalMove(piece, theBoard));
                possibleMoves.AddRange(pawnTakePiece(piece, true, theBoard));
                possibleMoves.Add(pawnEnPassant(piece, theBoard));
                break;
            case Piece.TYPE_KNIGHT: //knight
                possibleMoves.AddRange(knightNormalMove(piece, theBoard));
                break;
            case Piece.TYPE_BISHOP: //bishop
                possibleMoves.AddRange(bishopNormalMove(piece, theBoard));
                break;
            case Piece.TYPE_ROOK: //rook
                possibleMoves.AddRange(rookNormalMove(piece, theBoard));
                break;
            case Piece.TYPE_QUEEN: //queen
                possibleMoves.AddRange(queenNormalMove(piece, theBoard));
                break;
            case Piece.TYPE_KING: //king
                possibleMoves.AddRange(kingCastling(piece, theBoard));
                possibleMoves.AddRange(kingNormalMove(piece, theBoard));
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

    public bool somethingOnTile(int x, int y, bool sameTeamOnly, bool team, Piece[,] theBoard) //checks to see if a piece is on a tile, if sameTeam is true then also if enemy piece is there 
    {
        if (theBoard[x, y] != null)//something on the spot
        {
            if (sameTeamOnly)//team matters
            {
                if (theBoard[x, y].getTeam() == team) //the other team's piece
                {
                    return false;//can move there
                }
                return true;//same team's piece, can't move there
            }
            return true;//doesn't matter the team, can't move there
        }
        return false;//can move there
    }

    private bool teamOnTile(int x, int y, bool team, Piece[,] theBoard)//checks to see if a team is on a tile 
    {
        if (theBoard[x, y] != null)
        {
            if (theBoard[x, y].getTeam() == team)
            {
                return true;
            }
        }
        return false;
    }

    void movePieceToTile(Piece piece, int X, int Y) //moves a piece to a tile 
    {
        if (!piece.getMoved()) //piece has not moved
        {
            piece.setMoved();
        }
        if (teamOnTile(X, Y, !piece.getTeam(), board))//moving to a tile with an enemy on it
        {
            if (board[X, Y].getTeam())//white Team
            {
                blackTeam.points += board[X, Y].getValue();
            }
            else//black team
            {
                whiteTeam.points += board[X, Y].getValue();
            }
            deletePiece(board[X, Y]); //piece is deleted
            pieceTaken = true;
        }
        board[piece.getX(), piece.getY()] = null; //removes piece from previous location
        board[X, Y] = piece; //puts piece on new tile
        piece.setLocation(X, Y, true);
        piece.addMoveCount(1);
    }

    public bool tileWithinBoard(int x, int y) //tile within the board 
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return false;
        }
        return true;
    }

    public List<int[]> willMakeCheck(Piece piece, List<int[]> tiles, Piece[,] theBoard) // checks to see if the piece moves, will it make a check?
    {
        //tiles are the spots that piece will move to
        if (tiles.Count == 0)
        {
            return tiles;
        }
        Piece[,] temptBoard = new Piece[8, 8];
        System.Array.Copy(theBoard, temptBoard, 64); //copies board to tempBoard
        List<int[]> canMove = new List<int[]>(); //spots that can be moved to that will not cause a check
        canMove.AddRange(tiles);
        foreach (int[] tile in tiles)
        {
            //sets location in temptBoard, but does not change anything in the actual board
            int oldX = piece.getX();
            int oldY = piece.getY();
            temptBoard[piece.getX(), piece.getY()] = null;
            temptBoard[tile[0], tile[1]] = piece;
            piece.setLocation(tile[0], tile[1], false);
            for (int xDir = 0; xDir < 8; xDir++)
            {
                for (int yDir = 0; yDir < 8; yDir++)
                {
                    if (temptBoard[xDir, yDir] != null)
                    {
                        if (temptBoard[xDir, yDir].getTeam() != piece.getTeam())//other team
                        {
                            if (piece.getType() == Piece.TYPE_KING)//is the king
                            {
                                List<int[]> enemyMoves = getPossibleMoves(temptBoard[xDir, yDir], temptBoard);
                                if (temptBoard[xDir, yDir].getType() == Piece.TYPE_PAWN) //pawn attack is different than movement
                                {
                                    enemyMoves.Clear();
                                    enemyMoves.AddRange(pawnTakePiece(temptBoard[xDir, yDir], true, temptBoard));
                                }
                                foreach (int[] spot in enemyMoves)//if any enemy can move to spot
                                {
                                    if (spot != null)
                                    {
                                        if (spot[0] == tile[0] && spot[1] == tile[1])//does contain spot
                                        {
                                            canMove.Remove(tile);
                                            xDir = yDir = 8;
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (checkForChecks(piece.getTeam(), temptBoard[xDir, yDir], temptBoard))
                            {
                                canMove.Remove(tile);
                                xDir = yDir = 8;
                                break;
                            }   
                        }
                    }
                }
            }
            piece.setLocation(oldX, oldY, false);
            System.Array.Copy(theBoard, temptBoard, 64); //copies board to tempBoard
        }

        return canMove;
    }

    public bool checkForChecks(bool teamBeingChecked, Piece pieceChecking, Piece[,] theBoard)//checks for a check 
    {
        if(pieceChecking == null) { return false; }
        Piece king;
        if (teamBeingChecked)//white team
        {
            king = whiteTeam.king;
        }
        else//black team
        {
            king = blackTeam.king;
        }
        List<int[]> enemyMoves = getPossibleMoves(pieceChecking, theBoard);
        foreach (int[] spot in enemyMoves) //check spot of it is the spot of king
        {
            if (spot != null)
            {
                if (spot[0] == king.getX() && spot[1] == king.getY())//does contain spot
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool checkForCheckmates(bool teamBeingChecked, Piece[,] theBoard)//checks for a checkmates , LOOKOVER
    {
        Agent group;
        if (teamBeingChecked)//white team
        {
            group = whiteTeam;
        }
        else//black team
        {
            group = blackTeam;
        }
        Piece king = group.king;

        int[,] aroundSpots = new int[,] { {-1,-1 }, { 0, -1 }, { 1, -1 },
                                          {-1, 0 },            { 1,  0 },
                                          {-1, 1 }, { 0,  1 }, { 1 , 1 }};
        List<int[]> kingMoves = new List<int[]>();
        //int spotChecks = 0;
        for (int i = 0; i < 8; i++)//check the spots around the king
        {
            if (tileWithinBoard(king.getX() + aroundSpots[i, 0], king.getY() + aroundSpots[i, 1]))//tile inbounds
            {
                if (!somethingOnTile(king.getX() + aroundSpots[i, 0], king.getY() + aroundSpots[i, 1], true, false, theBoard))//if there is nothing on a tile
                {
                    kingMoves.Add(new int[] { king.getX() + aroundSpots[i, 0], king.getY() + aroundSpots[i, 1] });
                }
            }
        }
        if (willMakeCheck(king, kingMoves, theBoard).Count > 0)//if there is a spot that the king can move
        {
            return false;
        }
        foreach (Piece p in group.pieces)
        {
            List<int[]> pMoves = containsSameTiles(interceptionTiles(king, theBoard), getPossibleMoves(p, theBoard));
            pMoves = willMakeCheck(p, pMoves, theBoard);
            if (pMoves.Count > 0) //figures out if there are any spots that piece other than the king can move to to stop a checkmate
            {
                return false;
            }
        }
        return true;
    }

    public bool checkForStalemates(bool teamToCheck, Piece[,] theBoard)//checks to see if there is a stalemate 
    {
        if (!check)//if there is a check
        {
            List<Piece> listOfPieces;
            if (teamToCheck)//white team
            {
                listOfPieces = whiteTeam.pieces;
            }
            else//black team
            {
                listOfPieces = blackTeam.pieces;
            }
            foreach (Piece p in listOfPieces)
            {
                if (getPossibleMoves(p, theBoard).Count > 0)//if any piece can move
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    List<int[]> interceptionTiles(Piece piece, Piece[,] theBoard)//figures out the tiles that can be moved to to stop piece from being taken
    {
        List<int[]> possibleTiles = new List<int[]>(); //of possible moves piece can make
                                                       //for queen, bishops, and rooks
        for (int i = -1; i <= 1; i++)//x direction
        {
            for (int j = -1; j <= 1; j++)//y direction
            {
                if (i != 0 || j != 0)//to makes sure that both are not 0
                {
                    possibleTiles.AddRange(spotsInDirection(piece, i, j, theBoard));
                    foreach (int[] tile in possibleTiles)
                    {
                        if (theBoard[tile[0], tile[1]] != null)
                        {
                            if (theBoard[tile[0], tile[1]].getTeam() != piece.getTeam())
                            {
                                if (i == 0 || j == 0)//rook or queen
                                {
                                    if (theBoard[tile[0], tile[1]].getType() == 3 || theBoard[tile[0], tile[1]].getType() == 4)
                                    {
                                        return possibleTiles;
                                    }
                                }
                                else//bishop or queen
                                {
                                    if (theBoard[tile[0], tile[1]].getType() == 2 || theBoard[tile[0], tile[1]].getType() == 4)
                                    {
                                        return possibleTiles;
                                    }
                                }
                            }
                        }
                    }
                    possibleTiles.Clear();
                }
            }
        }

        //knights
        possibleTiles.Clear();
        possibleTiles.AddRange(knightNormalMove(piece, theBoard));
        foreach (int[] tile in possibleTiles)
        {
            if (theBoard[tile[0], tile[1]] != null)
            {
                if (theBoard[tile[0], tile[1]].getTeam() != piece.getTeam())
                {
                    if (theBoard[tile[0], tile[1]].getType() == 1)//knight
                    {
                        List<int[]> knight = new List<int[]>();
                        knight.Add(tile);
                        return knight;
                    }
                }
            }
        }
        possibleTiles.Clear();
        possibleTiles.AddRange(pawnTakePiece(piece, true, theBoard)); //Pawn
        return possibleTiles;
    }

    List<int[]> containsSameTiles(List<int[]> smallerList, List<int[]> largerList)//returns the tiles that are the same 
    {
        List<int[]> tiles = new List<int[]>();
        if (smallerList.Count <= 0 || largerList.Count <= 0) { return tiles; }//nothing is in one of the Lists
        if (largerList.Count < smallerList.Count) //switches list if needed
        {
            List<int[]> temp = smallerList;
            smallerList = largerList;
            largerList = temp;
        }
        for (int i = 0; i < largerList.Count; i++)//gets rid of null tiles
        {
            if (largerList[i] == null)
            {
                largerList.Remove(largerList[i]);
                i--;
            }
        }
        for (int i = 0; i < smallerList.Count; i++)
        {
            if (smallerList[i] == null)
            {
                smallerList.Remove(smallerList[i]);
                i--;
            }
        }

        foreach(int[] smallTile in smallerList)
        {
            foreach (int[] largeTile in largerList)
            {
                if (smallTile[0] == largeTile[0] && smallTile[1] == largeTile[1])//the same tile
                {
                    tiles.Add(smallTile);
                }
            }
        }

        return tiles;
    }

    public List<Piece> getTeamPieces(bool team, Piece[,] theBoard) 
    {
        List<Piece> theList = new List<Piece>();
        for(int i = 0; i <= 7; i++) //x
        {
            for(int j = 0; j <= 7; j++) //y
            {
                if (theBoard[i, j] != null)
                {
                    if(theBoard[i,j].getTeam() == team)
                    {
                        theList.Add(theBoard[i, j]);
                    }
                }
            }
        }
        return theList;
    }

    public List<Piece> getListOfSamePiece(Piece p, Piece[,] theBoard) //gets list of the same piece on the same team
    {
        List<Piece> list = new List<Piece>();
        foreach (Piece ally in getTeamPieces(p.getTeam(), theBoard))
        {
            if(p.getType() == ally.getType() && !p.Equals(ally)) { list.Add(ally); }
        }
        return list;
    }

    public void deletePiece(Piece piece) //removes a piece and gameObject from game 
    {
        if (piece.getTeam())//white side
        {
            whiteTeam.pieces.Remove(piece);
        }
        else//black side
        {
            blackTeam.pieces.Remove(piece);
        }
        Destroy(board[piece.getX(), piece.getY()].gameObject); //destroys the object that represents the piece
        board[piece.getX(), piece.getY()] = null;
    }

    public void changePieceType(Piece piece, int toType) //changes one piece to a different type 
    {
        int[] oldLocation = piece.getLocation();
        bool oldTeam = piece.getTeam();
        int oldMoveCount = piece.getMoveCount();
        deletePiece(piece);
        Piece newPiece = addPieceToBoard(oldLocation[0], oldLocation[1], oldTeam, toType);
        newPiece.setMoved(); //sets moved to true
        newPiece.setMoveCount(oldMoveCount);
        promoteTo = toType;//set for notation
    }

     ///Piece movements///

    //Pawn//
    int[] pawnStartMove(Piece pawn, Piece[,] theBoard)//pawn moves two if haven't moved yet
    {
        if (!pawn.getMoved()) //pawn has not moved
        {
            if (!pawn.getTeam())//black team
            {
                if (tileWithinBoard(pawn.getX(), pawn.getY() - 1) && tileWithinBoard(pawn.getX(), pawn.getY() - 2))
                {
                    if (somethingOnTile(pawn.getX(), pawn.getY() - 1, false, false, theBoard))//if there is a piece on the tile infront of it
                    {
                        return null;
                    }
                    else if (somethingOnTile(pawn.getX(), pawn.getY() - 2, false, false, theBoard)) //there is nothing 2 spots infront
                    {
                        return null;
                    }
                    return new int[] { pawn.getX(), pawn.getY() - 2 }; //nothing there
                }
                return null;
            }
            else//white team
            {
                if (tileWithinBoard(pawn.getX(), pawn.getY() + 1) && tileWithinBoard(pawn.getX(), pawn.getY() + 2))
                {
                    if (somethingOnTile(pawn.getX(), pawn.getY() + 1, false, false, theBoard))//if there is a piece on the tile infront of it
                    {
                        return null;
                    }
                    else if (somethingOnTile(pawn.getX(), pawn.getY() + 2, false, false, theBoard)) //there is nothing 2 spots infront
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

    int[] pawnEnPassant(Piece pawn, Piece[,] theBoard) //pawn special move of en passant
    {
        if(!pawn.getTeam() && pawn.getY() == 3)//black and on row 3
        {
            if (tileWithinBoard(pawn.getX() - 1, pawn.getY())) //within the board, tile to left
            {
                if (theBoard[pawn.getX() - 1, pawn.getY()] != null)//there is a piece on the tile
                {
                    if(theBoard[pawn.getX() - 1, pawn.getY()].getType() == 0 && theBoard[pawn.getX() - 1, pawn.getY()].getTeam() != pawn.getTeam() && theBoard[pawn.getX() - 1, pawn.getY()].getPawnDoubleMove())//piece is a pawn, not on the same team, and just moved two spaces
                    {
                        return new int[] { pawn.getX() - 1, pawn.getY() - 1 };
                    }
                }
            }
            if(tileWithinBoard(pawn.getX() + 1, pawn.getY()))//within the board, tile to right
            {
                if (theBoard[pawn.getX() + 1, pawn.getY()] != null)//there is a piece on the tile
                {
                    if (theBoard[pawn.getX() + 1, pawn.getY()].getType() == 0 && theBoard[pawn.getX() + 1, pawn.getY()].getTeam() != pawn.getTeam() && theBoard[pawn.getX() + 1, pawn.getY()].getPawnDoubleMove())//piece is a pawn, not on the same team, and just moved two spaces
                    {
                        return new int[] { pawn.getX() + 1, pawn.getY() - 1 };
                    }
                }
            }
        }
        else if(pawn.getTeam() && pawn.getY() == 4)//white and on 4
        {
            if (tileWithinBoard(pawn.getX() - 1, pawn.getY())) //within the board, tile to left
            {
                if (theBoard[pawn.getX() - 1, pawn.getY()] != null)//there is a piece on the tile
                {
                    if (theBoard[pawn.getX() - 1, pawn.getY()].getType() == 0 && theBoard[pawn.getX() - 1, pawn.getY()].getTeam() != pawn.getTeam() && theBoard[pawn.getX() - 1, pawn.getY()].getPawnDoubleMove())//piece is a pawn, not on the same team, and just moved two spaces
                    {
                        return new int[] { pawn.getX() - 1, pawn.getY() + 1 };
                    }
                }
            }
            if (tileWithinBoard(pawn.getX() + 1, pawn.getY()))//within the board, tile to right
            {
                if (theBoard[pawn.getX() + 1, pawn.getY()] != null)//there is a piece on the tile
                {
                    if (theBoard[pawn.getX() + 1, pawn.getY()].getType() == 0 && theBoard[pawn.getX() + 1, pawn.getY()].getTeam() != pawn.getTeam() && theBoard[pawn.getX() + 1, pawn.getY()].getPawnDoubleMove())//piece is a pawn, not on the same team, and just moved two spaces
                    {
                        return new int[] { pawn.getX() + 1, pawn.getY() + 1 };
                    }
                }
            }
        }

        return null;
    }

    int[] pawnNormalMove(Piece pawn, Piece[,] theBoard) //pawn moves one space forward
    {
        if (!pawn.getTeam())//black team
        {
            if (pawn.getY() - 1 >= 0)//still on board
            {
                if (somethingOnTile(pawn.getX(), pawn.getY() - 1, false, false, theBoard)) //if a piece in front
                {
                    return null;
                }
                return new int[] { pawn.getX(), pawn.getY() - 1 }; //nothing there
            }
            return null;//out of bounds
        }
        else//white team
        {
            if (pawn.getY() + 1 < 8)//still on board
            {
                if (somethingOnTile(pawn.getX(), pawn.getY() + 1, false, false, theBoard)) //if a piece in front
                {
                    return null;
                }
                return new int[] { pawn.getX(), pawn.getY() + 1 }; //nothing there
            }
            return null;//out of bounds
        }
    }

    public List<int[]> pawnTakePiece(Piece pawn, bool enemyTeam, Piece[,] theBoard) //pawn move diagonly if there are enemies
    {
        List<int[]> possibleMoves = new List<int[]>();
        if (!pawn.getTeam())//black team
        {
            if(tileWithinBoard(pawn.getX() - 1, pawn.getY() - 1))//within bounds, left
            {
                if (teamOnTile(pawn.getX() - 1, pawn.getY() - 1, !pawn.getTeam(), theBoard) && enemyTeam) //enemy piece on tile
                {           
                    possibleMoves.Add(new int[] {pawn.getX() - 1, pawn.getY() - 1});
                }
                else if (teamOnTile(pawn.getX() - 1, pawn.getY() - 1, pawn.getTeam(), theBoard) && !enemyTeam) //ally piece on tile
                {
                    possibleMoves.Add(new int[] { pawn.getX() - 1, pawn.getY() - 1 });
                }
            }
            if (tileWithinBoard(pawn.getX() + 1, pawn.getY() - 1))//within bounds, right
            {
                if (teamOnTile(pawn.getX() + 1, pawn.getY() - 1, !pawn.getTeam(), theBoard) && enemyTeam) //enemy piece on tile
                {
                    possibleMoves.Add(new int[] { pawn.getX() + 1, pawn.getY() - 1 });                    
                }
                else if (teamOnTile(pawn.getX() + 1, pawn.getY() - 1, pawn.getTeam(), theBoard) && !enemyTeam) //ally piece on tile
                {
                    possibleMoves.Add(new int[] { pawn.getX() + 1, pawn.getY() - 1 });
                }
            }
        }
        else//white team
        {
            if (tileWithinBoard(pawn.getX() - 1, pawn.getY() + 1))//within bounds, left
            {
                if (teamOnTile(pawn.getX() - 1, pawn.getY() + 1, !pawn.getTeam(), theBoard) && enemyTeam) //enemy piece on tile
                {
                    possibleMoves.Add(new int[] { pawn.getX() - 1, pawn.getY() + 1 });                       
                }
                else if (teamOnTile(pawn.getX() - 1, pawn.getY() + 1, pawn.getTeam(), theBoard) && !enemyTeam) //ally piece on tile
                {
                    possibleMoves.Add(new int[] { pawn.getX() - 1, pawn.getY() + 1 });
                }
            }
            if (tileWithinBoard(pawn.getX() + 1, pawn.getY() + 1))//within bounds, right
            {
                if (teamOnTile(pawn.getX() + 1, pawn.getY() + 1, !pawn.getTeam(), theBoard) && enemyTeam) //enemy piece on tile
                { 
                    possibleMoves.Add(new int[] { pawn.getX() + 1, pawn.getY() + 1 });                        
                }
                else if (teamOnTile(pawn.getX() + 1, pawn.getY() + 1, pawn.getTeam(), theBoard) && !enemyTeam) //enemy piece on tile
                {
                    possibleMoves.Add(new int[] { pawn.getX() + 1, pawn.getY() + 1 });
                }

            }
        }
        return possibleMoves;
    }

    void pawnPromotion() //Pawn Promotion 
    {
        if (selectedPiece.getType() == Piece.TYPE_PAWN)//selected piece is a pawn
        {
            if ((whoseTurn && selectedPiece.getY() == 7) || (!whoseTurn && selectedPiece.getY() == 0))//team with piece on oppoint
            {
                PromotePawn = false;
                REF_UI_TEXT_PAWN_PROMOTION.gameObject.SetActive(true);
            }
        }
    }
    public void toQueen_Click() //pawn to Queen 
    {
        //selectedPiece.setType(4);
        changePieceType(selectedPiece, Piece.TYPE_QUEEN);
        REF_UI_TEXT_PAWN_PROMOTION.gameObject.SetActive(false);
        PromotePawn = true;
        nextTurn();
    }
    public void toRook_Click() //pawn to Rook 
    {
        //selectedPiece.setType(3);
        changePieceType(selectedPiece, Piece.TYPE_ROOK);
        REF_UI_TEXT_PAWN_PROMOTION.gameObject.SetActive(false);
        PromotePawn = true;
        nextTurn();
    }
    public void toBishop_Click() //pawn to Bishop 
    {
        //selectedPiece.setType(2);
        changePieceType(selectedPiece, Piece.TYPE_BISHOP);
        REF_UI_TEXT_PAWN_PROMOTION.gameObject.SetActive(false);
        PromotePawn = true;
        nextTurn();
    }
    public void toKnight_Click() //pawn to Knight 
    {
        //selectedPiece.setType(1);
        changePieceType(selectedPiece, Piece.TYPE_KNIGHT);
        REF_UI_TEXT_PAWN_PROMOTION.gameObject.SetActive(false);
        PromotePawn = true;
        nextTurn();
    }

    public int AI_PawnPromotion(Piece pawn, int xPos, int yPos, Piece[,] theBoard)//looks at what the best move for AI is when promoting a pawn 
    {
        Piece[,] temptBoard = new Piece[8, 8];
        System.Array.Copy(theBoard, temptBoard, 64); //copies board to test
        temptBoard[xPos, yPos] = temptBoard[pawn.getX(), pawn.getY()]; //moves piece within temptBoard
        temptBoard[pawn.getX(), pawn.getY()] = null;

        temptBoard[xPos, yPos].setType(Piece.TYPE_KNIGHT);//test to see what would happen if it is a knight
        if (checkForChecks(!temptBoard[xPos, yPos].getTeam(), temptBoard[xPos, yPos], theBoard)) //If it cause a check
        {
            if (checkForCheckmates(!temptBoard[xPos, yPos].getTeam(), theBoard))
            { //and cause a checkmate, then pick knight
                return Piece.TYPE_KNIGHT;
            }
        }

        for (int t = 4; t >= 2; t--)
        {

            temptBoard[xPos, yPos].setType(t);//test to see what would happen if it was a queen, bishop, or rook
            if (!checkForStalemates(!temptBoard[xPos, yPos].getTeam(), theBoard)) //if a stalemate does not occur, then become selected type
            {
                return t;
            }
        }
        return Piece.TYPE_KNIGHT;//if all else fails, become a knight
    }

    //Knight//
    List<int[]> knightNormalMove(Piece knight, Piece[,] theBoard) //knight normal movement, L shape
    {
        List<int[]> possibleMoves = new List<int[]>();
        int xx = knight.getX();
        int yy = knight.getY();
        if (tileWithinBoard(xx-1,yy-2))//top left
        {
            if (!somethingOnTile(xx - 1, yy - 2, true, !knight.getTeam(), theBoard))//enemy on spot or empty
            {
                possibleMoves.Add(new int[] {xx-1,yy-2});
            }
        }
        if (tileWithinBoard(xx + 1, yy - 2))//top right
        {
            if (!somethingOnTile(xx + 1, yy - 2, true, !knight.getTeam(), theBoard))//enemy on spot or empty
            {
                possibleMoves.Add(new int[] { xx + 1, yy - 2 });
            }
        }

        if (tileWithinBoard(xx + 2, yy - 1))//right up
        {
            if (!somethingOnTile(xx + 2, yy - 1, true, !knight.getTeam(), theBoard))//enemy on spot or empty
            {
                possibleMoves.Add(new int[] { xx + 2, yy - 1 });
            }
        }
        if (tileWithinBoard(xx + 2, yy + 1))//right bottom
        {
            if (!somethingOnTile(xx + 2, yy + 1, true, !knight.getTeam(), theBoard))//enemy on spot or empty
            {
                possibleMoves.Add(new int[] { xx + 2, yy + 1 });
            }
        }

        if (tileWithinBoard(xx - 1, yy + 2))//down left
        {
            if (!somethingOnTile(xx - 1, yy + 2, true, !knight.getTeam(), theBoard))//enemy on spot or empty
            {
                possibleMoves.Add(new int[] { xx - 1, yy + 2 });
            }
        }
        if (tileWithinBoard(xx + 1, yy + 2))//down right
        {
            if (!somethingOnTile(xx + 1, yy + 2, true, !knight.getTeam(), theBoard))//enemy on spot or empty
            {
                possibleMoves.Add(new int[] { xx + 1, yy + 2 });
            }
        }

        if (tileWithinBoard(xx - 2, yy - 1))//left up
        {
            if (!somethingOnTile(xx - 2, yy - 1, true, !knight.getTeam(), theBoard))//enemy on spot or empty
            {
                possibleMoves.Add(new int[] { xx - 2, yy - 1 });
            }
        }
        if (tileWithinBoard(xx - 2, yy + 1))//left bottom
        {
            if (!somethingOnTile(xx - 2, yy + 1, true, !knight.getTeam(), theBoard))//enemy on spot or empty
            {
                possibleMoves.Add(new int[] { xx - 2, yy + 1 });
            }
        }
        return possibleMoves;
    }

    //Bishop//
    List<int[]> bishopNormalMove(Piece bishop, Piece[,] theBoard) //bishop normal movement, diagonal
    {
        List<int[]> possibleMoves = new List<int[]>();
        possibleMoves.AddRange(spotsInDirection(bishop, -1, -1, theBoard));
        possibleMoves.AddRange(spotsInDirection(bishop, -1,  1, theBoard));
        possibleMoves.AddRange(spotsInDirection(bishop,  1, -1, theBoard));
        possibleMoves.AddRange(spotsInDirection(bishop,  1,  1, theBoard));
        return possibleMoves;
    }

    //Rook//
    List<int[]> rookNormalMove(Piece rook, Piece[,] theBoard) //rook normal movement, left, right, up, down
    {
        List<int[]> possibleMoves = new List<int[]>();
        possibleMoves.AddRange(spotsInDirection(rook, -1, 0, theBoard));
        possibleMoves.AddRange(spotsInDirection(rook,  1, 0, theBoard));
        possibleMoves.AddRange(spotsInDirection(rook,  0,-1, theBoard));
        possibleMoves.AddRange(spotsInDirection(rook,  0, 1, theBoard));
        return possibleMoves;
    }


    List<int[]> spotsInDirection(Piece piece, int xDir, int yDir, Piece[,] theBoard)
    {
        List<int[]> possibleMoves = new List<int[]>();
        int xx = piece.getX();
        int yy = piece.getY();
        for(int i = 1; true; i++)
        {
        if (tileWithinBoard(xx + (xDir * i), yy + (yDir * i)))
            {
                if (!somethingOnTile(xx + (xDir * i), yy + (yDir * i), true, !piece.getTeam(), theBoard)) //tile is empty or has the opposite team on it
                {
                    possibleMoves.Add(new int[] { xx + (xDir * i), yy + (yDir * i) });
                    if (teamOnTile(xx + (xDir * i), yy + (yDir * i), !piece.getTeam(), theBoard))// if it is an enemy
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
    List<int[]> queenNormalMove(Piece queen, Piece[,] theBoard) //queen normal movement, left, right, up, down, diagonal
    {
        List<int[]> possibleMoves = new List<int[]>();
        possibleMoves.AddRange(bishopNormalMove(queen, theBoard));
        possibleMoves.AddRange(rookNormalMove(queen, theBoard));
        return possibleMoves;
    }

    //King//
    List<int[]> kingCastling(Piece king, Piece[,] theBoard)//a king castling
    {
        List<int[]> possibleMoves = new List<int[]>();
        if (!check && !king.getMoved())//if the king is not checked and the king hasn't moved
        {
            if (theBoard[0, king.getY()] != null && theBoard[1, king.getY()] == null && theBoard[2, king.getY()] == null && theBoard[3, king.getY()] == null)//left rook, two spots open
            {
                if(theBoard[0, king.getY()].getType() == Piece.TYPE_ROOK && !theBoard[0, king.getY()].getMoved())//piece is a rook and hasn't moved
                {
                    possibleMoves.Add(new int[] { 2, king.getY() });
                }
            }
            if (theBoard[7, king.getY()] != null && theBoard[6, king.getY()] == null && theBoard[5, king.getY()] == null)//right rook
            {
                if (theBoard[7, king.getY()].getType() == Piece.TYPE_ROOK && !theBoard[7, king.getY()].getMoved())//piece is a rook and hasn't moved
                {
                    possibleMoves.Add(new int[] { 6, king.getY() });
                }
            }
        }
        return possibleMoves;
    }

    List<int[]> kingNormalMove(Piece king, Piece[,] theBoard) //move in any direction one tile
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
                if (!somethingOnTile(xx, yy, true, !king.getTeam(), theBoard))
                {
                    startMoves.Add(new int[] {xx, yy});
                }
            }
        }
        List<int[]> enemyMoves = getPossibleMovesForTeam(!king.getTeam(), true, theBoard);
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

    public List<int[]> getPossibleMovesForTeam(bool team, bool withKing, Piece[,] theBoard) //gets all the possible moves a team  could make, with all square around their king //true = white, false = black
    {
        List<int[]> possibleMoves = new List<int[]>();
        for(int i = 0; i < 8; i++)//x
        {
            for(int j = 0; j < 8; j++)//y
            {
                if (theBoard[i, j] != null)//piece on it
                {
                    if(theBoard[i,j].getTeam() == team)//same team that is wanted
                    {
                        if(theBoard[i,j].getType() == Piece.TYPE_KING)//king
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
                        else if (theBoard[i, j].getType() == Piece.TYPE_PAWN)//pawn
                        {
                            possibleMoves.AddRange(pawnTakePiece(theBoard[i, j], true, theBoard));
                        }
                        else//another piece
                        {
                            possibleMoves.AddRange(getPossibleMoves(theBoard[i, j], theBoard));
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

}

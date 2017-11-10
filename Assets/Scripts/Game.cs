using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    private UnityEngine.UI.Button REF_UI_BUTTON;

    [SerializeField] private GameObject REF_W_KING;
    [SerializeField] private GameObject REF_W_QUEEN;
    [SerializeField] private GameObject REF_W_ROOK;
    [SerializeField] private GameObject REF_W_BISHOP;
    [SerializeField] private GameObject REF_W_KNIGHT;
    [SerializeField] private GameObject REF_W_PAWN;
    [SerializeField] private GameObject REF_B_KING;
    [SerializeField] private GameObject REF_B_QUEEN;
    [SerializeField] private GameObject REF_B_ROOK;
    [SerializeField] private GameObject REF_B_BISHOP;
    [SerializeField] private GameObject REF_B_KNIGHT;
    [SerializeField] private GameObject REF_B_PAWN;


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
    void Start() {
        //setup all references
        REF_UI_BUTTON = GameObject.Find("Button_Start").GetComponent<UnityEngine.UI.Button>();

        board = new Piece[8, 8];
    }

    // Update is called once per frame
    void Update() {
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

    public void resetGame() //for neural network and variable modification AI
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

        for(int pawnNum = 0; pawnNum <= 7; pawnNum++) //create all 8 white pawns
        {
            board[pawnNum, 6] = Instantiate(REF_W_PAWN).GetComponent<Piece>(); // white Pawn
            board[pawnNum, 6].setLocation(pawnNum, 6); //set location
            whiteTeam.pieces.Add(board[pawnNum, 6]); //adds to team
        }

        board[0, 7] = new Piece(0, 7, true, 3);//(0,7) white rook
        board[1, 7] = new Piece(1, 7, true, 1);//(1,7) white knight
        board[2, 7] = new Piece(2, 7, true, 2);//(2,7) white bishop
        board[3, 7] = new Piece(3, 7, true, 4);//(3,7) white queen
        board[4, 7] = new Piece(4, 7, true, 5);//(4,7) white king
        whiteTeam.king = board[4, 7]; //assign king piece
        board[5, 7] = new Piece(5, 7, true, 2);//(5,7) white bishop
        board[6, 7] = new Piece(6, 7, true, 1);//(6,7) white knight
        board[7, 7] = new Piece(7, 7, true, 3);//(7,7) white rook


        //Repopulates Black Side
    }

    void makeBoard() //does final touches to pieces' PictureBox
    {

    }

    void nextTurn()//moves to the next turn
    {

    }

    public void updateText(bool whoIsCheckmated)//updates the text on the screen
    {

    }
}

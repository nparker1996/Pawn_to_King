using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    private UnityEngine.UI.Button REF_UI_BUTTON;

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
    void Update() { }

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

    private void timer_Tick(object sender) //for AI's to do stuff
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    //list of pawn types that are constant
    public const int TYPE_PAWN = 0;
    public const int TYPE_KNIGHT = 1;
    public const int TYPE_BISHOP = 2;
    public const int TYPE_ROOK = 3;
    public const int TYPE_QUEEN = 4;
    public const int TYPE_KING = 5;

    private GameObject worldController;
    private int x; //x location of a piece
    private int y; //y location of a piece
    [SerializeField] private bool team; // team of the piece // true = white, false = black
    [SerializeField] private int type; //type of piece // 0 = Pawn, 1 = Knight, 2 = Bishop, 3 = Rook, 4 = Queen, 5 = King
    private int value; //value of a piece
    private bool moved; //piece has moved yet?
    private bool pawnDoubleMove; //for pawns only, if they just did their double openning move, of en passant move
    private int moveCount;
    List<int[]> moves;//list of possible Moves

    // Use this for initialization
    void Start()
    {
        worldController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        worldController.GetComponent<Game>().clickedPiece(x, y);
    }

    public Piece() //contructor
    {
        x = 0;
        y = 0;
        team = true;
        type = 0;
        value = 1;
        moved = false;
        pawnDoubleMove = false;
        moveCount = 0;
        moves = new List<int[]>();
        //determineImage();
    }

    public Piece(int X, int Y, bool Team, int Type) //contructor
    {
        x = X;
        y = Y;
        team = Team;
        type = Type;
        setValue();
        moved = false;
        pawnDoubleMove = false;
        moveCount = 0;
        moves = new List<int[]>();
        //determineImage();
    }

    //Getters and Setters
    public int getX()//return x location
    {
        return x;
    }

    public int getY()//return y location
    {
        return y;
    }

    public int[] getLocation() //return location of the piece
    {
        return new int[] { x, y };
    }

    public void setLocation(int X, int Y) //sets the location of the piece
    {
        x = X;
        y = Y;
        this.gameObject.transform.position = new Vector3(X, 0, Y);
    }

    public bool getTeam() //returns the team the piece is on
    {
        return team;
    }

    public void setTeam(bool Team) //sets the team the piece is on
    {
        team = Team;
    }

    public int getType() //gets the type of piece the piece is
    {
        return type;
    }

    public void setType(int Type) //sets the type of the piece
    {
        type = Type;
        setValue(); //also sets the value
        //determineImage();
    }

    public void testSetType(int Type)
    {
        type = Type;
    }

    public int getValue() //gets the value of the piece
    {
        return value;
    }

    private void setValue() //sets value of piece
    {
        switch (type)
        {
            case TYPE_PAWN: //pawn
                value = 1;
                break;
            case TYPE_KNIGHT: //knight
            case TYPE_BISHOP: //bishop
                value = 3;
                break;
            case TYPE_ROOK: //rook
                value = 5;
                break;
            case TYPE_QUEEN: //queen
                value = 9;
                break;
            case TYPE_KING://king
                value = 10;
                break;
            default:
                value = 0;
                break;
        }
    }

    public int getMoveCount()
    {
        return moveCount;
    }

    public void addMoveCount(int num)
    {
        moveCount += num;
    }

    public bool getMoved() //checks to see if piece has moved
    {
        return moved;
    }

    public void setMoved() //the piece has moved
    {
        moved = true;
    }

    public bool getPawnDoubleMove() //checks to see if pawn has moved two spots
    {
        return pawnDoubleMove;
    }

    public void setPawnDoubleMove(bool change) //the piece has moved
    {
        pawnDoubleMove = change;
    }

    public List<int[]> getMoves()
    {
        return moves;
    }

    //private void determineImage()
    //{
    //    char teamColor = 'B';
    //    if (this.team)//same team that is wanted
    //    {
    //        teamColor = 'W';
    //    }
    //    switch (type)
    //    {
    //        case 0: //pawn
    //            image.Image = Image.FromFile("images/" + teamColor + "Pawn.png");
    //            break;
    //        case 1: //knight
    //            image.Image = Image.FromFile("images/" + teamColor + "Knight.png");
    //            // box.Image = Image.FromFile("images/" + team + "KnightTest.png");
    //            break;
    //        case 2: //bishop
    //            image.Image = Image.FromFile("images/" + teamColor + "Bishop.png");
    //            break;
    //        case 3: //rook
    //            image.Image = Image.FromFile("images/" + teamColor + "Rook.png");
    //            break;
    //        case 4: //queen
    //            image.Image = Image.FromFile("images/" + teamColor + "Queen.png");
    //            break;
    //        case 5: //king
    //            image.Image = Image.FromFile("images/" + teamColor + "King.png");
    //            break;
    //        default:
    //            break;
    //    }
    //}
}

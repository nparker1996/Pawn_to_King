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

    }
}

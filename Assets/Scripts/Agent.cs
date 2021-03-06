﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour {

    public Game game;
    public Piece king;
    public List<Piece> pieces;
    public bool team; //true = white, false = black
    public int points;
    public string type; //the type of controller (human or AI)

    // Use this for initialization
    void Start ()
    {
    }

    void Awake()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
    }
	
	// Update is called once per frame
	void Update () {}

    public Agent(Game theGame, bool Team)
    {
        game = theGame;
        pieces = new List<Piece>();
        team = Team;
        points = 0;
        type = "Human";
    }

    abstract public void turn();

    public void setTeam(bool team) { this.team = team; }

    protected void printTurn()
    {
        Debug.Log("Turn " + game.turnCount + " - " + type);
    }
}

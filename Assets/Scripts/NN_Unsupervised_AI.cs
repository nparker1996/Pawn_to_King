using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NN_Unsupervised_AI : Agent {

    private struct Neuron
    {
        public List<int> inputW; //location of input weight
        public float value; //value being held in the neuron
        //public float bias; //bias that get added to calculation for value
        public List<int> outputW; //location of output weight
        public Neuron(float v)
        {
            inputW = new List<int>();
            value = v;
            outputW = new List<int>();
        }
    }

    private struct Weight
    {
        public int inputN; //location of input neuron
        public float value;
        public int outputN; //location of output neuron
        public Weight(int input, float val, int output)
        {
            inputN = input;
            value = val;
            outputN = output;
        }
    }

    private int numInputNeurons; //number of input neurons, the chess board
    private int layers; //number of layers in a neural network, between input and output neurons
    private int numNeuronsPerLayer; //number of neurons per hidden (middle) layer
    private int numOutputNeurons; //number of output neurons

    private Neuron[] neurons; //list of neurons input + hidden + output
    private Weight[] weights; //list of weights between neurons

    private int miniBatchSize;

    private float randomMin = -1; //random weights and bias min
    private float randomMax = 1; //random weights and bias max

    // Use this for initialization
    void Start()
    {
        type = "Unsupervised Neural Network";
        constructNewNeuralNetwork(64, 3, 32, 1);
        Debug.Log("nIN=" + numInputNeurons + ", l=" + layers + ", nNPL=" + numNeuronsPerLayer + ", nON=" + numOutputNeurons);

    }

    public NN_Unsupervised_AI(Game theGame, bool team) : base(theGame, team)
    {
        type = "Unsupervised Neural Network";
        constructNewNeuralNetwork(64, 3, 32, 1);
    }

    // Update is called once per frame
    void Update(){ }

    public override void turn() {
        Dictionary<Piece, List<double[]>> optimalChoices = new Dictionary<Piece, List<double[]>>(); //each piece with its moves and utilites (value) 
        double highestValue = -Mathf.Abs(numInputNeurons * layers * numNeuronsPerLayer * numOutputNeurons * randomMin * randomMax) - 1; // the highest value that has been returned

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
                    boardToInput(game.board);
                    movePieceInInput(game.board, p.getX(), p.getY(), move[0], move[1]);
                    propagate();
                    double output = (double) getOutputs()[0];
                    movesAndValues.Add(new double[] { move[0], move[1], output });
                    if(output > highestValue) { highestValue = output; }
                }
                optimalChoices.Add(p, movesAndValues);
            }


        }

        ////////////////////////////////////////////////
        ///determine the move with highest move value///
        List<Piece> bestMovesPieces = new List<Piece>();
        List<double[]> bestMovesMove = new List<double[]>();
        Debug.Log("Best Moves, high of " + highestValue);
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


    //Turn functions
    private void boardToInput(Piece[,] theBoard) //takes the board and turns it into input neurons
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //sets proper neuron to value <-10, 10>
                if (theBoard[i, j] != null)
                {
                    neurons[(i * 8) + j].value = theBoard[i, j].getTeam() ? theBoard[i, j].getValue() : -theBoard[i, j].getValue(); // white : black
                }
            }
        }
    }

    private void movePieceInInput(Piece[,] theBoard, int fromX, int fromY, int toX, int toY)
    {
        float fromValue = neurons[(fromX * 8) + fromY].value;
        neurons[(toX * 8) + toY].value = fromValue;
        neurons[(fromX * 8) + fromY].value = 0;
    }

    private void propagate()
    {
        for(int i = numInputNeurons; i < neurons.Length; i++) //for each neuron in neurons after the input neurons
        {
            float total = 0;
            foreach(int w in neurons[i].inputW) //for each weight in neuron's input wieghts
            {
                total += weights[w].value * neurons[weights[w].inputN].value;
            }
            neurons[i].value = total;
        }
    }

    private List<float> getOutputs()
    {
        List<float> outputs = new List<float>();
        for(int i = neurons.Length - numOutputNeurons; i < neurons.Length; i++)
        {
            outputs.Add(neurons[i].value);
        }
        return outputs;
    }

    //Utilitie function
    private void constructNewNeuralNetwork(int nIN, int l, int nNPL, int nON) //construct neural network with random weights and bias
    {
        numInputNeurons = nIN;
        layers = l;
        numNeuronsPerLayer = nNPL;
        numOutputNeurons = nON;
        randomMin = -1;
        randomMax = 1;

        neurons = new Neuron[numInputNeurons + (layers * numNeuronsPerLayer) + numOutputNeurons]; //new neurons
        for (int i = 0; i < neurons.Length; i++)
        {
            neurons[i] = new Neuron(0);
        }
        weights = new Weight[(numInputNeurons + (numNeuronsPerLayer * (layers - 1)) + numOutputNeurons) * numNeuronsPerLayer];//new weights
        int w = 0; //count of weights
        for (int i = 0; i < neurons.Length - numOutputNeurons && w < weights.Length; i++) //each node
        {
            int outNeuron = -1;
            int numWeights = i >= neurons.Length - numNeuronsPerLayer - numOutputNeurons ? numOutputNeurons : numNeuronsPerLayer;
            for (int j = 0; j < numWeights; j++)
            {
                if (i < numInputNeurons) //within input node range
                {
                    outNeuron = numInputNeurons + (w % numNeuronsPerLayer);
                }
                else if (i >= neurons.Length - numNeuronsPerLayer - numOutputNeurons) //within output node range
                {
                    int wm = w - (numNeuronsPerLayer * (neurons.Length - numOutputNeurons - numNeuronsPerLayer));
                    outNeuron = numInputNeurons + (layers * numNeuronsPerLayer) + (wm % numOutputNeurons);
                }
                else // within hidden node range
                {
                    outNeuron = numInputNeurons + (getLayer(i) * numNeuronsPerLayer) + (w % numNeuronsPerLayer);
                }
                float rand = Random.Range(randomMin, randomMax);
                weights[w] = new Weight(i, rand, outNeuron);//creating weight
                neurons[i].outputW.Add(w); //added weight as an output
                neurons[outNeuron].inputW.Add(w); //add weight as an input
                w++;
            }
        }
    }

    private int getNeuronLoc(int layer, int neuronLocInLayer)//gets the neuron's location in neuron array 
    {
        if (layer == 0) //input layer
        {
            if (neuronLocInLayer < numInputNeurons)
            {
                return neuronLocInLayer;
            }
        }
        else if (layer <= layers) //hidden layers
        {
            if (neuronLocInLayer < numNeuronsPerLayer)
            {
                return numInputNeurons + ((layer - 1) * numNeuronsPerLayer) + neuronLocInLayer;
            }
        }
        else if (layer == layers + 1)//output layer
        {
            if (neuronLocInLayer < numOutputNeurons)
            {
                return numInputNeurons + (layers * numNeuronsPerLayer) + neuronLocInLayer;
            }
        }
        return -1;
    }

    private int getWeightLoc(int neuronLoc, int weightLocOfNeuron) //gets weight location in weight array
    {
        if ((neuronLoc * numNeuronsPerLayer) + weightLocOfNeuron < weights.Length - (numNeuronsPerLayer * numOutputNeurons))
        {
            return (neuronLoc * numNeuronsPerLayer) + weightLocOfNeuron;
        }
        return -1;
    }

    private int getLayer(int nPos)
    {
        return (int)(((nPos - numInputNeurons)/numNeuronsPerLayer) + 1);
    }
}

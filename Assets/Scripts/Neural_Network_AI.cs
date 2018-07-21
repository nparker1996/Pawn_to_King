using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neural_Network_AI : Agent
{

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
    void Start() {
        type = "Neural Network";
        constructNewNeuralNetwork(64, 3, 32, 1);
    }

    public Neural_Network_AI(Game theGame, bool team) : base(theGame, team)
    {
        type = "Neural Network";
        constructNewNeuralNetwork(64, 3, 32, 1);
    }

	// Update is called once per frame
	void Update () {
		
	}

    public override void turn() { }

    //Utilitie function
    //Utilitie function
    private void constructNewNeuralNetwork(int nIN, int l, int nNPL, int nON) //construct neural network with random weights and bias
    {
        numInputNeurons = nIN;
        layers = l;
        numNeuronsPerLayer = nNPL;
        numOutputNeurons = nON;

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
                weights[w] = new Weight(i, Random.Range(randomMin, randomMax), outNeuron);//creating weight
                neurons[i].outputW.Add(w); //added weight as an output
                neurons[outNeuron].inputW.Add(w); //add weight as an input
                w++;
            }
        }
    }

    private void boardToInput(Piece[,] theBoard) //takes the board and turns it into input neurons
    {
        for(int i = 0; i < theBoard.Length; i++)
        {
            for(int j = 0; j < theBoard.Length; j++)
            {
                //sets proper neuron to value <-10, 10>
                neurons[(i * theBoard.Length) + j].value = theBoard[i, j].getTeam() ? theBoard[i, j].getValue() : -theBoard[i, j].getValue(); // white : black
            }
        }
    }

    private int getNeuronLoc(int layer, int neuronLocInLayer)//gets the neuron's location in neuron array 
    {
        if(layer == 0) //input layer
        {
            if(neuronLocInLayer < numInputNeurons)
            {
                return neuronLocInLayer;
            }
        }
        else if(layer <= layers) //hidden layers
        {
            if (neuronLocInLayer < numNeuronsPerLayer)
            {
                return numInputNeurons + ((layer - 1) * numNeuronsPerLayer) + neuronLocInLayer;
            }
        }
        else if(layer == layers + 1)//output layer
        {
            if(neuronLocInLayer < numOutputNeurons)
            {
                return numInputNeurons + (layers * numNeuronsPerLayer) + neuronLocInLayer;
            }
        }
        return -1;
    }
    
    private int getWeightLoc(int neuronLoc, int weightLocOfNeuron) //gets weight location in weight array
    {
        if((neuronLoc * numNeuronsPerLayer) + weightLocOfNeuron < weights.Length - (numNeuronsPerLayer * numOutputNeurons))
        {
            return (neuronLoc * numNeuronsPerLayer) + weightLocOfNeuron;
        }
        return -1;
    }

    private int getLayer(int nPos)
    {
        return (int)(((nPos - numInputNeurons) / numNeuronsPerLayer) + 1);
    }
}

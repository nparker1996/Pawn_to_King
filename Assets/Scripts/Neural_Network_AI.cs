using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neural_Network_AI : Agent
{

    private int numInputNeurons; //number of input neurons, the chess board
    private int layers; //number of layers in a neural network, between input and output neurons
    private int numNeuronsPerLayer; //number of neurons per hidden (middle) layer
    private int numOutputNeurons; //number of output neurons

    private float[] neurons; //list of neurons input + hidden + output
    private float[] weights; //list of weights between neurons

    private int miniBatchSize;

    private float randomMin = -10; //random weights and bias min
    private float randomMax = 10; //random weights and bias max

    // Use this for initialization
    void Start() {
        type = "Neural Network";
    }

    public Neural_Network_AI(Game theGame, bool team) : base(theGame, team)
    {
        type = "Neural Network";
    }

	// Update is called once per frame
	void Update () {
		
	}

    public override void turn() { }

    //Utilitie function
    private void constructNewNeuralNetwork() //construct neural network with random weights and bias
    {
        neurons = new float[numInputNeurons + (layers * numNeuronsPerLayer) + numOutputNeurons]; //new neurons
        weights = new float[(numInputNeurons + numNeuronsPerLayer * (layers - 1) + numOutputNeurons) * numNeuronsPerLayer];//new weights
        for(int i = numInputNeurons; i < neurons.Length - numOutputNeurons; i++)
        {
            neurons[i] = Random.Range(randomMin, randomMax);
        }
        for(int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(randomMin, randomMax);
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
}

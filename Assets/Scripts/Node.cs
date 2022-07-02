using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string letter;
    public enum GuessState { NoGuess, Incorrect, Correct, Close }
    public GuessState guessState;
}

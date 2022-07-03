using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    public string nodeCurrentLetter;

    public int rowNum;
    public int colNum;

    public enum GuessState { NoGuess, Incorrect, Correct, Close }
    public GuessState guessState;

    public TextMeshProUGUI letterText;

    // This function updates the node's letter
    public void UpdateLetter(string character)
    {
        nodeCurrentLetter = character;
        letterText.text = character;
    }
}

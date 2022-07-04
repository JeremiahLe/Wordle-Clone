using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public string nodeCurrentLetter;

    public int rowNum;
    public int colNum;

    public enum GuessState { NoGuess, Incorrect, Correct, Close }
    public GuessState guessState;

    public TextMeshProUGUI letterText;
    public Image nodeColor;
    public Animator nodeAnimator;

    // This function updates the node's letter
    public void UpdateLetter(string character)
    {
        nodeCurrentLetter = character;
        letterText.text = character;
    }

    // This function sets the nodes new state based on the guess
    public void SetNodeState(GuessState newState)
    {
        nodeAnimator.SetBool("showGuess", true);

        switch (newState)
        {
            case GuessState.Close:
                guessState = newState;
                nodeColor.color = Color.yellow;
                break;

            case GuessState.Correct:
                guessState = newState;
                nodeColor.color = Color.green;
                break;

            case GuessState.Incorrect:
                guessState = newState;
                nodeColor.color = Color.gray;
                break;

            default:
                break;
        }
    }

    // This function is called when the guess is entered
    public void PlayGuessAnimation(GuessState newState)
    {
        nodeAnimator.SetBool("showGuess", true);
        guessState = newState;
    }

    // This function is called when the animation is finished
    public void EndGuessAnimation()
    {
        SetNodeState(guessState);
    }
}

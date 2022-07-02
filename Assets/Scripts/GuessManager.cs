using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class GuessManager : MonoBehaviour
{
    public TextAsset file;
    public string[] listOfWords;

    public string currentWord;
    public int guessesRemaining = 5;
    public Word.LetterCount letterCount;

    public TextMeshProUGUI currentWordDisplay;
    public TextMeshProUGUI currentGuess;

    // Start is called before the first frame update
    void Start()
    {
        GetRandomWord();
    }

    // This function returns a random word based on the wordlist passed in
    public string ReturnRandomWord(string[] listOfWords)
    {
        int randomIndex = Random.Range(0, listOfWords.Length);
        return listOfWords[randomIndex].ToUpper();
    }

    // This function gets a random word and displays it
    public void GetRandomWord()
    {
        string[] lines = file.text.Split('\n');
        listOfWords = lines;

        currentWord = ReturnRandomWord(lines);
        currentWordDisplay.text = ($"Current Word:\n{currentWord}");
    }

    // This function gets the path of a list based on letter count 
    public string GetListOfWords(Word.LetterCount letterCount)
    {
        switch (letterCount)
        {
            case (Word.LetterCount.Five):
                return ("Resources/ListOf5LetterWords.txt");

            case (Word.LetterCount.Six):
                return ("Resources/ListOf6LetterWords.txt");

            case (Word.LetterCount.Seven):
                return ("Resources/ListOf7LetterWords.txt");

            default:
                return ("Resources/ListOf5LetterWords.txt");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Linq;
using System.Text;

public class GuessManager : MonoBehaviour
{
    public string[] listOfWords;

    public TextAsset FiveLetterWordList;
    public TextAsset SixLetterWordList;
    public TextAsset SevenLetterWordList;

    public string currentWord;
    public Word.LetterCount letterCount;

    public int guessesAllowed = 5;
    public int guessesRemaining = 5;

    public TextMeshProUGUI currentWordDisplay;
    public string currentGuess;

    public Transform nodePrefab;
    public Transform nodeParent;
    public Transform startPos;
    public float nodeSpacing = 2.5f;

    public List<Transform> nodeArray;
    public Transform currentNode;
    public int currentRow = -1;
    public int currentCol = -1;

    // Start is called before the first frame update
    private void Start()
    {
        GetRandomWord();
        SpawnNodes();
    }

    // OnGUI handles input events
    private void OnGUI()
    {
        Event e = Event.current;

        // Only keep track of alphabetical keycodes
        if (e.type == EventType.KeyDown && e.keyCode.ToString().Length == 1 && char.IsLetter(e.keyCode.ToString()[0]))
        {
            if (currentCol < ReturnLetterCountInt() - 1)
            {
                currentNode.gameObject.GetComponent<Node>().UpdateLetter(e.keyCode.ToString());
                UpdateArrayPosition(1);
            }
        }
        // Remove current letter
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Backspace)
        {
            if (currentCol > -1)
            {
                UpdateArrayPosition(-1);
                currentNode.gameObject.GetComponent<Node>().UpdateLetter("");
            }
        }
        // Enter guessed word
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
        {
            AppendGuess();
        }
    }

    // This function appends the user's guess in a string then checks its validity
    public void AppendGuess()
    {
        StringBuilder appendedGuess = new StringBuilder();
        List<Transform> nodeRow = nodeArray.Where(node => node.gameObject.GetComponent<Node>().rowNum == currentRow + 1).ToList();

        foreach (Transform node in nodeRow)
        {
            string letter = node.gameObject.GetComponent<Node>().nodeCurrentLetter;
            appendedGuess.Append($"{letter}");
        }

        currentGuess = appendedGuess.ToString().ToUpper().Trim();
        CheckGuess();
    }

    // This function checks the guesse's validity
    public void CheckGuess()
    {
        if (currentGuess.Count() != ReturnLetterCountInt())
        {
            return;       
        }

        if (currentGuess == currentWord)
        {
            Debug.Log("You win!");
        }

        foreach (string word in listOfWords)
        {
            if (word.ToUpper().Trim() == currentGuess)
            {
                Debug.Log("Incorrect Guess!");
                DisplayGuessResults();
                SetNewNodeAndRow();
                return;
            }
        }

        Debug.Log("Word not in word pool!");
    }

    // This function assigns the node state based on the guessed word
    public void DisplayGuessResults()
    {
    }

    // This function moves to the next row after an incorrect guess
    public void SetNewNodeAndRow()
    {
        currentCol = -1;
        currentRow += 1;

        Transform firstNode = nodeArray.Where(first => first.GetComponent<Node>().rowNum == currentRow + 1 && first.GetComponent<Node>().colNum == currentCol + 1).ToList().First();
        currentNode = firstNode;
    }

    // This function tracks where the current position is in the grid
    public void UpdateArrayPosition(int pos)
    {
        currentCol += pos;
        if (currentCol != ReturnLetterCountInt() - 1)
        {
            currentNode = nodeArray.Where(newNode => newNode.GetComponent<Node>().colNum == currentCol + 1 && newNode.GetComponent<Node>().rowNum == currentRow + 1).First();
        }
    }

    // This function returns a random word based on the wordlist passed in
    public string ReturnRandomWord(string[] listOfWords)
    {
        int randomIndex = Random.Range(0, listOfWords.Length);
        return listOfWords[randomIndex].ToUpper().Trim();
    }

    // This function gets a random word and displays it
    public void GetRandomWord()
    {
        string[] lines = GetListOfWords(letterCount).text.Split('\n');
        listOfWords = lines;

        currentWord = ReturnRandomWord(lines);
        currentWordDisplay.text = ($"Current Word:\n");
    }

    // This function shows the current word
    public void ShowCurrentWord()
    {
        currentWordDisplay.text += ($"{currentWord}");
    }

    // This function spawns the grid of nodes for the guesses
    public void SpawnNodes()
    {
        for (int row = 0; row < guessesAllowed; row++)
        {
            for (int col = 0; col < ReturnLetterCountInt(); col++)
            {
                Vector3 startingPos = new Vector3(startPos.position.x + col * nodeSpacing, startPos.position.y - row * nodeSpacing, startPos.position.z);
                Transform _node = Instantiate(nodePrefab, startingPos, Quaternion.identity);
                _node.SetParent(nodeParent);
                _node.gameObject.GetComponent<Node>().rowNum = row;
                _node.gameObject.GetComponent<Node>().colNum = col;
                nodeArray.Add(_node);
            }
        }

        Transform firstNode = nodeArray.Where(first => first.GetComponent<Node>().rowNum == currentRow + 1&& first.GetComponent<Node>().colNum == currentCol + 1).ToList().First();
        currentNode = firstNode;
    }

    // This function gets the path of a list based on letter count 
    public TextAsset GetListOfWords(Word.LetterCount letterCount)
    {
        switch (letterCount)
        {
            case (Word.LetterCount.Five):
                return FiveLetterWordList;

            case (Word.LetterCount.Six):
                return SixLetterWordList;

            case (Word.LetterCount.Seven):
                return SevenLetterWordList;

            default:
                return FiveLetterWordList;
        }
    }

    // This function returns the int of the assigned wordcount enum
    public int ReturnLetterCountInt()
    {
        switch (letterCount)
        {
            case (Word.LetterCount.Five):
                return 5;

            case (Word.LetterCount.Six):
                return 6;

            case (Word.LetterCount.Seven):
                return 7;

            default:
                return 5;
        }
    }
}

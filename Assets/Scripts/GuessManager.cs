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
    public string currentGuess;

    public Word.LetterCount letterCount;

    public int guessesAllowed = 5;
    public int guessesRemaining = 5;

    public int currentWordStreak = 0;

    public int currentRow = -1;
    public int currentCol = -1;

    public TextMeshProUGUI currentWordStreakText;
    public TextMeshProUGUI currentWordDisplay;

    public Transform currentNode;

    public Transform nodePrefab;
    public Transform nodeParent;
    public Transform startPos;

    public float nodeSpacing = 2.5f;

    public List<Transform> nodeArray;
    public List<Transform> currentRowOfNodes;

    public GameObject messageBox;

    bool gameOver = false;

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

        if (gameOver == true)
        {
            return;
        }

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
        // Do you have enough letters?
        if (currentGuess.Count() != ReturnLetterCountInt())
        {
            return;       
        }

        // Did you guess the word correctly?
        if (currentGuess == currentWord)
        {
            //Debug.Log("You win!");
            gameOver = true;
            currentWordStreak += 1;
            currentWordStreakText.text = ($"Current Streak: {currentWordStreak}");
            DisplayGuessResults();
            return;
        }

        // Does the word exist in the word pool?
        foreach (string word in listOfWords)
        {
            if (word.ToUpper().Trim() == currentGuess)
            {
                //Debug.Log("Incorrect Guess!");
                guessesRemaining -= 1;
                DisplayGuessResults();
                return;
            }
        }

        messageBox.SetActive(true);
        messageBox.GetComponent<MessageManager>().SetMessage("Word not in word pool!");
        //Debug.Log("Word not in word pool!");  
    }

    // This function assigns the node state based on the guessed word
    public void DisplayGuessResults()
    {
        currentRowOfNodes = nodeArray.Where(row => row.gameObject.GetComponent<Node>().rowNum == currentRow + 1).ToList();
        int i = 0;

        foreach(Transform node in currentRowOfNodes)
        {
            if (node.gameObject.GetComponent<Node>().nodeCurrentLetter == currentWord[i].ToString())
            {
                node.gameObject.GetComponent<Node>().PlayGuessAnimation(Node.GuessState.Correct);
                //Debug.Log($"Letter at position {i} is correct.");
            }
            else if (currentWord.Contains(node.gameObject.GetComponent<Node>().nodeCurrentLetter))
            {
                node.gameObject.GetComponent<Node>().PlayGuessAnimation(Node.GuessState.Close);
                //Debug.Log($"Letter at position {i} exists somewhere else.");
            }
            else if (!currentWord.Contains(node.gameObject.GetComponent<Node>().nodeCurrentLetter))
            {
                node.gameObject.GetComponent<Node>().PlayGuessAnimation(Node.GuessState.Incorrect);
                //Debug.Log($"Letter at position {i} doesn't exist at all.");
            }
            i++;
        }

        // Check if game not over
        if (guessesRemaining > 0 && gameOver == false)
        {
            SetNewNodeAndRow();
        }
        else if (guessesRemaining <= 0)
        {
            currentWordStreak = 0;
            currentWordStreakText.text = ($"Current Streak: {currentWordStreak}");

            messageBox.SetActive(true);
            messageBox.GetComponent<MessageManager>().SetMessage($"Word was {currentWord}!");
            Invoke("RestartGame", 1.0f);
        }
        else
        {
            Invoke("RestartGame", 1.0f);
        }
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
        currentWordDisplay.text = ($"Current Word:\n{currentWord}");
        currentWordStreak = 0;
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

        Transform firstNode = nodeArray.Where(first => first.GetComponent<Node>().rowNum == currentRow + 1 && first.GetComponent<Node>().colNum == currentCol + 1).ToList().First();
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

    // This function resets the game state
    public void RestartGame()
    {
        currentRow = -1;
        currentCol = -1;
        guessesRemaining = guessesAllowed;
        gameOver = false;

        DestroyNodes();
        GetRandomWord();
        SpawnNodes();
    }

    // This function clears the current nodeArray to create a new one
    public void DestroyNodes()
    {
        foreach (Transform node in nodeArray.ToArray())
        {
            nodeArray.Remove(node);
            Destroy(node.gameObject);
        }
    }
}

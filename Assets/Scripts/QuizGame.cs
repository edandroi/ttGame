using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizGame : MonoBehaviour
{
    QuizDataLoader quizDataLoader;
    public TextMeshProUGUI questionText;
    public Button twainButton;
    public Button tilbeButton;
    public TextMeshProUGUI resultText;
    public Button nextButton;
    public float nextQuestionDelay = 5f;

    private List<string> shownQuestions = new List<string>();
    private string currentQuestion;
    private bool isAnswerTwain;
    private bool isAnswerTilbe;
    private Coroutine timerCoroutine;

    private void Start()
    {
        FindQuizDataLoader();

        if (quizDataLoader != null)
        {
            if (quizDataLoader.QuizDataCount > 0)
            {
                DisplayRandomQuestion();
            }
            else
            {
                Debug.LogError("No quiz data available.");
            }
        }
        else
        {
            Debug.LogError("QuizDataLoader component not found in the scene. Make sure the script is attached to the same GameObject as QuizDataLoader or add QuizDataLoader to the scene.");
        }
        
        if (quizDataLoader == null)
        {
            FindQuizDataLoader();
        }

        if (quizDataLoader.QuizDataCount == 0)
        {
            FindQuizDataLoader();
        }
    }


    private void FindQuizDataLoader()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("quiz_data");
        if (csvFile == null)
        {
            Debug.LogError("Failed to load CSV file. Make sure it exists in the 'Resources' folder with the name 'quiz_data.csv'.");
            return;
        }

        quizDataLoader = gameObject.AddComponent<QuizDataLoader>();
        quizDataLoader.LoadQuizData();
    }
    
    private void DisplayRandomQuestion()
    {
        // Get a random question that has not been shown before
        do
        {
            currentQuestion = quizDataLoader.GetRandomQuestion();
        } while (shownQuestions.Contains(currentQuestion));

        shownQuestions.Add(currentQuestion);

        // Retrieve answer indicators for the current question
        isAnswerTwain = quizDataLoader.GetAnswerTwain(currentQuestion);
        isAnswerTilbe = quizDataLoader.GetAnswerTilbe(currentQuestion);

        // Check if the questionText reference is valid
        if (questionText == null)
        {
            currentQuestion = quizDataLoader.GetRandomQuestion();
        }

        // Display the question text
        questionText.text = currentQuestion;

        // Enable answer buttons
        twainButton.interactable = true;
        tilbeButton.interactable = true;

        // Hide result text and next button
        resultText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }

    public void OnTwainButtonClicked()
    {
        Debug.Log("twain clicked");
        // Disable answer buttons
        twainButton.interactable = false;
        tilbeButton.interactable = false;

        // Check the answer
        if (isAnswerTwain)
        {
            Debug.Log("twain correct");
            ShowResult(true);
        }
        else
        {
            ShowResult(false);
        }
    }

    public void OnTilbeButtonClicked()
    {
        // Disable answer buttons
        twainButton.interactable = false;
        tilbeButton.interactable = false;

        // Check the answer
        if (isAnswerTilbe)
        {
            ShowResult(true);
        }
        else
        {
            ShowResult(false);
        }
    }

    public void OnNextButtonClicked()
    {
        // Stop the timer coroutine
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        DisplayRandomQuestion();
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(nextQuestionDelay);

        // Timer has expired, display the next question
        DisplayRandomQuestion();
    }

    private void ShowResult(bool isCorrect)
    {
        // Start the timer coroutine
        timerCoroutine = StartCoroutine(StartTimer());
        
        resultText.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);

        if (isCorrect)
        {
            resultText.text = "CORRECT!";
        }
        else
        {
            resultText.text = "WRONG :(";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QuizDataLoader : MonoBehaviour
{
    public string csvFileName = "quiz_data";

    private Dictionary<string, Dictionary<string, bool>> quizData = new Dictionary<string, Dictionary<string, bool>>();
    private List<string> questionList = new List<string>();

    public int QuizDataCount => questionList.Count;

    private void Start()
    {
        LoadQuizData();
        // You can access the quiz data dictionary here and use it in your game
        // Example usage:
        string question = GetRandomQuestion();
        bool isAnswerTwain = GetAnswerTwain(question);
        bool isAnswerTilbe = GetAnswerTilbe(question);
        Debug.Log($"Question: {question}\nTwain: {isAnswerTwain}\nTilbe: {isAnswerTilbe}");
    }

    public void LoadQuizData()
    {
        TextAsset csvFile = Resources.Load<TextAsset>(csvFileName);
        if (csvFile == null)
        {
            Debug.LogError($"Failed to load CSV file '{csvFileName}'. Make sure it exists in the 'Resources' folder.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Start from 1 to skip the header line
        {
            string[] columns = lines[i].Split(',');
            if (columns.Length >= 3)
            {
                string question = columns[0];
                bool isAnswerTwain = columns[1] == "Y";
                bool isAnswerTilbe = columns[2] == "Y";

                if (!quizData.ContainsKey(question))
                {
                    Dictionary<string, bool> answers = new Dictionary<string, bool>
                    {
                        { "Twain", isAnswerTwain },
                        { "Tilbe", isAnswerTilbe }
                    };

                    quizData.Add(question, answers);
                    questionList.Add(question);
                }
                else
                {
                    Debug.LogWarning($"Duplicate question found: '{question}'. Skipping this question.");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid data format in line {i + 1} of CSV file: {lines[i]}");
            }
        }

        if (questionList.Count == 0)
        {
            Debug.LogWarning("No quiz questions available.");
        }
    }
    
    public string GetRandomQuestion()
    {
        if (questionList.Count > 0)
        {
            int randomIndex = Random.Range(0, questionList.Count);
            string randomQuestion = questionList[randomIndex];
            return randomQuestion;
        }
        else
        {
            Debug.LogWarning("No quiz questions available.");
            return string.Empty;
        }
    }

    public bool GetAnswerTwain(string question)
    {
        if (quizData.ContainsKey(question))
        {
            return quizData[question]["Twain"];
        }
        else
        {
            Debug.LogWarning($"Question '{question}' not found in quiz data.");
            return false;
        }
    }

    public bool GetAnswerTilbe(string question)
    {
        if (quizData.ContainsKey(question))
        {
            return quizData[question]["Tilbe"];
        }
        else
        {
            Debug.LogWarning($"Question '{question}' not found in quiz data.");
            return false;
        }
    }
}

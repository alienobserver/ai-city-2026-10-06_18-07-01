using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MathPuzzleTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mathMenuPanel;
    public GameObject gameStatsMenuPanel;              
    public TMP_Text questionText;          
    public TMP_InputField answerInputField; 
    public Button submitButton;            

    [Header("Visual Settings")]
    public float rotationSpeed = 100f;     // Speed at which the marker spins

    // Global flag so other scripts know the puzzle is open
    public static bool IsPuzzleActive = false;

    private bool puzzleSolved = false;
    private int correctAnswer;

    void Start()
    {
        if (mathMenuPanel != null)
            mathMenuPanel.SetActive(false);

        if( gameStatsMenuPanel != null) gameStatsMenuPanel.SetActive(true);

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(CheckAnswer);
        }
    }

    // New Update method to make the trigger spin continuously
    void Update()
    {
        // Rotate around the Y axis. 
        // Using unscaledDeltaTime ensures it keeps spinning even when Time.timeScale = 0.
        transform.Rotate(Vector3.up * rotationSpeed * Time.unscaledDeltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !puzzleSolved)
        {
            OpenMathPuzzle();
        }
    }

    void OpenMathPuzzle()
    {
        IsPuzzleActive = true; 

        if (mathMenuPanel != null)
            mathMenuPanel.SetActive(true);

        if (gameStatsMenuPanel != null)
            gameStatsMenuPanel.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int num1 = Random.Range(1, 10);
        int num2 = Random.Range(1, 10);
        correctAnswer = num1 * num2;

        questionText.text = $"Solve to proceed:\nWhat is {num1} × {num2}?";
        answerInputField.text = "";
        answerInputField.ActivateInputField();
    }

    public void CheckAnswer()
    {
        if (int.TryParse(answerInputField.text, out int playerAnswer))
        {
            if (playerAnswer == correctAnswer)
            {
                Debug.Log("Correct!");
                puzzleSolved = true;
                CloseMathPuzzle();
            }
            else
            {
                Debug.Log("Incorrect, try again!");
                questionText.text = "Incorrect! Try again:\n" + questionText.text;
                answerInputField.text = "";
                answerInputField.ActivateInputField();
            }
        }
    }

    void CloseMathPuzzle()
    {
        IsPuzzleActive = false; 
        puzzleSolved = false;

        if (mathMenuPanel != null)
            mathMenuPanel.SetActive(false);

        if (gameStatsMenuPanel != null)
            gameStatsMenuPanel.SetActive(true);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
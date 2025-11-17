using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionDialogueController : MonoBehaviour
{
    [Header("Dados da Pergunta")]
    public QuestionDialogueData questionData;

    [Header("UI Components")]
    public DialogueUI dialogueUI;
    public Button buttonOption1;
    public Button buttonOption2;
    public TypeTextAnimation typeTextAnimation;

    [Header("Configuração")]
    public MonoBehaviour playerMovementScript;

    [Header("Referências Externas")]
    public InteractionsI interactionsScript; // Referência ao outro script
    public GameObject objetoParaDestruir;    // Objeto que será destruído

    private bool isDialogueActive = false;
    private bool isShowingAnswer = false;

    private enum State { ShowingQuestion, ShowingAnswer }
    private State currentState;
    public GameObject player;

    void Start()
    {
        StartDialogue();
    }

    void StartDialogue()
    {
        if (questionData == null)
        {
            Debug.LogError("QuestionDialogueData não foi atribuído!");
            return;
        }

        isDialogueActive = true;
        isShowingAnswer = false;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        currentState = State.ShowingQuestion;
        ShowQuestion();
    }

    void ShowQuestion()
    {
        dialogueUI.Enable();
        dialogueUI.SetName(questionData.questionName);

        buttonOption1.gameObject.SetActive(true);
        buttonOption2.gameObject.SetActive(true);

        buttonOption1.interactable = false;
        buttonOption2.interactable = false;

        buttonOption1.GetComponentInChildren<TextMeshProUGUI>().text = questionData.option1Text;
        buttonOption2.GetComponentInChildren<TextMeshProUGUI>().text = questionData.option2Text;

        typeTextAnimation.textObject = dialogueUI.talkText;
        typeTextAnimation.fullText = questionData.questionText;

        typeTextAnimation.TypeFinished = () =>
        {
            buttonOption1.interactable = true;
            buttonOption2.interactable = true;
        };

        typeTextAnimation.StartTyping();
    }

    public void OnOption1Selected()
    {
        HandleChoice(0);
        player.GetComponent<InteractionsI>().afeto += 25;
    }

    public void OnOption2Selected()
    {
        HandleChoice(1);
    }

    void HandleChoice(int optionIndex)
    {
        if (isShowingAnswer) return;

        bool chosenCorrect = (optionIndex == 0 && questionData.option1IsCorrect) ||
                             (optionIndex == 1 && questionData.option2IsCorrect);

        isShowingAnswer = true;

        buttonOption1.interactable = false;
        buttonOption2.interactable = false;

        ShowAnswer(chosenCorrect);
    }

    void ShowAnswer(bool correct)
    {
        currentState = State.ShowingAnswer;

        string name = correct ? questionData.correctName : questionData.wrongName;
        string text = correct ? questionData.correctText : questionData.wrongText;

        dialogueUI.SetName(name);
        typeTextAnimation.fullText = text;

        typeTextAnimation.TypeFinished = () =>
        {
            if (correct)
                Invoke(nameof(EndDialogue), 2f);
            else
                Invoke(nameof(ReturnToQuestion), 2f);
        };

        typeTextAnimation.StartTyping();
    }

    void ReturnToQuestion()
    {
        isShowingAnswer = false;
        currentState = State.ShowingQuestion;
        ShowQuestion();
    }

    void EndDialogue()
    {
        isDialogueActive = false;

        // Esconde os botões ao finalizar
        buttonOption1.gameObject.SetActive(false);
        buttonOption2.gameObject.SetActive(false);

        dialogueUI.Disable();

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        

        if (objetoParaDestruir != null)
        {
            Destroy(objetoParaDestruir);
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "QuestionDialogueData", menuName = "ScriptableObject/QuestionDialogueData", order = 2)]
public class QuestionDialogueData : ScriptableObject
{
    [Header("Pergunta")]
    public string questionName;
    [TextArea(3,10)]
    public string questionText;

    [Header("Opções")]
    public string option1Text;
    public bool option1IsCorrect;

    public string option2Text;
    public bool option2IsCorrect;

    [Header("Resposta correta")]
    public string correctName;
    [TextArea(3,10)]
    public string correctText;

    [Header("Resposta errada")]
    public string wrongName;
    [TextArea(3,10)]
    public string wrongText;
}

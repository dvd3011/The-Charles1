using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    Image background;
    TextMeshProUGUI nameText;
    public TextMeshProUGUI talkText;  // Agora público para acessar em outro script
    public float speed = 10f;
    bool open = false;

    void Awake()
    {
        background = transform.GetChild(0).GetComponent<Image>();
        nameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        talkText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (open)
        {
            background.fillAmount = Mathf.Lerp(background.fillAmount, 1, speed * Time.deltaTime);
        }
        else
        {
            background.fillAmount = Mathf.Lerp(background.fillAmount, 0, speed * Time.deltaTime);
        }
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetText(string text)
    {
        talkText.text = text;
    }

    public void Enable()
    {
        background.fillAmount = 0;
        open = true;
    }

    public void Disable()
    {
        open = false;
        nameText.text = " ";
        talkText.text = " ";
    }
}

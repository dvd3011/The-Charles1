using UnityEngine;
using TMPro;
using System.Collections;

public class CollectFeedBack : MonoBehaviour
{
    public GameObject feedbackPrefab; // Arraste o prefab aqui
    public Canvas canvas; // Refer�ncia ao Canvas (arraste no Inspector)

    // Agora o m�todo n�o precisa mais de worldPosition � ignore ou remova se n�o usar
    public void ShowFeedback(string text, Color color)
    {
        GameObject feedbackObj = Instantiate(feedbackPrefab, canvas.transform);
        RectTransform rect = feedbackObj.GetComponent<RectTransform>();

        // Posiciona no centro da tela
        rect.anchoredPosition = Vector2.zero; // (0,0) com anchor central = meio da tela

        // Configura o texto
        TextMeshProUGUI tmpText = feedbackObj.GetComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.color = color;

        // Inicia a anima��o
        StartCoroutine(AnimateFeedback(feedbackObj));
    }

    private IEnumerator AnimateFeedback(GameObject obj)
    {
        TextMeshProUGUI tmpText = obj.GetComponent<TextMeshProUGUI>();
        Vector3 originalScale = obj.transform.localScale;
        Color originalColor = tmpText.color;

        // Fase 1: Surge com movimento (pop up + escala) � agora o movimento � relativo ao centro
        float durationIn = 2f;
        for (float t = 0; t < durationIn; t += Time.deltaTime)
        {
            float progress = t / durationIn;
            // Escala de 0 para 1.2 (efeito bounce)
            obj.transform.localScale = originalScale * (1f + Mathf.Sin(progress * Mathf.PI) * 0.2f);
            // Movimento para cima (relativo ao centro)
            obj.transform.Translate(Vector3.up * 25f * Time.deltaTime); // Ajuste se quiser mais/menos
            yield return null;
        }
        obj.transform.localScale = originalScale * 1.1f; // Pico final

        // Fase 2: Desaparece aos poucos (fade out + escala de volta)
        float durationOut = 0.3f;
        for (float t = 0; t < durationOut; t += Time.deltaTime)
        {
            float progress = t / durationOut;
            // Fade out da cor (alpha)
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            tmpText.color = newColor;
            // Escala de volta para 0
            obj.transform.localScale = originalScale * (1f - progress);
            // Movimento cont�nuo para cima (flutua para fora do centro)
            obj.transform.Translate(Vector3.up * 15f * Time.deltaTime);
            yield return null;
        }

        // Destroi o objeto
        Destroy(obj);
    }
}

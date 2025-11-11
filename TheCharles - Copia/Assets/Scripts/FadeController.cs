using UnityEngine;
using System.Collections;

public class FadeController : MonoBehaviour
{
    // Função pública para iniciar o fade
    // target: objeto que terá o fade
    // duration: tempo em segundos para o fade
    // fadeOut: true para fade out (desaparecer), false para fade in (aparecer)
    public void StartFade(GameObject target, float duration, bool fadeOut)
    {
        if (target == null)
        {
            Debug.LogError("FadeController.StartFade recebeu um target nulo!");
            return;
        }

        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            StartCoroutine(FadeSprite(sr, duration, fadeOut));
        }
        else
        {
            Debug.LogWarning($"Objeto {target.name} não possui SpriteRenderer para fazer fade.");
        }
    }

    private IEnumerator FadeSprite(SpriteRenderer sr, float duration, bool fadeOut)
    {
        float startAlpha = sr.color.a;
        float endAlpha = fadeOut ? 0f : 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
            yield return null;
        }

        // Garante que o alpha final seja exatamente o desejado
        Color finalColor = sr.color;
        finalColor.a = endAlpha;
        sr.color = finalColor;
    }
}

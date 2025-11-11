using UnityEngine;
using UnityEngine.UI;

public class Brush : MonoBehaviour
{
    public Image brushIndicator; // O indicador visual que terá seu sprite trocado

    public Sprite brush1Sprite;
    public Sprite brush2Sprite;
    public Sprite brush3Sprite;
    public Sprite brush4Sprite;

    // Esses métodos devem ser ligados aos botões no Inspector

    public void SelecionarBrush1()
    {
        brushIndicator.sprite = brush1Sprite;
    }

    public void SelecionarBrush2()
    {
        brushIndicator.sprite = brush2Sprite;
    }

    public void SelecionarBrush3()
    {
        brushIndicator.sprite = brush3Sprite;
    }

    public void SelecionarBrush4()
    {
        brushIndicator.sprite = brush4Sprite;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeAfeto : MonoBehaviour
{
    public Slider slider;
    
    public void AlterarBarraDeAfeto(float afeto)
    {
        slider.value = afeto;
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderText : MonoBehaviour
{
    public Slider slider;            // Sleep je slider hierin
    public TMP_Text valueText;       // Sleep je TMP Text hierin

    void Update()
    {
        valueText.text = slider.value.ToString("0");
    }
}

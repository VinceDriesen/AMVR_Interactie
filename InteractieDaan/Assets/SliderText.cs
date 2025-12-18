using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderText : MonoBehaviour
{
    public Slider slider;
    public TMP_Text valueText;

    void Update()
    {
        valueText.text = slider.value.ToString("0");
    }
}

using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient sliderGradient;
    public Image image;

    public void SetMaxHeal(float heal)
    {
        slider.maxValue = heal;
        slider.value = heal;

        image.color = sliderGradient.Evaluate(1f);
    }


    public void SetHeal(float heal)
    {

        slider.value = heal;
        image.color = sliderGradient.Evaluate(slider.normalizedValue);
    }
    void Awake()
    {
        if (slider == null)
            slider = GetComponentInChildren<Slider>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UITheme : MonoBehaviour
{
    [Header("Tema")]
    [SerializeField] private Color[] normalColor;
    [SerializeField] private Color[] highlightedColor;
    [SerializeField] private Color[] disabledColor;
    [SerializeField] private Sprite[] background;
    [SerializeField] private Color[] logoColor;
    [SerializeField] private Color[] sliderBackgroundColor;
    [SerializeField] private Color[] sliderOutlineColor;
    [SerializeField] private Color[] sliderFillColor;
    [Header("Assign components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image namaKelompokImage;
    [SerializeField] private Image logoImage;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Image[] SliderBackgrounds;
    [SerializeField] private Image[] SliderOutlines;
    [SerializeField] private Image[] SliderFills;
    private ColorBlock cb;

    public void UpdateTheme(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            cb = buttons[i].colors;
            cb.normalColor = normalColor[index];
            cb.highlightedColor = highlightedColor[index];
            cb.pressedColor = normalColor[index];
            cb.selectedColor = normalColor[index];
            cb.disabledColor = disabledColor[index];
            buttons[i].colors = cb;
        }
        for (int i = 0; i < SliderBackgrounds.Length; i++)
        {
            SliderBackgrounds[i].color = sliderBackgroundColor[index];
            SliderOutlines[i].color = sliderOutlineColor[index];
            SliderFills[i].color = sliderFillColor[index];
        }
        backgroundImage.sprite = background[index];
        namaKelompokImage.color = logoColor[index];
        logoImage.color = logoColor[index];
    }
}

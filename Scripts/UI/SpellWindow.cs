using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellWindow : MonoBehaviour
{
    public string currentSpellKey;
    public string currentSpellcode;
    [SerializeField] private Slider targetingSlider;
    [SerializeField] private Slider formingSlider;
    [SerializeField] private Slider actingSlider;
    public void Open(Vector3 position)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = position;
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void SetSpellcode(string spellKey)
    {
        currentSpellKey = spellKey;
        string spellcode = PlayerPrefs.GetString(currentSpellKey);
        targetingSlider.value = float.Parse(spellcode[0].ToString());
        formingSlider.value = float.Parse(spellcode[1].ToString());
        actingSlider.value = float.Parse(spellcode[2].ToString());
    }
    public void SaveSpellcode(string sliderName)
    {
        string tempcode = PlayerPrefs.GetString(currentSpellKey);
        switch (sliderName) 
        {
            case "TargetingSlider":
                PlayerPrefs.SetString(currentSpellKey, $"{targetingSlider.value}{tempcode[1]}{tempcode[2]}");
                break;
            case "FormingSlider":
                PlayerPrefs.SetString(currentSpellKey, $"{tempcode[0]}{formingSlider.value}{tempcode[2]}");
                break;
            case "ActingSlider":
                PlayerPrefs.SetString(currentSpellKey, $"{tempcode[0]}{tempcode[1]}{actingSlider.value}");
                break;
        }
    }
}

using UnityEngine;
using TMPro;

public class SetUiText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _textField;
    [SerializeField]
    private string _fixedText;

    public void OnSliderValueChanged(float numericValue)
    {
        // Format the numeric value with one decimal place
        string formattedValue = numericValue.ToString("F1");

        // Update the TextMeshPro text
        _textField.text = $"{_fixedText}: {formattedValue}";
    }
}

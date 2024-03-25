using TMPro;
using UnityEngine;

namespace VComponent.UI.Tools
{
    public class SliderValueFeedback : MonoBehaviour
    {
        [SerializeField] private bool _wholeNumber;
        [SerializeField] private TMP_Text _feedbackTxt;

        public void UpdateFeedbackText(float sliderValue)
        {
            var feedback = _wholeNumber ? Mathf.RoundToInt(sliderValue).ToString() : $"{sliderValue:00.00}";
            _feedbackTxt.text = feedback;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestClock
{
    public class ClockView : MonoBehaviour
    {
        [SerializeField] private ClockBehaviour _clockBehaviour;
        [SerializeField] private TMPro.TextMeshProUGUI _clockText;
        [SerializeField] private TMPro.TextMeshProUGUI _clockPartText;
        [SerializeField] private List<ArrowBehaviour> _arrows;

        [Header("DateInfo")]
        [SerializeField] private TMPro.TextMeshProUGUI _yearText;
        [SerializeField] private TMPro.TextMeshProUGUI _monthText;
        [SerializeField] private TMPro.TextMeshProUGUI _weekdayText;

        private void Update()
        {
            _clockText.text = ParseTimeSpan(_clockBehaviour.Date);
            _clockPartText.text = _clockBehaviour.Date.Hour < 12 ? "AM" : "PM";
            
            foreach (ArrowBehaviour arrow in _arrows)
            {
                arrow.UpdateRotation(_clockBehaviour.Date);
            }

            _yearText.text = _clockBehaviour.Date.ToString("yyy");
            _monthText.text = _clockBehaviour.Date.ToString("M");
            _weekdayText.text = _clockBehaviour.Date.ToString("ddd");

        }

        private string ParseTimeSpan(DateTime date)
        {
            string result = date.ToString("hh':'mm':'ss");
            return result;
        }
    }
}


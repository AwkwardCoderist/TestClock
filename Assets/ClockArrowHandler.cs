using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

namespace TestClock
{
    public class ClockArrowHandler : MonoBehaviour
    {
        [SerializeField] private ClockBehaviour _clockBehaviour;
        [SerializeField] private Button _changeTimeButton;
        [SerializeField] private List<ArrowBehaviour> _arrows;
        [SerializeField] private List<TMP_InputField> _inputFields;

        [Header("Tweener Info")]
        [SerializeField] private CanvasGroup _mainTimeGroup;
        [SerializeField] private CanvasGroup _inputFieldsTimeGroup;
        [SerializeField] private float _duration = 1;
        [SerializeField] private float _distance = 100;

        private TMPro.TMP_Text _buttonText;

        private bool _changeTimeMode;

        private DateTime _startArrowChangingTime;

        private Sequence _showInputFieldsSequence;

        public bool ChangeTimeMode
        {
            get => _changeTimeMode;
            set
            {
                _changeTimeMode = value;
                _inputFieldsTimeGroup.interactable = value;
                _clockBehaviour.enabled = !value;
                _buttonText.text = value ? "Resume" : "Pause";

                if (value)
                {
                    UpdateInputFieldsValues();
                    _showInputFieldsSequence.Rewind();
                    _showInputFieldsSequence.PlayForward();
                }
                else
                {
                    _showInputFieldsSequence.Complete();
                    _showInputFieldsSequence.PlayBackwards();
                }
            }
        }

        private void Start()
        {
            _changeTimeButton.onClick.AddListener(OnChangeTimeInput);

            _buttonText = _changeTimeButton.GetComponentInChildren<TMPro.TMP_Text>();

            foreach (ArrowBehaviour arrow in _arrows)
            {
                arrow.onDrag.AddListener(OnArrowDrag);
                arrow.onPointDown.AddListener(OnArrowPointDown);
            }

            foreach (TMP_InputField field in _inputFields)
            {
                TMP_InputField selectedField = field;
                field.onValueChanged.AddListener(x => OnInputFieldValueChange(selectedField, x));
            }

            ChangeTimeMode = ChangeTimeMode;

            _showInputFieldsSequence = DOTween.Sequence().SetAutoKill(false);
            _showInputFieldsSequence.Pause();

            _showInputFieldsSequence.Append(_mainTimeGroup.DOFade(0, _duration).SetAutoKill(false));
            _showInputFieldsSequence.Join(_mainTimeGroup.transform.DOLocalMoveX(_distance, _duration).SetAutoKill(false));
            _showInputFieldsSequence.Join(_inputFieldsTimeGroup.DOFade(1, _duration).SetAutoKill(false));
            _inputFieldsTimeGroup.transform.DOLocalMoveX(-_distance, 0);
            _showInputFieldsSequence.Join(_inputFieldsTimeGroup.transform.DOLocalMoveX(0, _duration).SetAutoKill(false));
        }

        private void UpdateInputFieldsValues()
        {
            for (int i = 0; i < _inputFields.Count; i++)
            {
                ArrowType type = _arrows[i].arrowType;
                string clockValue = string.Empty;

                switch (type)
                {
                    case ArrowType.Second:
                        clockValue = _clockBehaviour.Date.ToString("ss");
                        break;
                    case ArrowType.Minute:
                        clockValue = _clockBehaviour.Date.ToString("mm");
                        break;
                    case ArrowType.Hour:
                        clockValue = _clockBehaviour.Date.ToString("HH");
                        break;
                    default:
                        break;
                }

                _inputFields[i].text = clockValue;
            }
        }

        private void OnInputFieldValueChange(TMP_InputField field, string value)
        {
            int index = _inputFields.IndexOf(field);
            DateTime dateTime = _clockBehaviour.Date;
            int parsedValue = int.Parse(value);

            int newHour = dateTime.Hour;
            int newMin = dateTime.Minute;
            int newSec = dateTime.Second;

            switch (_arrows[index].arrowType)
            {
                case ArrowType.Second:

                    parsedValue = Mathf.Clamp(parsedValue, 0, 59);
                    newSec = parsedValue;
                    break;

                case ArrowType.Minute:

                    parsedValue = Mathf.Clamp(parsedValue, 0, 59);
                    newMin = parsedValue;
                    break;

                case ArrowType.Hour:

                    parsedValue = Mathf.Clamp(parsedValue, 0, 23);
                    newHour = parsedValue;
                    break;
                default:

                    break;

            }

            _clockBehaviour.Date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, newHour, newMin, newSec, DateTimeKind.Local);

        }

        private void OnChangeTimeInput()
        {
            ChangeTimeMode = !ChangeTimeMode;

            if (ChangeTimeMode)
            {
                UpdateInputFieldsValues();
            }
        }

        private void OnArrowPointDown(ArrowBehaviour arrow)
        {
            _startArrowChangingTime = _clockBehaviour.Date;
        }

        private void OnArrowDrag(ArrowBehaviour arrow)
        {
            if (ChangeTimeMode)
            {
                _clockBehaviour.Date = _startArrowChangingTime.Add(arrow.GetAddedTime());
                UpdateInputFieldsValues();
            }            
        }
    }
}


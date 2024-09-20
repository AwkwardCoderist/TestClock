using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TestClock
{
    public enum ArrowType
    {
        Second,
        Minute,
        Hour
    }

    [ExecuteInEditMode]
    public class ArrowBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        public ArrowType arrowType;

        [SerializeField] private float _t;

        public Vector2 _startedUp;
        public float addedAngle;
        private float _angleSegment;
        private Vector2 _prevDir;

        public UnityEvent<ArrowBehaviour> onDrag;
        public UnityEvent<ArrowBehaviour> onPointDown;

        public void OnPointerDown(PointerEventData eventData)
        {
            _startedUp = transform.up;
            _prevDir = transform.up;
            addedAngle = 0;
            onPointDown?.Invoke(this);
        }
        public void OnDrag(PointerEventData eventData)
        {            
            Vector2 cursorDir = eventData.position - (Vector2)transform.position;
            
            addedAngle += -Vector2.SignedAngle(_prevDir, cursorDir);

            _prevDir = cursorDir;

            onDrag?.Invoke(this);
        }

        [Range(0, 24)] public int testHours;

        private void Update()
        {/*
            _t = (float)testHours / 24;

            transform.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.back * 360 * 2, _t);*/
        }

        public void UpdateRotation(DateTime date)
        {
            _t = 0;
            float t_hourCoef = (arrowType == ArrowType.Hour ? 2 : 1);

            switch (arrowType)
            {
                case ArrowType.Second:
                    _t = date.Second / 60f;
                    break;
                case ArrowType.Minute:
                    _t = date.Minute / 60f;
                    break;
                case ArrowType.Hour:
                    _t = date.Hour / 24f;
                    break;
                default:
                    break;
            }


            transform.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.back * 360 * t_hourCoef, _t);
        }

        public TimeSpan GetAddedTime()
        {
            TimeSpan result;
            _angleSegment = addedAngle / 360;

            switch (arrowType)
            {
                case ArrowType.Second:
                    result = TimeSpan.FromSeconds(60 * _angleSegment);
                    break;
                case ArrowType.Minute:
                    result = TimeSpan.FromMinutes(60 * _angleSegment);
                    break;
                case ArrowType.Hour:
                    result = TimeSpan.FromHours(12 * _angleSegment);
                    break;
                default:
                    result = TimeSpan.Zero;
                    break;
            }

            Debug.Log(result.ToString());
            return result;

        }

    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace TestClock
{
    public class ClockBehaviour : MonoBehaviour
    {
        private const float HOURS_TO_AUTOSYNC = 1;

        [System.Serializable]
        private class ClockData
        {
            public string time;
            public string clocks;
        }

        private UnityWebRequest _clockRequest;

        public long _clockTime;
        private string _debugTimeString;
        private DateTime _dateTime;
        private float _lastSyncTime;


        public DateTime Date
        {
            get => _dateTime;
            set
            {
                _dateTime = value;

                _debugTimeString = _dateTime.ToString();
            }
        }

        public bool AutoSync { get; set; }



        void Start()
        {
            SyncTime();
        }
        public void SyncTime()
        {
            StartCoroutine(SyncTimeCoroutine());
        }

        private IEnumerator SyncTimeCoroutine()
        {
            _clockRequest = UnityWebRequest.Get("https://yandex.com/time/sync.json");

            yield return _clockRequest.SendWebRequest();

            if (_clockRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(_clockRequest.result);

                _dateTime = DateTime.Now;
            }
            else
            {
                ClockData data = JsonUtility.FromJson(_clockRequest.downloadHandler.text, typeof(ClockData)) as ClockData;
                _clockTime = long.Parse(data.time);

                _dateTime = DateTimeOffset.FromUnixTimeMilliseconds(_clockTime).UtcDateTime.ToLocalTime();

            }

            Debug.Log("initial: " + _dateTime.ToString());
        }

        void Update()
        {
            Date = Date.AddSeconds(Time.deltaTime);

            //TODO: update clock every hour

            if(AutoSync)
            {
                if (Time.time > _lastSyncTime + HOURS_TO_AUTOSYNC * 60 * 60)
                {
                    SyncTime();
                    _lastSyncTime = Time.time;
                }
            }
        }

    }
}


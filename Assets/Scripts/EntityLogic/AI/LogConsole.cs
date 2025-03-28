﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace EntityLogic.AI
{
    public class LogConsole : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _tmp;
        private ScrollRect _scrollRect;

        private static LogConsole _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _tmp.text = string.Empty;
        }

        public static void Log(string text)
        {
            if (_instance != null)
            {
                var consoleText = _instance._tmp.text;
                var length = consoleText.Length;
                if (length > 2000)
                {
                    _instance._tmp.text = consoleText.Substring(length - 1000) + text + Environment.NewLine;
                }
                else
                {
                    _instance._tmp.text += text + Environment.NewLine;
                }
                Canvas.ForceUpdateCanvases();
                _instance._scrollRect.verticalNormalizedPosition = 0.0f;
            }
        }
    }
}
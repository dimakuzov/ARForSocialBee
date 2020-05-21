using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SocialBeeAR
{
    /// <summary>
    /// For printing debug message on screen.
    /// </summary>
    public class DebugPanelController : MonoBehaviour
    {
        private Queue<string> messageQueue = new Queue<string>();
        private Text debugConsoleText;
        private string finalMessage;
        public int rowNumber = 1; //by default the raw number is 1
        private int counter = 0;

        private void Awake()
        {
            this.debugConsoleText = gameObject.GetComponent<Text>();
        }

        private void Start()
        {
        }

        public void PushMessage(string textValue)
        {
            if (textValue != null)
            {
                this.messageQueue.Enqueue("[" + ++counter + "] " + textValue);
                if (this.messageQueue.Count > this.rowNumber)
                {
                    this.messageQueue.Dequeue();
                }

                this.PrintMessage();
            }
        }

        private void PrintMessage()
        {
            this.finalMessage = "";
            int index = 0;
            foreach (string message in this.messageQueue)
            {
                this.finalMessage += (index == 0 ? "" : "\n") + message;
                index++;
            }
            this.debugConsoleText.text = this.finalMessage;
        }

    }

}







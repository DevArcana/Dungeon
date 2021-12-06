using UnityEngine;


namespace EntityLogic.AI
{
    public class LogConsole : MonoBehaviour
    {
        public static string myLog = "";
        bool _doShow = true;
        
        void Update() { if (Input.GetKeyDown(KeyCode.Space)) { _doShow = !_doShow; } }
        
        public static void Log(string logString)
        {
            // for onscreen...
            myLog = myLog + "\n" + logString;
            if (myLog.Length > 1000) { myLog = myLog.Substring(myLog.Length - 1000); }
        }
        
        void OnGUI()
        {
            if (!_doShow) { return; }
            GUI.TextArea(new Rect(10, Screen.height - 470, 540, 370), myLog);
        }
        // static string myLog = "";
        // private string output;
        // private string stack;
        //
        // void OnEnable()
        // {
        //     Application.logMessageReceived += Log;
        // }
        //
        // void OnDisable()
        // {
        //     Application.logMessageReceived -= Log;
        // }
        //
        // public void Log(string logString, string stackTrace, LogType type)
        // {
        //     output = logString;
        //     stack = stackTrace;
        //     myLog = output + "\n" + myLog;
        //     if (myLog.Length > 5000)
        //     {
        //         myLog = myLog.Substring(0, 4000);
        //     }
        // }
        //
        // void OnGUI()
        // {
        //     myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), myLog);
        // }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Logger", menuName = "Custom/Logger")]
public class Logger : ScriptableObject
{
	public enum LogMode {
		Details, Warnings, Errors
	}

	[SerializeField] private LogMode mode = LogMode.Warnings;
	[SerializeField] private Color outputColor = Color.black;


	#region Built-in loggers
	private static Logger _defaultLogger;
	public static Logger DefaultLogger {
		get {
			if(_defaultLogger == null) {
				_defaultLogger = CreateInstance<Logger>();
			}

			return _defaultLogger;
		}
	}
	#endregion


	public void Log(object message, Object context = null) {
		if(mode == LogMode.Details) {
			if(context == null) {
				Debug.Log(formatMessage(message));
			}
			else {
				Debug.Log(formatMessage(message), context);
			}
		}
	}

	public void Warn(object message, Object context = null) {
		if(mode != LogMode.Errors) {
			if(context == null) {
				Debug.LogWarning(formatMessage(message));
			}
			else {
				Debug.LogWarning(formatMessage(message), context);
			}
		}
	}

	public void Error(object message, Object context = null) {
		if(context == null) {
			Debug.LogError(formatMessage(message));
		}
		else {
			Debug.LogError(formatMessage(message), context);
		}
	}

	public string formatMessage(object message) {
		return
			"<color=#" + ColorUtility.ToHtmlStringRGB(outputColor) + ">"
			+ message.ToString() + "</color>";
	}
}

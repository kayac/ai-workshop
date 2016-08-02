// UnityCommandLineWindow 1.0.0
// @takashicompany

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace TakashiCompany.Unity.UniCmd
{

	public class UniCmdWindow : EditorWindow
	{

		private const string NAME_COMMAND_FIELD = "CommandField";
		private const int LOG_COUNT = 100;

		private Vector2 _logScrollPosition;

		private string _commandLine = "";
		
		private List<string> _logs = new List<string>();

		private bool _isForceMaxScroll; 

		private float _maxScrollY;

		[MenuItem("Window/UniCmd %u")]
		static void Init()
		{
			UniCmdWindow window = (UniCmdWindow)EditorWindow.GetWindow<UniCmdWindow>(); 
			window.Setup();
		}

		public void Setup()
		{ 
			Process.onLog += OnLog;
		} 

		private void OnLog(List<string> logs)
		{
			_logs = logs;
			Repaint();
		}

		void OnGUI()
		{
			if (Event.current.type == EventType.KeyUp && Event.current.isKey && Event.current.keyCode == KeyCode.Return)
			{
				Execute(); 
				_logScrollPosition = new Vector2(_logScrollPosition.x, float.MaxValue);
			}

			if (Event.current.type != EventType.KeyUp && Event.current.type != EventType.KeyDown)
			{
				_logScrollPosition = GUILayout.BeginScrollView(_logScrollPosition,GUILayout.ExpandHeight(true));

				var builder = new System.Text.StringBuilder();

				foreach (var log in _logs)
				{
					if (string.IsNullOrEmpty(log))continue;
					builder.Append(log);
					builder.Append("\n");
				}

				var content = new GUIContent(builder.ToString());

				GUI.skin.label.wordWrap = true;
				GUILayout.Label(content);
				GUILayout.EndScrollView();
			}

			GUI.SetNextControlName(NAME_COMMAND_FIELD);
			_commandLine = EditorGUILayout.TextField(_commandLine);

			GUI.FocusControl(NAME_COMMAND_FIELD); 
		}

		private void Execute()
		{
			if (string.IsNullOrEmpty(_commandLine)) return;

			Execute(_commandLine);

			_commandLine = "";
		}

		private string Execute(string rawCommand)
		{
			return Process.Execute(rawCommand);
		}
	}

	internal enum Command
	{ 
		help,	// help
		cr,		// create
		sr,		// search
		ac,		// add compnent
		htc, 	// hex to color
		sds,	// Scripting Define Symbols
		log,	// Log
		cl,		// clear 
		dl		// debug log
	}

	internal static class Process
	{
		public delegate void LogDelegate(List<string> logs);
		 
		public static event LogDelegate onLog;

		private static int maxLogCount = 100;

		private static readonly string[] STR_SPACE =  new string[]{" "};
		private static readonly string[] STR_COLON = new string[]{":"};

		private static List<string> _logs = new List<string>();

		internal static string Execute(this Command command, params string[] args)
		{
			switch(command)
			{
				case Command.help:  return Help(args);
				case Command.cr:    return Create(args);
				case Command.ac:    return AddComponent(args);
				case Command.htc:   return HexToColor(args);
				case Command.sds:   return SetScriptingDefineSymbols(args);
				case Command.log:   return Log(args);
				case Command.cl:    return Clear(args);
				case Command.dl:    return DebugLog(args);
			}

			return command.ToString() + " does not exist.";
		}

		internal static string Help(this Command command)
		{
			string message = "";

			switch(command)
			{
				case Command.cr: 
					message = "'cr'(Create GameObject):\n[cr MyObject BoxCollider Rigidbody]";
					break;

				case Command.ac:
					message = "'ac'(Add Component to Selected Hierarchy & Inspector GameObject):\n[ac SpriteRenderer CircleCollider]";
					break;

				case Command.htc:
					message = "'htc'(Hex to Color ex.00ff00 => 0,255,0):\n[htc ff0000]"; 
					break; 

				case Command.sds:
					message = "'sds'(Set/Add/Remove Scripting Define Symbols):\n[sds IS_DEBUG;USE_PAYMENT;] [sds IS_DEBUG USE_PAYMENT] [sds -a IS_DEBUG USE_PAYMENT] [sds -r IS_DEBUG USE_PAYMENT]";
					break; 

				case Command.cl:
					message = "'cl'(Clear Log):";
					break;

				case Command.help:
					message = "'help':\n Show help";
					break;

				case Command.dl:
					message = "'dl'(Call Debug.Log Function):\n [dl -l Log Message] options -l:Debug.Log, -w:Debug.LogWarning, -er:Debug.LogError, -ex:Debug.LogException";
					break;

				case Command.log:
					message = "'log'(Capture Log):\nwith the option. -s: start -q: quit";
					break;
				
				default:
					message = "'" + command.ToString() + "' does not exist.";
					break;
			}

			return message;
		}

		internal static string Clear(string[] args)
		{
			_logs.Clear();
			return "";
		}

		public static string Help (string[] args)
		{ 

			var cmds = new List<Command>();

			 
			if (args == null || args.Length == 0)
			{

				foreach (Command cmd in Enum.GetValues(typeof(Command)))
				{
					cmds.Add(cmd);
				}
			}
			else
			{ 
				foreach(var arg in args) 
				{
					foreach (Command cmd in Enum.GetValues(typeof(Command)))
					{
						if (arg == cmd.ToString())
						{
							cmds.Add(cmd);
						} 
					}
				}
			}

			if (cmds == null || cmds.Count == 0)
			{
				return "Invalid argments on Help.";
			}

			var builder = new System.Text.StringBuilder();
			 
			foreach (var cmd in cmds)
			{
				builder.Append(cmd.Help());
				builder.Append("\n\n");
			}

			return builder.ToString();
		}

		public static string Create (string[] args)
		{
			Transform parent = Selection.activeTransform;

			GameObject go = new GameObject();

			string name = "GameObject";
			bool alreadySetName = false;
			
			if (args != null)
			{
				foreach (string arg in args)
				{
					// with option
					if (!alreadySetName)
					{
						name = arg;
						alreadySetName = true;
					}
					else
					{

						Component component = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(go, "Assets/Scripts/UniCmdWindow.cs (258,29)", arg);
						if (component == null)
						{
							GameObject.DestroyImmediate(go);
							return "Component '" + arg + "' does not exist.";
						}
					}
				}
			}

			go.name = name;
			if (parent != null)
			{
				go.transform.parent = parent;
			}

			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one; 

			return "Succeeded in creating GameObject(" + name + ").";
		}

		public static string AddComponent (string[] args)
		{

			GameObject[] gameObjects = Selection.gameObjects;

			if (gameObjects == null) return "Please select GameObject."; 

			foreach (string arg in args)
			{
				foreach (GameObject target in gameObjects)
				{
					UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(target, "Assets/Scripts/UniCmdWindow.cs (291,6)", arg);
				}
			}

			string successFormat = "Succeeded in `AddComponent {0}.`";
			string joined = string.Join(", ", args);

			return string.Format(successFormat, joined);
		}


		public static string HexToColor (string[] args)
		{

			if (args == null || args.Length == 0) return "Error Hex To Color.";

			Color color = GetHexToColor(args[0]);
			return color.ToString();
		}

		private static Color GetHexToColor(string colorCode)
		{
			float red = Convert.ToUInt32(colorCode.Substring(0, 2), 16);
			float green = Convert.ToUInt32(colorCode.Substring(2, 2), 16);
			float blue = Convert.ToUInt32(colorCode.Substring(4, 2), 16);
			return new Color(red, green, blue);
		}

		public static string SetScriptingDefineSymbols (string[] args)
		{ 
			if (args == null || args.Length == 0) 
			{
				return string.Format(
					"ScriptingDefineSymbols\nAndroid:{0}\niOS:{1}",
					PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android),
					PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS)
				);

			}

			if (args.Length == 1)
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,args[0]);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS,args[0]);
			}
			else
			{
				string definesAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
				string definesIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);

				if (args[0] == "-a" || args[0] == "-r")
				{
					bool isAdd = args[0] == "-a";
					definesAndroid = GenerateScriptingDefineSymbols(definesAndroid, args, isAdd);
					definesIOS = GenerateScriptingDefineSymbols(definesIOS, args, isAdd);
				}
				else
				{
					definesAndroid = "";
					definesIOS = "";
					foreach (string arg in args)
					{
						definesIOS = definesAndroid += arg + ";";
					}
				}
				
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,definesAndroid);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS,definesIOS);
			}

			return string.Format(
				"Set ScriptingDefineSymbols\nAndroid:{0}\niOS:{1}",
				PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android),
				PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS)
			);

		}

		private static string GenerateScriptingDefineSymbols(
			string define, 
			string[] args,
			bool isAdd
		)
		{
			define += ";";


			for(int i = 1; i < args.Length; i++)
			{
				if (string.IsNullOrEmpty(args[i])) continue;

				var section = args[i] + ";";

				if (define.Contains(section))
				{
					if (isAdd)
					{
						// 何もしない
					}
					else
					{
						// 消す 
						define = define.Replace(section, "");
					}
				}
				else
				{
					if (isAdd)
					{
						define += section;
					}
				}
			} 

			return define;
		}

		public static string Log(string[] args)
		{
			if (args != null && 0 < args.Length)
			{
				if (args[0] == "-s" || args[0] == "-q")
				{
					bool isSet = args[0] == "-s"; 

					if (isSet)
					{
						Application.RegisterLogCallback(LogCallBack);
						return "Log Watching.";
					}
					else
					{
						Application.RegisterLogCallback(null); 
						return "Quit Log Watching.";
					}
				}
			}
			return "Please run `log` command with the option. -s: start -q: quit";
		}

		private static void LogCallBack(string logString, string stackTrace, LogType type)
		{
			string log = type.ToString() + ": " + logString;
			AddLog(log);
		} 

		private static string DebugLog(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				return "Invalid argments";
			}

			bool useOption = true;
			LogType logType = LogType.Log;

			switch (args[0])
			{
				case "-l":
					logType = LogType.Log;
					break;

				case "-w":
					logType = LogType.Warning;
					break;

				case "-er":
					logType = LogType.Error;
					break;

				case "-ex": 
					logType = LogType.Exception;
					break;

				default:
					useOption = false;
					break;
			}

			var builder = new System.Text.StringBuilder();
			 
			for (int i = useOption ? 1 : 0; i < args.Length; i++)
			{

				var message = args[i];
				builder.Append(message);
				builder.Append(" ");
			}
			 
			

			switch(logType)
			{
				case LogType.Log:
					Debug.Log(builder.ToString());
					break;

				case LogType.Warning: 
					Debug.LogWarning(builder.ToString());
					break;

				case LogType.Error: 
					Debug.LogError(builder.ToString());
					break;

				case LogType.Exception: 
					Debug.LogException(new Exception(builder.ToString()));
					break;
			}

			return "Debug-" + logType.ToString() + ":" + builder.ToString();
		}


		#region Core Logic
		public static string Execute(string rawCommand)
		{
			if (string.IsNullOrEmpty(rawCommand)) return "Command is empty.";

			string commandStr = null;
			string[] args = null;

			foreach (string cmdName in System.Enum.GetNames(typeof(Command)))
			{
				string cmdType = cmdName + STR_SPACE[0];
				if (rawCommand.StartsWith(cmdType))
				{
					commandStr = cmdName;
					string argStr = rawCommand.Substring(cmdType.Length);
					if(0 < argStr.Length) args = argStr.Split(STR_SPACE,System.StringSplitOptions.None);
				}
				else if (rawCommand == cmdName)
				{
					commandStr = rawCommand;
				}
			}

			Command command = (Command)Enum.Parse(typeof(Command), commandStr);

			var result = command.Execute(args);
			AddLog(result);
			return result;
		}

		private static KeyValuePair<string,string> GetArgment (string arg)
		{
			if (arg.Contains(STR_COLON[0]))
			{
				string[] args = arg.Split(STR_COLON,StringSplitOptions.None);
				if (args.Length == 2)
				{
					if (args[0].Length == 0 || args[1].Length == 0)
					{
						return new KeyValuePair<string, string>();
					}
					return new KeyValuePair<string, string>(args[0],args[1]);
				}
			}
			return new KeyValuePair<string, string>();
		}

		private static List<string> GetMethodNames(string className)
		{
			var type = GetType(className);

			if (type == null) return null;

			var methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

			var names = new List<string>(); 

			foreach (var methodInfo in methodInfos)
			{
				names.Add(methodInfo.Name);
			}

			return names;
		}

		private static List<string> GetFieldNames (string className)
		{
			var type = GetType(className);

			if (type == null) return null;

			FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic); 
			PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

			List<string> names = new List<string>();

			foreach (var fieldInfo in fieldInfos)
			{
				names.Add(fieldInfo.Name);
			}

			foreach (var propertyInfo in propertyInfos)
			{
				names.Add(propertyInfo.Name);
			}

			return names;
		}

		private static Type GetType(string className)
		{
			if (string.IsNullOrEmpty(className)) return null;

			Type type = Type.GetType(className);

			if (type == null)
			{
				string formatStr = "{0}, Assembly-CSharp";
				type = Type.GetType(string.Format(formatStr,className));
			}

			if (type == null)
			{
				string formatStr = "UnityEngine.{0}, UnityEngine";
				type = Type.GetType(string.Format(formatStr,className));
			} 

			if (type == null)
			{
				string formatStr = "UnityEditor.{0}, UnityEditor";
				type = Type.GetType(string.Format(formatStr,className));
			}

			return type;
		}

		private static void AddLog(string log)
		{
			_logs.Add(log);
			if (maxLogCount < _logs.Count)
			{
				_logs.RemoveAt(0);
			}

			if (onLog != null)
			{
				onLog(_logs);
			}
		}

		#endregion
	} 
}

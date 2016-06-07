// @takashicompany (takashicompany.com)
// CustomHierarchy var1.0.1

using UnityEditor;
using UnityEngine;

using System;
using System.Reflection;
using System.Collections.Generic;

/// <summary>
/// HierarchyViewを拡張するクラス
/// 専用のウィンドウ(Shift + 4)に「HogeClass.FugaVal」といった指定をすると
/// HogeClassのFugaValの値をHierarchyに表示する
/// </summary>
[InitializeOnLoad]
public class CustomHierarchy
{
	private const string KEY_CLASS_NAME = "tc_customhierarchy_class_name";
	private const string KEY_FIELD_NAME = "tc_customhierarchu_field_name";

	public static string className
	{
		get
		{
			return EditorPrefs.GetString(KEY_CLASS_NAME);
		}
		set
		{
			EditorPrefs.SetString(KEY_CLASS_NAME, value);
		}
	}


	public static string fieldName
	{
		get
		{
			return EditorPrefs.GetString(KEY_FIELD_NAME);
		}
		set
		{
			EditorPrefs.SetString(KEY_FIELD_NAME, value);
		}
	}

	static CustomHierarchy ()
	{
		EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
		EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;
	}

	public static void Repaint ()
	{
		EditorApplication.RepaintHierarchyWindow();
	}

	static void HierarchyWindowChanged ()
	{
		Repaint();
	}

	static void HierarchyWindowItemOnGUI (int instanceID, Rect selectionRect)
	{

		if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(fieldName))
		{
			return;
		}

		GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

		if(gameObject != null)
		{

			Component component = gameObject.GetComponent(className);

			if (component == null)
			{
				return;
			}

			Type type = component.GetType();

			FieldInfo fieldInfo = type.GetField(fieldName,BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

			object targetParam;

			if (fieldInfo != null)
			{
				targetParam = fieldInfo.GetValue(component);
			}
			else
			{
				PropertyInfo propertyInfo = type.GetProperty(fieldName,BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

				if (propertyInfo == null)
				{
					return;
				}
				else
				{
					targetParam = propertyInfo.GetValue(component,null);
				}

			}

			if (targetParam == null)
			{
				return;
			}

			string fieldStr = string.Format("[{0}:{1}]",fieldName,targetParam.ToString());



			Rect labelRect = new Rect(
				EditorStyles.label.CalcSize(new GUIContent(gameObject.name)).x + selectionRect.x,
				selectionRect.y,
				EditorStyles.label.CalcSize(new GUIContent(fieldStr)).x,
				selectionRect.height
				);

			if (targetParam is GameObject)
			{
				GameObject targetObj = (GameObject) targetParam;
				if (GUI.Button(labelRect,targetObj.name))
				{
					Selection.activeGameObject = targetObj;
				}
			}
			if (targetParam is Component)
			{
				Component targetComponent = (Component) targetParam;
				if (GUI.Button(labelRect,targetComponent.gameObject.name))
				{
					Selection.activeGameObject = targetComponent.gameObject;
				}
			}
			else
			{
				GUI.Label(labelRect,fieldStr);
			}
		}
	}

}



public class CustomHierarchyWindow : EditorWindow{

	private string _userInput;

	private string _className;
	private string _fieldName;

	private int _index;

	[MenuItem("Window/CustomHierarchy #4")]
	static void Init ()
	{
		CustomHierarchyWindow window = (CustomHierarchyWindow)EditorWindow.GetWindow<CustomHierarchyWindow>();
	}

	void OnGUI ()
	{
		if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
		{
			SetClassAndField ();
		}
		_userInput = EditorGUILayout.TextField(_userInput);

		Parse();

		List<string> list = GetFieldNameList();

		if ((list != null && 0 < list.Count) && string.IsNullOrEmpty(_fieldName))
		{
			int myIndex = _index;

			_index = EditorGUILayout.Popup(_index,list.ToArray());

			if (_index != myIndex)
			{
				_fieldName = list[_index];
				UpdateInput();
				SetClassAndField();
			}
		}
		else
		{
			_index = -1;
		}
	}

	void SetClassAndField()
	{
		CustomHierarchy.className = _className;
		CustomHierarchy.fieldName = _fieldName;
		CustomHierarchy.Repaint();
	}

	private List<string> GetFieldNameList ()
	{
		if (string.IsNullOrEmpty(_className)) return null;

		Type type = Type.GetType(_className);

		if (type == null)
		{
			string formatStr = "{0}, Assembly-CSharp";
			type = Type.GetType(string.Format(formatStr,_className));
		}

		if (type == null)
		{
			string formatStr = "UnityEngine.{0}, UnityEngine";
			type = Type.GetType(string.Format(formatStr,_className));
		}

		if (type == null) return null;


		FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
		PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

		List<string> nameList = new List<string>();

		foreach (var fieldInfo in fieldInfos)
		{
			nameList.Add(fieldInfo.Name);
		}

		foreach (var propertyInfo in propertyInfos)
		{
			nameList.Add(propertyInfo.Name);
		}

		return nameList;
	}

	private void Parse ()
	{
		if (string.IsNullOrEmpty(_userInput)) return;

		string[] splitChars = new string[]{"."};

		string[] rawSplit = _userInput.Split(splitChars, System.StringSplitOptions.None);
		if (rawSplit == null || rawSplit.Length != 2)
		{
			_className = _userInput;
			_fieldName = null;
			return;
		}

		_className = rawSplit[0];
		_fieldName = rawSplit[1];
	}

	private void UpdateInput ()
	{
		_userInput = string.Format("{0}.{1}",_className,_fieldName);
	}

}


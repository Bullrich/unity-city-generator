using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

//Autor: Iván Ramos
[CustomEditor(typeof(CityGenerator.BuildCity))]
public class CityBuilderCustomEditor : Editor
{
    CityGenerator.BuildCity _target;
    /*
    bool _inputGenerate = false;
    bool _inputDraw = false;
    bool _inputSetActive = false;
    */

    private void OnEnable()
    {
        _target = (CityGenerator.BuildCity)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("City Generator", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("by Javier Bullrich, Juan Cruz Araujo, Iván Ramos - © 2017", EditorStyles.miniLabel);
        EditorGUILayout.Space();
        /*
        ShowInput();
        EditorGUILayout.Space();
        */
        SetGameObjects();
        EditorGUILayout.Space();
        SetCityParams();
        EditorGUILayout.Space();
        ConfigureNeighborhoods();
        Repaint();
    }

    /*
     * 26/10/17: Por si nos sirve para el input. Lo hice con toggles pero capaz con botones sería mejor.
     * 
    void ShowInput()
    {
        EditorGUILayout.LabelField("Controls:", EditorStyles.boldLabel);
        _inputGenerate = EditorGUILayout.Toggle("Generate ('L')", _inputGenerate);
        if (_inputGenerate)
        {
            _target.Input_Generate();
            _inputGenerate = false;
        }
        _inputDraw = EditorGUILayout.Toggle("Draw ('M')", _inputDraw);
        if (_inputDraw)
        {
            _target.Input_Draw();
            _inputDraw = false;
        }
        _inputSetActive = EditorGUILayout.Toggle("Set Active ('A')", _inputSetActive);
        if (_inputSetActive)
        {
            _target.Input_SetActive();
            _inputSetActive = false;
        }
    }
    */

    void SetGameObjects()
    {
        EditorGUILayout.LabelField("Set GameObjects for...", EditorStyles.boldLabel);
        _target.xStreets = (GameObject)EditorGUILayout.ObjectField("...Horizontal Streets", _target.xStreets, typeof(GameObject), false);
        _target.zStreets = (GameObject)EditorGUILayout.ObjectField("...Vertical Streets", _target.zStreets, typeof(GameObject), false);
        _target.crossRoad = (GameObject)EditorGUILayout.ObjectField("...Cross-roads", _target.crossRoad, typeof(GameObject), false);
        if (_target.xStreets == null || _target.zStreets == null || _target.crossRoad == null) EditorGUILayout.HelpBox("You must set all three GameObjects.", MessageType.Error);
    }

    void SetCityParams()
    {
        EditorGUILayout.LabelField("Set city size:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _target.mapWidth = EditorGUILayout.IntField("Width", _target.mapWidth);
        _target.mapHeight = EditorGUILayout.IntField("Height", _target.mapHeight);
        EditorGUILayout.EndHorizontal();
		if (_target.mapWidth < 0) EditorGUILayout.HelpBox("Map Width must be a positive value.", MessageType.Error);
        else if (_target.mapWidth == 0) EditorGUILayout.HelpBox("Map Width must not be zero. A city of width 0 is literally nothing, don't get metaphysical with me!", MessageType.Error);

        if (_target.mapHeight < 0) EditorGUILayout.HelpBox("Map Height must be a positive value.", MessageType.Error);
        else if (_target.mapWidth == 0) EditorGUILayout.HelpBox("Map Height must not be zero. A city of height 0 is literally nothing, don't get metaphysical with me!", MessageType.Error);
    }

    void ConfigureNeighborhoods()
    {
        EditorGUILayout.LabelField("Configure Neighborhoods", EditorStyles.boldLabel);
        if (_target.neighborhoods.Length == 0) EditorGUILayout.HelpBox("No Nieghborhoods set!", MessageType.Error);
        for (int i = 0; i < _target.neighborhoods.Length; i++)
        {
            EditorGUILayout.LabelField("Neighborhood #" + (i+1));
            _target.neighborhoods[i].ChanceToAppear = (int)EditorGUILayout.Slider("Chance To Appear", _target.neighborhoods[i].ChanceToAppear, 0, 100);
            if (_target.neighborhoods[i].ChanceToAppear == 0) EditorGUILayout.HelpBox("Keeping Chance to Appear at 0 will make this Neighborhood never appear.", MessageType.Warning);
            for (int j = 0; j < _target.neighborhoods[i].buildings.Length; j++)
            {
                _target.neighborhoods[i].buildings[j] = (GameObject)EditorGUILayout.ObjectField("Building #" + (i + 1), _target.neighborhoods[i].buildings[j], typeof(GameObject), false);
            }
            if (_target.neighborhoods[i].buildings.Length == 0) EditorGUILayout.HelpBox("No buildings in this Neighborhood!", MessageType.Error);
			if (GUILayout.Button ("Add Building"))
			{
				_target.neighborhoods [i].buildings [_target.neighborhoods [i].buildings.Length] = new GameObject ();
			}
			/*
            Rect bBtn = EditorGUILayout.BeginHorizontal("Add Building");
            if (GUI.Button(bBtn, GUIContent.none)) _target.neighborhoods[i].buildings[_target.neighborhoods[i].buildings.Length] = new GameObject();
            EditorGUILayout.EndHorizontal();
            */
            EditorGUILayout.Space();
        }
		if (GUILayout.Button ("Add Neighborhood"))
		{
			_target.neighborhoods [_target.neighborhoods.Length] = new CityGenerator.Neighborhood ();
		}
        /*
        Rect nBtn = EditorGUILayout.BeginHorizontal("Add Neighbor");
        if (GUI.Button(nBtn, GUIContent.none)) _target.neighborhoods[_target.neighborhoods.Length] = new CityGenerator.Neighborhood();
        EditorGUILayout.EndHorizontal();
        */
    }
}

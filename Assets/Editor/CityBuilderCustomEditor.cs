using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CityGenerator
{
    //Autor: Iván Ramos
    [CustomEditor(typeof(CityGenerator.BuildCity))]
    public class CityBuilderCustomEditor : Editor
    {
        BuildCity _target;
        Vector2 widthHeight;
        Dictionary<int, bool> foldoutsNeighborhoods = new Dictionary<int, bool>();

        private void OnEnable()
        {
            _target = (CityGenerator.BuildCity)target;
            widthHeight.Set(_target.mapWidth, _target.mapHeight);
            for (int t = 0; t < 50; t++)
            {
                foldoutsNeighborhoods[t] = false;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("City Generator", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("by Javier Bullrich, Juan Cruz Araujo, Iván Ramos - © 2017", EditorStyles.miniLabel);
            EditorGUILayout.Space();
            //EditorGUILayout.HelpBox("Controls = L: Generate | M: Draw | A: Toggle Active", MessageType.None);
            if(GUILayout.Button("Generate City"))
                _target.Regenerate();
            EditorGUILayout.Space();
            SetGameObjects();
            EditorGUILayout.Space();
            SetSeedParams();
            EditorGUILayout.Space();
            SetCityParams();
            EditorGUILayout.Space();
            ConfigureNeighborhoods();
            Repaint();
        }

        void SetGameObjects()
        {
            EditorGUILayout.LabelField("Set GameObjects for...", EditorStyles.boldLabel);
            _target.xStreets = (GameObject)EditorGUILayout.ObjectField("...Horizontal Streets", _target.xStreets, typeof(GameObject), false);
            _target.zStreets = (GameObject)EditorGUILayout.ObjectField("...Vertical Streets", _target.zStreets, typeof(GameObject), false);
            _target.crossRoad = (GameObject)EditorGUILayout.ObjectField("...Cross-roads", _target.crossRoad, typeof(GameObject), false);
            if (_target.xStreets == null || _target.zStreets == null || _target.crossRoad == null) EditorGUILayout.HelpBox("You must set all three GameObjects.", MessageType.Error);
        }

        void SetSeedParams()
        {
            EditorGUILayout.LabelField("Set Seed", EditorStyles.boldLabel);
            _target.seed = EditorGUILayout.IntField("Value", _target.seed);
            if (_target.seed < 0 || _target.seed > 400)
            {
                EditorGUILayout.HelpBox("Seed value must be between 0 and 400.", MessageType.Error);
            }
            else if (_target.seed == 0)
            {
                _target.randomSeed = true;
                EditorGUILayout.HelpBox("Seed will generate randomly.", MessageType.Info);
            }
            else
            {
                _target.randomSeed = false;
            }
        }

        void SetCityParams()
        {
            EditorGUILayout.LabelField("Set city size:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            widthHeight = EditorGUILayout.Vector2Field("Width | Height", widthHeight);
            _target.mapWidth = (int)widthHeight.x;
            _target.mapHeight = (int)widthHeight.y;
            EditorGUILayout.EndHorizontal();
            if (_target.mapWidth < 0) EditorGUILayout.HelpBox("Map Width must be a positive value.", MessageType.Error);
            else if (_target.mapWidth == 0) EditorGUILayout.HelpBox("Map Width must not be zero. A city of width 0 is literally nothing, don't get metaphysical with me!", MessageType.Error);

            if (_target.mapHeight < 0) EditorGUILayout.HelpBox("Map Height must be a positive value.", MessageType.Error);
            else if (_target.mapHeight == 0) EditorGUILayout.HelpBox("Map Height must not be zero. A city of height 0 is literally nothing, don't get metaphysical with me!", MessageType.Error);
        }

        void ConfigureNeighborhoods()
        {
            EditorGUILayout.LabelField("Configure Neighborhoods", EditorStyles.boldLabel);
            if (_target.neighborhoods.Count == 0) EditorGUILayout.HelpBox("No Nieghborhoods set!", MessageType.Error);
            for (int i = 0; i < _target.neighborhoods.Count; i++)
            {
                foldoutsNeighborhoods[i] = EditorGUILayout.Foldout(foldoutsNeighborhoods[i], "Neighborhood #" + (i + 1));
                if (foldoutsNeighborhoods[i])
                {
                    _target.neighborhoods[i].ChanceToAppear = (int)EditorGUILayout.Slider("Chance To Appear", _target.neighborhoods[i].ChanceToAppear, 0, 100);
                    if (_target.neighborhoods[i].ChanceToAppear == 0) EditorGUILayout.HelpBox("Keeping Chance to Appear at 0 will make this Neighborhood never appear.", MessageType.Warning);
                    if (_target.neighborhoods[i].buildings == null || _target.neighborhoods[i].buildings.Count == 0)
                    {
                        EditorGUILayout.HelpBox("No buildings in this Neighborhood!", MessageType.Error);
                    }
                    else
                    {
                        for (int j = 0; j < _target.neighborhoods[i].buildings.Count; j++)
                        {
                            _target.neighborhoods[i].buildings[j] = (GameObject)EditorGUILayout.ObjectField("Building #" + (j + 1), _target.neighborhoods[i].buildings[j], typeof(GameObject), false);
                        }
                    }
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Add Building"))
                    {
                        _target.neighborhoods[i].buildings.Add(new GameObject("TEMPBUILDING"));
                    }
                    if (GUILayout.Button("Remove Last Building"))
                    {
                        int lastIndexB = _target.neighborhoods[i].buildings.Count - 1;
                        _target.neighborhoods[i].buildings.RemoveAt(lastIndexB);
                    }
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Neighborhood"))
            {
                _target.neighborhoods.Add(new Neighborhood());
            }
            if (GUILayout.Button("Remove Last Neighborhood"))
            {
                int lastIndexN = _target.neighborhoods.Count - 1;
                _target.neighborhoods.RemoveAt(lastIndexN);
            }
            EditorGUILayout.Space();
        }
    }
}
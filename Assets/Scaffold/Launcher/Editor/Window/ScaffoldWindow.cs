﻿using UnityEditor;
using UnityEngine;
using Scaffold.Launcher.PackageHandler;
using Scaffold.Launcher;
using System.Collections.Generic;
using System;
using System.Linq;
using Scaffold.Launcher.Objects;

namespace Scaffold.Launcher.Editor
{
    internal class ScaffoldWindow : EditorWindow
    {
        [MenuItem("Scaffold/Open Launcher")]
        public static void OpenLauncher()
        {
            Window.Show();
            Window.minSize = _minWindowSize;
        }

        private static Vector2 CurrentWindowSize => Window.position.size;
        private static Vector2 _minWindowSize = new Vector2(400, 400);

        private static ScaffoldWindow Window
        {
            get
            {
                if (_window == null)
                {
                    _window = (ScaffoldWindow)EditorWindow.GetWindow(typeof(ScaffoldWindow));
                }
                return _window;
            }
        }
        private static ScaffoldWindow _window;
        private static ScaffoldManager Scaffold
        {
            get
            {
                if (_scaffold == null) _scaffold = new ScaffoldManager();
                return _scaffold;
            }
        }
        private static ScaffoldManager _scaffold;

        private Vector2 _scrollView;

        private WindowTab CurrentTab
        {
            get
            {
                if(_currentTab == null)
                {
                    _currentTab = Tabs[0];
                }
                return _currentTab;
            }
            set
            {
                _currentTab = value;
            }
        }
        private WindowTab _currentTab;
        private List<WindowTab> Tabs
        {
            get
            {
                if(_tabs == null)
                {
                    _tabs = GetAllTabs();
                }
                return _tabs;
            }
        }
        private List<WindowTab> _tabs;

        private void OnGUI()
        {
            ScaffoldModule launcher = Scaffold.GetLauncher();

            _scrollView = EditorGUILayout.BeginScrollView(_scrollView, GUIStyle.none, GUIStyle.none);
            {
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Box("Scaffold", EditorStyles.HeaderBox);
                    EditorGUILayout.LabelField(launcher.InstalledVersion, EditorStyles.CenterLabel);
                    EditorGUILayout.BeginHorizontal();
                    {
                        foreach(WindowTab tab in Tabs)
                        {
                            string tabName = tab.TabName;
                            bool selected = CurrentTab == tab;
                            EditorGUI.BeginDisabledGroup(selected);
                            {
                                if (GUILayout.Button(tabName))
                                {
                                    CurrentTab = tab;
                                }
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5);
                    CurrentTab.Draw(CurrentWindowSize, Scaffold);
                    EditorGUILayout.Space(5);
                    DrawFooter();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawFooter()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Website"))
                {
                    //TODO: Website Link
                }
                if (GUILayout.Button("GitHub"))
                {
                    //TODO: GIT Link
                }
                if (GUILayout.Button("License"))
                {
                    //TODO: License Link
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private List<Type> GetAllTabTypes()
        {
            Type type = typeof(WindowTab);
            return GetType().Assembly.GetTypes()
                                     .Where(t => t.IsSubclassOf(type))
                                     .Where(t => !t.IsAbstract)
                                     .OrderBy(t => (t.GetCustomAttributes(typeof(TabOrder), true).FirstOrDefault() as TabOrder).Order)
                                     .ToList();
        }

        private List<WindowTab> GetAllTabs()
        {
            List<WindowTab> tabs = new List<WindowTab>();
            List<Type> types = GetAllTabTypes();
            foreach(Type type in types)
            {
                object[] parameters = new object[] { CurrentWindowSize, Scaffold };
                WindowTab tab = Activator.CreateInstance(type, parameters) as WindowTab;
                tabs.Add(tab);
            }
            return tabs;
        }
    }
}
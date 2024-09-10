using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Zuy.Workspace.Base
{
    /// <summary>
    /// Interface for CSV parsers.
    /// </summary>
    public interface ICSVParser
    {
        /// <summary>
        /// Parses the given lines from a CSV file.
        /// </summary>
        /// <param name="lines">The lines to parse.</param>
        void Parse(string[] lines);

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <returns>The name of the parser.</returns>
        string GetName();
    }

    /// <summary>
    /// Example implementation of a CSV parser.
    /// </summary>
    public class ExampleCSVParser : ICSVParser
    {
        /// <summary>
        /// Parses the given lines from a CSV file.
        /// </summary>
        /// <param name="lines">The lines to parse.</param>
        public void Parse(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                Debug.Log($"Line {i}: {lines[i]}");
            }
        }

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <returns>The name of the parser.</returns>
        public string GetName()
        {
            return "Example Parser";
        }
    }

    /// <summary>
    /// Editor window for parsing CSV files into ScriptableObjects.
    /// </summary>
    public class CSVEditorWindow : EditorWindow
    {
        /// <summary>
        /// Path to the CSV file.
        /// </summary>
        protected string m_CSVPath;

        /// <summary>
        /// Name of the CSV file.
        /// </summary>
        protected string m_CSVFileName;

        /// <summary>
        /// Lines read from the CSV file.
        /// </summary>
        protected string[] m_Lines;

        /// <summary>
        /// Index of the selected parse mode.
        /// </summary>
        protected int m_SelectedParseMode = 0;

        /// <summary>
        /// List of available CSV parsers.
        /// </summary>
        private List<ICSVParser> m_Parsers = new List<ICSVParser>();

        /// <summary>
        /// Shows the CSV parser window.
        /// </summary>
        [MenuItem("Tools/Parse CSV to ScriptableObject")]
        protected static void ShowWindow()
        {
            GetWindow<CSVEditorWindow>("CSV Parser");
        }

        /// <summary>
        /// Initializes the editor window.
        /// </summary>
        protected virtual void OnEnable()
        {
            // Add the example parser to the list of parsers
            m_Parsers.Add(new ExampleCSVParser());
        }

        /// <summary>
        /// Draws the GUI for the editor window.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("CSV to ScriptableObject", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUI.backgroundColor = Color.cyan;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Select CSV File", GUILayout.Width(150), GUILayout.Height(30)))
            {
                m_CSVPath = EditorUtility.OpenFilePanel("Select CSV file", "", "csv");
                if (!string.IsNullOrEmpty(m_CSVPath))
                {
                    m_CSVFileName = Path.GetFileName(m_CSVPath);
                    LoadCSV();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            if (!string.IsNullOrEmpty(m_CSVFileName))
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label($"Selected CSV File: {m_CSVFileName}");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); 
            GUILayout.Label("Select Parse Mode:");
            string[] parseModeNames = m_Parsers.ConvertAll(parser => parser.GetName()).ToArray();
            m_SelectedParseMode = EditorGUILayout.Popup(m_SelectedParseMode, parseModeNames);
            GUILayout.FlexibleSpace(); 
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            GUI.backgroundColor = Color.green;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); 
            if (GUILayout.Button("Parse CSV to ScriptableObject", GUILayout.Width(200), GUILayout.Height(30)))
            {
                ParseCSV();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// Loads the CSV file from the specified path.
        /// </summary>
        private void LoadCSV()
        {
            try
            {
                using (var fileStream = new FileStream(m_CSVPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fileStream))
                {
                    var fileContent = reader.ReadToEnd();
                    m_Lines = fileContent.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                }
            }
            catch (IOException ex)
            {
                Debug.LogError($"Failed to load CSV file: {ex.Message}");
            }
        }

        /// <summary>
        /// Parses the loaded CSV file using the selected parser.
        /// </summary>
        private void ParseCSV()
        {
            if (m_Lines == null || m_Lines.Length < 2)
            {
                Debug.LogError("CSV file is empty or not loaded properly.");
                return;
            }

            if (m_SelectedParseMode >= 0 && m_SelectedParseMode < m_Parsers.Count)
            {
                EditorUtility.DisplayProgressBar("Parsing CSV", "Parsing in progress...", 0.0f);
                try
                {
                    m_Parsers[m_SelectedParseMode].Parse(m_Lines);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }
            else
            {
                Debug.LogError("Invalid parse mode selected.");
            }
        }
    }
}

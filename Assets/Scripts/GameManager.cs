using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<DataItem> dataItems;

    public Texture2D Ramp;

    public List<TMP_InputField> _allInputFields;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        string downloadsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "/Downloads/";
        FileBrowser.SetFilters( true, new FileBrowser.Filter( "CSV files", ".csv"));
        FileBrowser.SetDefaultFilter( ".csv" );
        FileBrowser.AddQuickLink( "Users", "C:\\Users", null );
        FileBrowser.AddQuickLink( "Downloads", downloadsPath, null );

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
        {
            UpdateKeywordUsage();
        }
    }

    public void UpdateKeywordUsage()
    {
        foreach (TMP_InputField i in _allInputFields)
        {
            foreach (DataItem dataItem in dataItems)
            {
                dataItem.NumberOfTimesUsed = 0;
            }
        }

        foreach (TMP_InputField i in _allInputFields)
        {
            string text = i.text.RemoveHtmlTags();
            foreach (DataItem dataItem in dataItems)
            {
                dataItem.NumberOfTimesUsed += CountStringOccurrences(text, dataItem.Keyword);
            }
        }

        
        foreach (TMP_InputField i in _allInputFields)
        {
            int caretPos = i.caretPosition;
            string text = i.text.RemoveHtmlTags();
            foreach (DataItem dataItem in dataItems)
            {
                text = SetTextColorWhereFound(i, text, dataItem.Keyword, dataItem.KeywordColor);
            }

            //i.caretPosition = i.text.Length - 1;
            i.caretPosition = caretPos;
        }
        
        UIManager.Instance.UpdateKeywordList(dataItems);
    }

    private int CountStringOccurrences(string input, string searchString)
    {
        string pattern = "\\b" + Regex.Escape(searchString) + "\\b";
        Regex regex = new Regex(pattern);
        return regex.Matches(input).Count;
    }

    public string SetTextColorWhereFound(TMP_InputField textMeshPro, string input, string searchString, Color textColor)
    {
        string replacementText = "<color=#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + searchString + "</color>";
        string escapedSearchText = Regex.Escape(searchString);
        string pattern = "\\b" + escapedSearchText + "\\b";

        string formattedText = Regex.Replace(input, pattern, match => "<color=#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + match.Value + "</color>", RegexOptions.IgnoreCase);

        //string formattedText = Regex.Replace(input, Regex.Escape(searchString), replacementText);
        //string formattedText = input.Replace(searchString, replacementText);

        textMeshPro.text = (formattedText);
        //print(formattedText);
        return formattedText;
    }


    public void OpenFileBrowser()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    private IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: both, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load CSV File", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            // for( int i = 0; i < FileBrowser.Result.Length; i++ )
            //     Debug.Log( FileBrowser.Result[i] );

            string csvText = System.IO.File.ReadAllText(FileBrowser.Result[0]);
            GetDataFromPath(csvText);

            // Read the bytes of the first file via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            //byte[] bytes = FileBrowserHelpers.ReadBytesFromFile( FileBrowser.Result[0] );

            // Or, copy the first file to persistentDataPath
            // string destinationPath = Path.Combine( Application.persistentDataPath, FileBrowserHelpers.GetFilename( FileBrowser.Result[0] ) );
            // FileBrowserHelpers.CopyFile( FileBrowser.Result[0], destinationPath );
        }
    }

    public void ReadCSV(TMP_Text tmpText)
    {
        GetDataFromPath(tmpText.text);
    }

    private void GetDataFromPath(string csvText)
    {
        UIManager.Instance.EnableGetCSVPanel(false);
        string[] lines = csvText.Split('\n');

        dataItems = new List<DataItem>();

        for (int i = 1; i < lines.Length; i++) // Start from 1 to skip the header line
        {
            string[] values = lines[i].Split(',');

            // Create a new instance of the DataItem class and populate its properties
            DataItem item = new DataItem();
            item.Keyword = values[0];
            item.Volume = int.Parse(values[1]);

            dataItems.Add(item);
        }
        dataItems = dataItems.OrderByDescending(item => item.Volume).ToList();


        SetKeywordColor();
        UIManager.Instance.PopulateKeyordList(dataItems);

        // Use the dataItems list as needed in your Unity application
        // foreach (DataItem item in dataItems)
        // {
        //     Debug.Log($"Name: {item.Keyword}, Age: {item.Volume}");
        // }
    }

    private void SetKeywordColor()
    {
        float min = float.MaxValue;
        float max = float.MinValue;

        foreach (DataItem i in dataItems)
        {
            if (i.Volume > max)
            {
                max = i.Volume;
            }

            if (i.Volume < min)
            {
                min = i.Volume;
            }
        }


        foreach (DataItem i in dataItems)
        {
            float val = i.Volume / max;

            //val = Remap(i.Volume, min, max, 0, 1);
            i.KeywordColor = Ramp.GetPixel((int)val * Ramp.width, 0);
            i.KeywordColor.a = 1;
            //print(i.Keyword + ": " + val + ": " + i.KeywordColor);
        }
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}


[System.Serializable]
public class DataItem
{
    public string Keyword;
    public int Volume;
    public int NumberOfTimesUsed;
    public Color KeywordColor;
}
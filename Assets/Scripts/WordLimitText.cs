using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class WordLimitText : MonoBehaviour
{

    private TMP_Text _textUI;
    public TMP_InputField InputField;

    public int CharacterLimit = 10;
    private void Awake()
    {
        _textUI =  GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateWordLimitText();
    }

    public void UpdateWordLimitText()
    {
        string text = InputField.text.RemoveHtmlTags();
        _textUI.text = text.Length + "/" + CharacterLimit;
    }

    public void CopyToClipboard()
    {
        string text = InputField.text.RemoveHtmlTags();
        GUIUtility.systemCopyBuffer = text;
    }
    
    
}

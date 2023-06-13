using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Util
{

    public static string RemoveHtmlTags(this string input)
    {
        string pattern = @"<[^>]+>|&nbsp;";
        string replacement = "";
        return Regex.Replace(input, pattern, replacement);
    }
    
}

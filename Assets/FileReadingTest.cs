using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class FileReadingTest : MonoBehaviour
{
    private static readonly string FILE_PATH = "TestData/TestData";
    private static readonly Regex ALPHABET_FILTER = new Regex("[^a-zA-Z]");
    
    private Dictionary<string, int> _frequencyTable;

    public TextMeshProUGUI WordCountText;
    public TextMeshProUGUI ResultCountText;
    public TMP_InputField TextInput;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CacheFile());
    }

    // This function is the user saying they're ready to lookup the frequency
    public void PerformSearch()
    {
        if (TextInput == null || TextInput.text.Length == 0)
        {
            Debug.LogError("Cannot perform search without an input or with an empty value!");
            return;
        }

        if (WordCountText)
        {
            WordCountText.text = $"Total Words: {_frequencyTable.Count}";
        }
        
        CheckWord(TextInput.text);
    }
    
    public void CheckWord(string checkedWord)
    {
        var lowerS = checkedWord.ToLower();
        int foundCount = 0;
        if (_frequencyTable.TryGetValue(lowerS, out var curCount))
        {
            foundCount = curCount;
        }
        
        if (ResultCountText)
        {
            ResultCountText.text = $"Result Count: {foundCount}";
        }
    }
    
    void ParseWords(ref string[] stringSet)
    {
        foreach (var rawString in stringSet)
        {
            if (rawString.Length == 0)
            {
                continue;
            }
            
            var lowerVal = rawString.ToLower();
            lowerVal = ALPHABET_FILTER.Replace(lowerVal, String.Empty);
            
            // If it doesn't exist, explicitly initialize it
            if (!_frequencyTable.TryGetValue(lowerVal, out var currentVal))
            {
                currentVal = 0;
            }

            _frequencyTable[lowerVal] = currentVal + 1;
        }
    }
    
    void ParseLine(string line)
    {
        // split it on space
        var spaceDelimited = line.Split(" ");
        var commaDelimited = line.Split(",");

        if (spaceDelimited.Length > commaDelimited.Length)
        {
            ParseWords(ref spaceDelimited);
        }
        else
        {
            ParseWords(ref commaDelimited);
        }
    }

    // Assuming we had multiple files or super long text, IEnumerator would allow us to handle it async and defer from the main thread
    IEnumerator CacheFile()
    {
        // We're going to use the Resources system, since we want this file to be included in the build
        // If we were reading files from disc, we'd typically use the Directory and File functionalities
        // But since we want to make sure it's packaged in unity's bundle, we're using Resources.Load
        var loaded = Resources.Load<TextAsset>(FILE_PATH);
        if (loaded == null)
        {
            Debug.LogError("Cannot find the file specified!");
            yield break;
        }
        
        // Reset the frequency table
        _frequencyTable = new();

        var text = loaded.text;
        ParseLine(text);

        if (WordCountText)
        {
            WordCountText.text = $"Total Words: {_frequencyTable.Count}";
        }

        yield return 0;
    }
}

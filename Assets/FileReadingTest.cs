using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class FileReadingTest : MonoBehaviour
{
    private static string FILE_DIRECTORY = Path.Combine(Application.dataPath, "TestData");
    private static string FILE_PATH = Path.Combine(FILE_DIRECTORY,"TestData.txt");
    
    private Dictionary<string, int> _frequencyTable;

    public TextMeshProUGUI WordCountText;
    public TextMeshProUGUI ResultCountText;
    public TMP_InputField TextInput;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CacheFile());
    }

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

    IEnumerator CacheFile()
    {
        if (!Directory.Exists(FILE_DIRECTORY))
        {
            Debug.LogError("Failed to locate file directory! " + FILE_DIRECTORY);
            yield break;
        }

        _frequencyTable = new();

        using (var stream = File.OpenText(FILE_PATH))
        {
            while (!stream.EndOfStream)
            {
                var currentLine = stream.ReadLine();
                ParseLine(currentLine);
            }
        }
        
        if (WordCountText)
        {
            WordCountText.text = $"Total Words: {_frequencyTable.Count}";
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Branch", menuName = "ScriptableObjects/Branch", order = 1)]
[Serializable]
public class Branch : ScriptableObject, IStoryNode
{
    [SerializeField]
    private StringTriple[] words;
    private int currentIndex;

    private void OnEnable()
    {
        currentIndex = 0;
    }


    public StringTriple GetNextWords(string selectedWord = "")
    {
        var nextWords = words[currentIndex];
        currentIndex++;

        return nextWords;
    }

    public StringTriple PeekThisWord(string selectedWord = "")
    {
        if (currentIndex - 1 < 0 || selectedWord == null)
            return new StringTriple("", "", "");

        return words[currentIndex-1];
    }

    public bool IsStoryNodeFinished()
    {
        return currentIndex >= words.Length;
    }
}

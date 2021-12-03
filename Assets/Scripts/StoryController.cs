using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryController : MonoBehaviour
{
    public Story story;

    [SerializeField]
    private int currentIndex = 0;

    public StringTriple GetNextWords(string selectedWord)
    {
        if (currentIndex < story.storyTree.Length)
        {
            var currentNode = story.storyTree[currentIndex];
            var storyNode = (IStoryNode)currentNode;

            if (!storyNode.IsStoryNodeFinished())
            {
                return storyNode.GetNextWords(selectedWord);
            }

            else
            {
                currentIndex++;
                return GetNextWords(selectedWord);
            }
        }
        else
        {
            return new StringTriple("", "", "");
        }
    }
}

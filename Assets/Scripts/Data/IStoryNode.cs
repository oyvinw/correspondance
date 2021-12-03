
using System;

public interface IStoryNode
{
    StringTriple GetNextWords(string selectedWord);
    bool IsStoryNodeFinished();

}
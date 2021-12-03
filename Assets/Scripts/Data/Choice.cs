using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Choice", menuName = "ScriptableObjects/Choice", order = 2)]
[Serializable]
public class Choice : ScriptableObject, IStoryNode
{
    [SerializeField]
    private Branch Option1;

    [SerializeField]
    private Branch Option2;

    [SerializeField]
    private Branch Option3;

    private Branch chosenBranch;

    private void OnEnable()
    {
        chosenBranch = null;
    }

    public StringTriple GetNextWords(string selectedWord)
    {
        if (Option1 != null && selectedWord == Option1.PeekThisWord().one)
        {
            chosenBranch = Option1;
        }
        else if (Option2 != null && selectedWord == Option2.PeekThisWord().one)
        {
            chosenBranch = Option2;
        }
        else if (Option3 != null && selectedWord == Option3.PeekThisWord().one)
        {
            chosenBranch = Option3;
        }

        if (chosenBranch == null)
        {
            return new StringTriple(
                Option1 ? Option1.GetNextWords().one : "",
                Option2 ? Option2.GetNextWords().one : "",
                Option3 ? Option3.GetNextWords().one : "");
        }

        return chosenBranch.GetNextWords();
    }

    public bool IsStoryNodeFinished()
    {
        if (chosenBranch == null)
        {
            return false;
        }
        else
        {
            return chosenBranch.IsStoryNodeFinished();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Story", menuName = "ScriptableObjects/Story", order = 3)]
public class Story : ScriptableObject
{
    public ScriptableObject[] storyTree;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StringTriple{
    public string one;
    public string two;
    public string three;

    public StringTriple(string one, string two, string three)
    {
        this.one = one;
        this.two = two;
        this.three = three;
    }

    public StringTriple()
    {
    }

    public List<string> ToList()
    {
        return new List<string>(3) { one, two, three };
    }
}

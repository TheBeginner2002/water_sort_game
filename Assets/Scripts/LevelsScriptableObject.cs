using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Value
{
    [SerializeField] private List<Color> colors = new List<Color>();

    public List<Color> Colors
    {
        get => colors;
        set => colors = value;
    }
}

[System.Serializable]
public class Map
{
    [SerializeField] private string level;
    [SerializeField] private List<Value> values = new List<Value>();

    public string Level
    {
        get => level;
        set => level = value;
    }

    public List<Value> Values
    {
        get => values;
        set => values = value;
    }
}

[CreateAssetMenu(fileName = "Levels Content",menuName = "Scriptable Object/Levels")]
public class LevelsScriptableObject : ScriptableObject
{
    [SerializeField] private List<Map> levelContent = new List<Map>();

    public List<Map> LevelContent
    {
        get => levelContent;
        set => levelContent = value;
    }
}

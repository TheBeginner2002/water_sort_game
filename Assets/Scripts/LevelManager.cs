using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
    private readonly List<BottleController> _bottles = new List<BottleController>();
    private Map _currentMap;
    private bool _isCompleted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => _isCompleted = value;
    }

    public void SpawnBottle(BottleController bottlePrefab,Map map,GameObject container)
    {
        
        _currentMap = map;
        for (int i = 0; i < _currentMap.Values.Count; i++)
        {
            var bottle = Instantiate(bottlePrefab,new Vector3(-.5f+(i * .5f),0,0),Quaternion.identity, container.transform);
            bottle.NumberOfColorsInBottle =
                _currentMap.Values[i].Colors.Count;
            int indexColor = 0;
            foreach (var color in _currentMap.Values[i].Colors)
            {
                bottle.BottleColors[indexColor] = color;
                indexColor++;
            }
            _bottles.Add(bottle);
        }
    }

    public void CheckLevelComplete()
    {
        if (_bottles.Count > 0)
        {
            int numBottlesCompleted = 0;
            foreach (var bottle in _bottles)
            {
                if (bottle.CheckBottleColorIsFull() || bottle.NumberOfColorsInBottle == 0)
                {
                    numBottlesCompleted++;
                }
            }

            if (numBottlesCompleted == _bottles.Count)
            {
                _isCompleted = true;
            }
            else
            {
                _isCompleted = false;
            }
        }
        else
        {
            _isCompleted = false;
        }
        
    }
}

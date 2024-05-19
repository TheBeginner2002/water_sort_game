using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private BottleController bottlePrefab;
    [SerializeField] private LevelsScriptableObject levels;
    [SerializeField] private GameObject uiLevelPrefab;
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject winUI;
    
    private GameObject _container;
    private int _currentNum;

    private void Start()
    {
        UILevel();
        winUI.SetActive(false);
        gameplayUI.SetActive(false);
    }

    private void Update()
    {
        EnableWinUI();
    }

    void CreateContainer()
    {
        _container = new GameObject
        {
            name = "Container",
            transform =
            {
                position = Vector3.zero
            }
        };
    }

    void UILevel()
    {
        for (int index = 0; index < levels.LevelContent.Count;index++)
        {
            int numLevel = index;
            var level = Instantiate(uiLevelPrefab,uiContainer.transform);
            level.GetComponentInChildren<TMP_Text>().text = levels.LevelContent[numLevel].Level;
            level.GetComponent<Button>().onClick.AddListener(() => EnterLevel(numLevel));
        }
    }

    void EnterLevel(int levelNum)
    {
        _currentNum = levelNum;
        CreateContainer();
        LevelManager.Instance.SpawnBottle(bottlePrefab,levels.LevelContent[levelNum],_container);
        menuUI.SetActive(false);
        gameplayUI.SetActive(true);
    }

    public void ComeBackUI()
    {
        Destroy(_container);
        gameplayUI.SetActive(false);
        menuUI.SetActive(true);
    }

    void EnableWinUI()
    {
        if (LevelManager.Instance.IsCompleted)
        {
            SoundManager.Instance.CorrectSound();
            gameplayUI.SetActive(false);
            winUI.SetActive(true);
            LevelManager.Instance.IsCompleted = false;
        }
    }

    public void NextLevel()
    {
        SoundManager.Instance.StopPlaying();
        LevelManager.Instance.IsCompleted = false;
        winUI.SetActive(false);
        Destroy(_container);
        gameplayUI.SetActive(true);
        CreateContainer();
        LevelManager.Instance.SpawnBottle(bottlePrefab,levels.LevelContent[++_currentNum],_container);
    }

    public void BackToMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManger : MonoBehaviour
{
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private GameObject mainMenu;

    private void Awake()
    {
        MainMenu();
    }

    public void ChangePlayScene()
    {
        SceneManager.LoadScene(1);
    }

    public void SettingMainMenu()
    {
        settingMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void MainMenu()
    {
        settingMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}

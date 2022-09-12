using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject highScorePanel;
    public GameObject pauseMenu;
    public GameObject giveUpPanel;
    public GameObject settings;
    public GameObject settingsPanel;
    public GameObject backGround;
    public Image backgroundImage;
    public Sprite fall;
    public Sprite ocean;
    public Sprite aurora;   
    public Sprite greenFelt;
    public GameObject timer;
    

    private SpriteRenderer spriteRenderer;
    public bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!giveUpPanel.activeSelf && !settingsPanel.activeSelf)
        {
            if (Input.GetKeyDown("escape"))
            {

                if (isPaused == true)
                {
                    Time.timeScale = 1.0f;
                    pauseMenu.SetActive(false);
                    isPaused = false;
                }
                else
                {

                    Time.timeScale = 0.0f;
                    pauseMenu.SetActive(true);
                    isPaused = true;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown("escape"))
            {
                Time.timeScale = 1.0f;
                giveUpPanel.SetActive(false);
                settingsPanel.SetActive(false);
                isPaused = false;
            }
        }
    }
    public void PlayAgain()
    {
        highScorePanel.SetActive(false);
        ResetScene();
    }

    public void ResetScene()
    {
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach (UpdateSprite card in cards)
        {
            Destroy(card.gameObject);
        }

        ClearTopValues();

        FindObjectOfType<Solitaire>().PlayCards();

        Timer resetTime = FindObjectOfType<Timer>();

        resetTime.secondsCount = 0;
        resetTime.minuteCount = 0;
        resetTime.hourCount = 0;

    }

    void ClearTopValues()
    {
        Selectable[] selectables = FindObjectsOfType<Selectable>();

        foreach (Selectable selectable in selectables)
        {
            if (selectable.CompareTag("Top"))
            {
                selectable.suit = null;
                selectable.value = 0;
            }
        }
    }

    public void LoadLevelByName(string levelToLoadName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(levelToLoadName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

        
    public void Resume()        
    {          
        Time.timeScale = 1.0f;            
        pauseMenu.SetActive(false);
        giveUpPanel.SetActive(false);
        settingsPanel.SetActive(false);
        isPaused = false;
    }

    public void GiveUpMenu()
    {
        pauseMenu.SetActive(false);
        giveUpPanel.SetActive(true);
        isPaused = true;

    }

    public void Settings()
    {
        if (!pauseMenu.activeSelf && !giveUpPanel.activeSelf)
        {
            Time.timeScale = 0.0f;
            settingsPanel.SetActive(true);
            isPaused = true;
        }
    }

 
    public void fallButton()
    {
        backgroundImage.sprite = fall;
    }

    public void oceanButton()
    {
        backgroundImage.sprite = ocean;
    }
    public void auroraButton()
    {
        backgroundImage.sprite = aurora;
    }

    public void feltButton()
    {
        backgroundImage.sprite = greenFelt;
    }

    public void mainMenu()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        Time.timeScale = 1.0f;
        ResetScene();
        SceneManager.LoadScene("Main Menu");

    }

    public void PauseMenu()
    {
        settingsPanel.SetActive(false);
        giveUpPanel.SetActive(true);
    }
}

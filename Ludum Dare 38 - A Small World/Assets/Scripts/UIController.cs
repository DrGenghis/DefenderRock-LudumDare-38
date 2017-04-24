using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public static UIController instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    public GameObject lifeBar;      // Displays the life of the player
    public GameObject oreBar;       // Displays the remaining ore in the planet
    public GameObject score;        // Displays the player's current score
    public GameObject scrap;        // Displays how much scrap the player currently has
    public GameObject dayCount;     // Displays what day it currently is, out of 7

    public GameObject mainMenu;     // The main menu of the game
    public GameObject victoryScreen;// The screen that shows when you complete the game

    public GameObject messageBar;   // Used for displaying messages to the player
    public GameObject introBar;     // Displays the intro for the game
    public bool introStart;
    public float introTimer;

    // Use this for initialization
    void Start () {
		
	}

	// Update is called once per frame
	void Update () {
        if (introStart) {
            introTimer += Time.deltaTime;

            if (introTimer >= 4.5f) {
                IntroEnd();
                introTimer = 0;
                introStart = false;
            }
        }
	}


    // Updates the ui elements to show the correct values
    public void UIRefresh(int lifeValue, int oreValue, int scoreNum, int scrapNum, int day) {
        lifeBar.transform.GetChild(0).GetComponent<Slider>().value = lifeValue;
        oreBar.transform.GetChild(0).GetComponent<Slider>().value = oreValue;
        score.transform.GetChild(1).GetComponent<Text>().text = scoreNum.ToString();
        scrap.transform.GetChild(0).GetComponent<Text>().text = "x " + scrapNum.ToString();
        dayCount.transform.GetComponent<Text>().text = "Day " + day + "/7";
    }

    // Toggles the UI on and off, used for menus
    public void UIElementsToggle() {
        lifeBar.SetActive(!lifeBar.activeInHierarchy);
        oreBar.SetActive(!oreBar.activeInHierarchy);
        score.SetActive(!score.activeInHierarchy);
        scrap.SetActive(!scrap.activeInHierarchy);
        dayCount.SetActive(!dayCount.activeInHierarchy);
    }

    // Displays a box with the included message
    public void Message(string mes) {
        messageBar.SetActive(true);
        messageBar.transform.GetChild(0).GetComponent<Text>().text = mes;
    }

    // Disables the message box and erases any messages within
    public void ClearMessage() {
        messageBar.transform.GetChild(0).GetComponent<Text>().text = "";
        messageBar.SetActive(false);
    }

    public void MainMenu() {
        mainMenu.SetActive(true);
    }

    public void MainMenuClose() {
        mainMenu.SetActive(false);
        GameManager.instance.gameStarted = true;
        GameManager.instance.paused = false;
        Intro();
    }

    public void VictoryScreen(int score) {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(0).GetComponent<Text>().text = "Congratulations!\nYou have beaten the corporations \nand protected your home!";
        victoryScreen.transform.GetChild(1).GetComponent<Text>().text = "Score: " + score;
    }

    public void VictoryRestart() {
        victoryScreen.SetActive(false);
    }

    public void Intro() {
        introBar.SetActive(true);
        introStart = true;
    }

    public void IntroEnd() {
        introBar.SetActive(false);
    }
}

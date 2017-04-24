using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public List<GameObject> enemyList = new List<GameObject>();     // Stores all of the enemy soldiers.
    public List<GameObject> turretList = new List<GameObject>();    // Stores all of the turrets placed by the player.
    public List<GameObject> enemyShipList = new List<GameObject>();     // Stores all of the enemy ships and drillbots (ships stay ships upon landing, just change sprites).
    public List<GameObject> deadShipList = new List<GameObject>();      // Storess all ships that died after turning into drills, used for end of day cleanup
    public List<GameObject> scrapList = new List<GameObject>();     // Stores all the scrap in the level
    public List<GameObject> bullets = new List<GameObject>();       // Stores all the bullets;
    public Texture2D cursor;

    // Prefabs
    public GameObject enemyShip;
    public GameObject turret;
    GameObject planet;
    GameObject player;
    public bool spawning;
    public bool playerAtHouse;

    // UI Elements
    int score;
    public int ore;
    public int day = 1;
    public float timeLeftInDay = 70.0f;
    public bool dayStarted = false;
    public bool gameStarted;
    public bool paused;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        Cursor.visible = false;

        planet = GameObject.Find("Planet");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnGUI() {
        GUI.DrawTexture( new Rect(Event.current.mousePosition.x - cursor.width, Event.current.mousePosition.y - cursor.height, cursor.width * 2, cursor.height * 2), cursor);
    }

    float timer;

    // Update is called once per frame
    void Update () {
        if (gameStarted && !paused) {
            if (timeLeftInDay <= 0) {
                timeLeftInDay = 0;
            } else if (timeLeftInDay > 0 && dayStarted) {
                timeLeftInDay -= Time.deltaTime;
            }

            if ((enemyList.Count > 0 || enemyShipList.Count > 0) && timeLeftInDay <= 15) {
                UIController.instance.Message("Kill all enemies to continue...");
            } else if (enemyList.Count == 0 && enemyShipList.Count == 0 && playerAtHouse) {
                EndOfDay();
            }

            if (enemyList.Count == 0 && enemyShipList.Count == 0 && spawning && timeLeftInDay <= 30) {
                UIController.instance.Message("Return home to start the new day...");
            }

            if (!dayStarted) {
                if (Input.anyKeyDown) {
                    NewDay();
                }
            } else if (dayStarted && !spawning) {
                timer += Time.deltaTime;
                if (timer >= 5.0f) {
                    spawning = true;
                    timer = 0;
                }
            }

            if (ore <= 0) {
                paused = true;

                UIController.instance.Message("The planet has run dry, the corp has won. Press 'r' to restart.");
            }

            // Shrinks the planet's core based on the amount of ore left in it.
            float coreSize = (float)(ore / 2) / 100;
            planet.transform.GetChild(0).transform.localScale = new Vector3(coreSize, coreSize, 1);
        } else if (!gameStarted) {
            UIController.instance.MainMenu();
        } else if (paused && day == 8) {
            if (Input.GetKeyDown(KeyCode.R)) {
                Restart();
                UIController.instance.VictoryRestart();
            }
        }
	}

    private void FixedUpdate() {
        UIController.instance.UIRefresh(player.GetComponent<PlayerController>().health, ore, score, player.GetComponent<PlayerController>().scrap, day);
    }

    public void AddScore(int value) {
        score += value;
    }

    void NewDay() {
        dayStarted = true;

        UIController.instance.ClearMessage();
        playerAtHouse = false;
    }

    void EndOfDay() {
        player.GetComponent<PlayerController>().health = 30;

        for (int i = 0; i < turretList.Count; i++) {
            turretList[i].GetComponent<Turret>().health = 10;
        }

        DeleteDeadShips();
        DeleteScrap();

        planet.transform.rotation = Quaternion.identity;
        UIController.instance.Message("Press any key to start the day...");
        timeLeftInDay = 60;
        dayStarted = false;
        playerAtHouse = false;
        spawning = false;
        day++;

        if (day == 8) {
            paused = true;
            UIController.instance.VictoryScreen(score);
        }
    }

    // Restarts the game. Called from the death screen or the victory screen.
    public void Restart() {
        DeleteScrap();
        DeleteEnemies();
        DeleteShips();
        DeleteDeadShips();
        DeleteTurrets();

        player.GetComponent<PlayerController>().health = 30;
        player.GetComponent<PlayerController>().scrap = 10;
        player.GetComponent<Animator>().SetTrigger("Reset");
        ore = 200;
        score = 0;
        day = 1;

        planet.transform.rotation = Quaternion.identity;

        paused = false;
        gameStarted = true;
        spawning = false;
        timer = 0.0f;

        UIController.instance.ClearMessage();
    }

    void DeleteEnemies() {
        for (int i = 0; i < enemyList.Count; i++) {
            Destroy(enemyList[i]);
        }

        enemyList.RemoveRange(0, enemyList.Count);
    }

    void DeleteShips() {
        for (int i = 0; i < enemyShipList.Count; i++) {
            Destroy(enemyShipList[i]);
        }

        enemyShipList.RemoveRange(0, enemyShipList.Count);
    }

    void DeleteDeadShips() {
        for (int i = 0; i < deadShipList.Count; i++) {
            Destroy(deadShipList[i]);
        }

        deadShipList.RemoveRange(0, deadShipList.Count);
    }

    void DeleteTurrets() {
        for (int i = 0; i < turretList.Count; i++) {
            Destroy(turretList[i]);
        }

        turretList.RemoveRange(0, turretList.Count);
    }

    void DeleteScrap() {
        for (int i = 0; i < scrapList.Count; i++) {
            Destroy(scrapList[i]);
        }

        scrapList.RemoveRange(0, scrapList.Count);
    }

    void DeleteBullets() {
        for (int i = 0; i < bullets.Count; i++) {
            Destroy(bullets[i]);
        }

        bullets.RemoveRange(0, bullets.Count);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!playerAtHouse) {
            if (collision.gameObject.GetComponent<PlayerController>() != null) {
                if (enemyList.Count == 0 && enemyShipList.Count == 0 && timeLeftInDay <= 30) {
                    playerAtHouse = true;
                }
            }
        }
    }
}
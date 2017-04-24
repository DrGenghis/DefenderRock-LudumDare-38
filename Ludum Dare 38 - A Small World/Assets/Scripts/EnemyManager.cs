using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager instance;

    public GameObject enemyShip;            // Enemy ship prefab
    GameObject planet;                      // The planet in the level
    public float spawnTime = 5f;            // How long between each spawn
    Vector2 spawnPoint;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        planet = GameObject.Find("Planet");

        // Continually spawn enemies until the end of the day
        InvokeRepeating("SpawnEnemyShip", spawnTime, spawnTime);
    }

    void SpawnEnemyShip() {
        if (GameManager.instance.spawning && !GameManager.instance.paused) {
            // Checks to see if there is enough time left in the day
            if (GameManager.instance.timeLeftInDay <= 30.0f) {
                return;
            }

            Vector2 spawnPoint = Random.insideUnitCircle * 40;

            if (Vector2.Distance(spawnPoint, planet.transform.position) <= 25) {
                SpawnEnemyShip();
            } else {
                GameObject tempGO = Instantiate(enemyShip, new Vector3(spawnPoint.x, spawnPoint.y, 0), Quaternion.identity);

                tempGO.transform.SetParent(planet.transform);

                GameManager.instance.enemyShipList.Add(tempGO);
            }
        }
    }
}

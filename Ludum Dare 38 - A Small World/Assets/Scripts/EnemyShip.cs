using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour {

    int health = 5;
    public bool landed = false;
    bool dead;
    bool soldiers;
    int speed = 7;
    int value = 15;
    int soldierNum;

    float drillTimer;
    float spawnTimer;
    float spawnGoal = 1;

    public GameObject enemySoldier;
    public GameObject scrap;

    public AudioClip drill;
    public AudioClip hit;
    public AudioClip death;

    AudioSource audioS;
    AudioSource exAudio;


    GameObject planet;

    Animator animator;
    Animator exAnim;

	// Use this for initialization
	void Start () {
        planet = GameObject.Find("Planet");
        animator = GetComponent<Animator>();

        if (GameManager.instance.day >=2 ) {
            soldiers = true;
        }

        audioS = GetComponent<AudioSource>();
        exAudio = transform.GetChild(1).GetComponent<AudioSource>();
        exAnim = transform.GetChild(1).GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.instance.paused) {
            animator.enabled = true;
            if (!dead) {
                if (!landed) {
                    SetDir(planet);

                    float toMove = speed * Time.deltaTime;
                    transform.Translate(Vector3.right * toMove);
                } else {
                    drillTimer += Time.deltaTime * 1;

                    if (drillTimer >= 5) {
                        SuckOre();

                        drillTimer = 0;
                    }

                    if (soldiers) {
                        if (spawnTimer >= spawnGoal) {
                            SpawnSoldier();

                            if (soldierNum == GameManager.instance.day / 2) {
                                soldiers = false;
                            }

                            spawnTimer = 0;
                            soldierNum++;
                        }

                        spawnTimer += Time.deltaTime;
                    }
                }

                if (health <= 0) {
                    dead = true;
                    audioS.mute = true;

                    DeathAnim();
                }
            }
        } else if (GameManager.instance.paused) {
            animator.enabled = false;
        }
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Planet") {
            landed = true;
            DrillForm();
        }
    }

    void SpawnSoldier() {
        GameObject tempGO = Instantiate(enemySoldier, transform.position, Quaternion.identity);
        tempGO.transform.SetParent(planet.transform);

        GameManager.instance.enemyList.Add(tempGO);
    }

    public void Hurt() {
        animator.SetTrigger("Hurt");
        exAudio.PlayOneShot(hit, 1);
        health--;
    }

    void DeathAnim() {
        animator.SetTrigger("Death");

        if (!landed) {
            GameManager.instance.enemyShipList.Remove(this.gameObject);
            GameManager.instance.AddScore(value);
        } else if (landed) {
            GameManager.instance.enemyShipList.Remove(this.gameObject);
            GameManager.instance.deadShipList.Add(gameObject);
            GameManager.instance.AddScore(value);
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "Dead_Ships";
            SpawnScrap();
        }
    }

    void DeathSound() {
        exAudio.PlayOneShot(death, 1);

        Debug.Log("Played");
    }

    void Explosion() {
        exAnim.SetTrigger("Explode");
    }

    void Death() {
        Destroy(gameObject);
    }

    void DrillForm() {
        animator.SetBool("Drill", true);
        audioS.clip = drill;
        audioS.Play();
        SetDir(planet);
    }

    void SetDir(GameObject target) {
        transform.right = target.transform.position - transform.position;

        if (landed) {
            transform.up = -target.transform.position - -transform.position;
        }
    }

    void SuckOre() {
        GameManager.instance.ore--;
    }

    void SpawnScrap() {
        int amount = Random.Range(1, 3);

        for (int i = 0; i < amount; i++) {
            int rand = Random.Range(-2, 2);
            GameObject tempGO = Instantiate(scrap, transform.position, Quaternion.identity);
            tempGO.transform.RotateAround(planet.transform.position, Vector3.forward, rand * 3);
            tempGO.transform.SetParent(planet.transform);

            tempGO.transform.up = -planet.transform.position - -tempGO.transform.position;
            GameManager.instance.scrapList.Add(tempGO);

            tempGO = null;
        }
    }
}

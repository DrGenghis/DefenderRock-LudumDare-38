using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoldier : MonoBehaviour {

    int health = 3;             // Health of the enemy.
    int speed = 6;              // Speed the enemy moves at.
    float atkCDTimer = 0;       // Cooldown timer for attacks.
    float atkCD = 2;            // Time it takes for the attack to cool down.
    int value = 5;              // Points this unit is worth.

    float timer;                // Handles moving left and right

    bool facingLeft = false;
    bool hasTarget;
    bool stunned;
    float stunTimer;

    Animator animator;

    public AudioClip shoot;
    public AudioClip hit;
    public AudioClip death;

    AudioSource audioS;

    public GameObject target;
    GameObject player;
    public GameObject projectile;
    public GameObject scrap;
    GameObject projectileSpawn;
    GameObject planet;
    //GameObject ammo;      Will add later if time permits.

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");

        projectileSpawn = transform.GetChild(0).gameObject;

        planet = GameObject.Find("Planet");

        transform.up = -planet.transform.position - -transform.position;

        audioS = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.instance.paused) {
            GetComponent<Animator>().enabled = true;
            atkCDTimer += Time.deltaTime;

            if (atkCDTimer >= atkCD && hasTarget) {
                Attack();

                atkCDTimer = 0;
            }

            if (health <= 0) {
                Death();
            }

            if (stunned) {
                stunTimer += Time.deltaTime;
            }

            if (stunTimer >= 0.5f) {
                stunned = false;
                stunTimer = 0;
            }
        } else if (GameManager.instance.paused) {
            animator.enabled = false;
        }
	}

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Player" || collision.tag == "Turret") {
            target = collision.gameObject;

            hasTarget = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player" || collision.tag == "Turret") {
            hasTarget = false;
        }
    }

    void FixedUpdate() {
        if (!GameManager.instance.paused) {
            if (facingLeft) {
                transform.localScale = new Vector3(1, 1);
            } else {
                transform.localScale = new Vector3(-1, 1);
            }

            if (hasTarget == false) {
                facingLeft = PlayerLeftOfEnemy();
            }

            if (facingLeft && hasTarget == false && !stunned) {
                transform.RotateAround(planet.transform.position, Vector3.forward, speed * Time.deltaTime);
            } else if (!facingLeft && hasTarget == false) {
                transform.RotateAround(planet.transform.position, Vector3.back, speed * Time.deltaTime);
            }
        }

    }

    // Triggers the attack animation and fires a projectile at their target
    void Attack() {
        animator.SetTrigger("Attack");
    }

    // Triggers the hurt animation and stuns the enemy (resets the attack cd)
    public void Hurt() {
        animator.SetTrigger("Hurt");
        audioS.PlayOneShot(hit, 1);
        atkCDTimer = 0;
        health--;
        stunned = true;
    }

    // Triggers the death animation and removes the unit from the enemy list before destroying it
    void Death() {
        animator.SetTrigger("Death");
        audioS.PlayOneShot(death, 1);
        SpawnScrap();

        GameManager.instance.enemyList.Remove(this.gameObject);
        GameManager.instance.AddScore(value);
        Destroy(gameObject);
    }

    // Spawns the bullet at the correct point in the animation
    void BulletSpawn() {
        audioS.PlayOneShot(shoot, 1);
        GameObject bullet = Instantiate(projectile, projectileSpawn.transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDir(target);
        bullet.transform.SetParent(planet.transform);
        GameManager.instance.bullets.Add(bullet);
    }

    bool PlayerLeftOfEnemy() {
        int quad = CheckQuadrant(gameObject);
        int pQuad = CheckQuadrant(player);


        if (quad == 1) {
            if (pQuad == 1) {
                return LeftOfPlayerInSQ(quad, player);
            } else if (pQuad == 2) {
                return false;
            } else if (pQuad == 3) {
                return true;
            } else if (pQuad == 4) {
                return true;
            }
        } else if (quad == 2) {
            if (pQuad == 1) {
                return true;
            } else if (pQuad == 2) {
                return LeftOfPlayerInSQ(quad, player);
            } else if (pQuad == 3) {
                return true;
            } else if (pQuad == 4) {
                return false;
            }
        } else if (quad == 3) {
            if (pQuad == 1) {
                return false;
            } else if (pQuad == 2) {
                return true;
            } else if (pQuad == 3) {
                return LeftOfPlayerInSQ(quad, player);
            } else if (pQuad == 4) {
                return true;
            }
        } else if (quad == 4) {
            if (pQuad == 1) {
                return true;
            } else if (pQuad == 2) {
                return true;
            } else if (pQuad == 3) {
                return false;
            } else if (pQuad == 4) {
                return LeftOfPlayerInSQ(quad, player);
            }
        }

        return false;
    }

    int CheckQuadrant(GameObject checking) {
        if (checking.transform.position.x >= 0 && checking.transform.position.x < 11) {
            if (checking.transform.position.y > -11 && checking.transform.position.y <= 0) {
                return 4;       // Bottom-Right Quadrant
            } else if (checking.transform.position.y >= 0 && checking.transform.position.y < 11) {
                return 2;       // Top-Right Quadrant
            }
        } else if (checking.transform.position.x > -11 && checking.transform.position.x <= 0) {
            if (checking.transform.position.y > -11 && checking.transform.position.y <= 0) {
                return 3;       // Bottom-Left Quadrant
            } else if (checking.transform.position.y >= 0 && checking.transform.position.y < 11) {
                return 1;
            }
        }


        return 0;
    }

    bool LeftOfPlayerInSQ(int quad, GameObject checking) {
        if (quad == 1) {
            if (transform.position.x < checking.transform.position.x) {
                return false;
            } else {
                return true;
            }
        } else if (quad == 2) {
            if (transform.position.x > checking.transform.position.x) {
                return true;
            } else {
                return false;
            }
        } else if (quad == 3) {
            if (transform.position.x < checking.transform.position.x) {
                return false;
            } else {
                return true;
            }
        } else if (quad == 4) {
            if (transform.position.x > checking.transform.position.x) {
                return false;
            } else {
                return true;
            }
        }

        return false;
    }

    void SpawnScrap() {
        GameObject tempGO = Instantiate(scrap, transform.position, Quaternion.identity);
        tempGO.transform.SetParent(planet.transform);

        tempGO.transform.up = -planet.transform.position - -tempGO.transform.position;
        GameManager.instance.scrapList.Add(tempGO);
    }
}
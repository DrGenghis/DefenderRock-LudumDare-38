using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public int health;
    public int speed;
    public bool dead;
    public int scrap;
    bool charged;

    public GameObject projectile;
    public GameObject projectileSpawn;
    public GameObject playerWeapon;
    public GameObject turret;

    public AudioClip hit;
    public AudioClip charge;
    public AudioClip shoot;
    public AudioClip pickup;

    AudioSource aSource;

    GameObject planet;

    public float atkCD;
    public float atkCDTimer;

    public bool invincible;
    public float invincibilityDur;
    public float invincTimer;
    public float blinkTimer;

    Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();

        planet = GameObject.Find("Planet");
        aSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.instance.paused) {
            animator.enabled = true;
            if (!dead) {
                // Movement
                if (Input.GetKey(KeyCode.A)) {
                    planet.transform.Rotate(Vector3.back * (Time.deltaTime * speed));
                    animator.SetBool("Walking", true);
                }

                if (Input.GetKey(KeyCode.D)) {
                    planet.transform.Rotate(Vector3.forward * (Time.deltaTime * speed));
                    animator.SetBool("Walking", true);
                }

                if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
                    animator.SetBool("Walking", false);
                }

                // Correct player facing based on mouse position
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > 0) {
                    transform.localScale = new Vector3(1, 1);
                    playerWeapon.transform.localScale = new Vector3(1, 1);
                } else if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < 0) {
                    transform.localScale = new Vector3(-1, 1);
                    playerWeapon.transform.localScale = new Vector3(-1, -1);
                }

                // Controlling the weapon facing
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 targetPos = mousePos - playerWeapon.transform.position;

                float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
                playerWeapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                // Firing
                if (atkCDTimer >= atkCD) {
                    if (!charged) {
                        aSource.PlayOneShot(charge, 1);
                        charged = true;
                        playerWeapon.gameObject.GetComponent<Animator>().SetBool("Charged", true);
                    }
                    if (Input.GetMouseButtonDown(0)) {
                        GameObject bullet = Instantiate(projectile, projectileSpawn.transform.position, Quaternion.identity);
                        bullet.GetComponent<Bullet>().SetDir(targetPos);
                        bullet.transform.SetParent(planet.transform);
                        bullet.GetComponent<Bullet>().playerBullet = true;
                        GameManager.instance.bullets.Add(bullet);

                        atkCDTimer = 0.0f;
                        playerWeapon.gameObject.GetComponent<Animator>().SetBool("Charged", false);
                        aSource.PlayOneShot(shoot, 1);
                        charged = false;
                    }
                } else if (atkCDTimer < atkCD) {
                    atkCDTimer += Time.deltaTime;
                }

                // Placing Turrets
                if (scrap >= 5) {
                    if (Input.GetKeyDown(KeyCode.Space)) {
                        GameObject tempGO = Instantiate(turret, transform.position, Quaternion.identity);
                        GameManager.instance.turretList.Add(tempGO);
                        tempGO.transform.SetParent(planet.transform);

                        scrap -= 5;
                    }
                }

                // Invincibility Frames
                if (invincible) {
                    invincTimer += Time.deltaTime;
                    blinkTimer += Time.deltaTime;

                    if (blinkTimer >= 0.1f) {
                        GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
                        playerWeapon.GetComponent<SpriteRenderer>().enabled = !playerWeapon.GetComponent<SpriteRenderer>().enabled;
                        blinkTimer = 0;
                    }

                    if (invincTimer >= invincibilityDur) {
                        GetComponent<SpriteRenderer>().enabled = true;
                        playerWeapon.GetComponent<SpriteRenderer>().enabled = true;

                        invincTimer = 0;
                        invincible = false;
                    }
                }

                // Pausing
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    GameManager.instance.paused = true;
                    UIController.instance.Message("Paused...");
                }

                if (health <= 0) {
                    Death();
                }
            }
        } else if (GameManager.instance.paused && GameManager.instance.gameStarted) {
            animator.enabled = false;

            if (Input.GetKeyDown(KeyCode.Escape) && (GameManager.instance.ore > 0 || health > 0)) {
                GameManager.instance.paused = false;
                UIController.instance.ClearMessage();
            }
            
            if (Input.GetKeyDown(KeyCode.R) && (GameManager.instance.ore <= 0 || health <= 0)) {
                GameManager.instance.Restart();
            }
        }
	}

    public void Hurt() {
        animator.SetTrigger("Hurt");
        invincible = true;
        health--;
        aSource.PlayOneShot(hit, 1);
    }

    public void Death() {
        animator.SetTrigger("Death");
        playerWeapon.GetComponent<SpriteRenderer>().enabled = false;
        dead = true;
        aSource.PlayOneShot(hit, 1);
        UIController.instance.Message("You have died. Press 'r' to restart.");
        GameManager.instance.paused = true;
    }
}

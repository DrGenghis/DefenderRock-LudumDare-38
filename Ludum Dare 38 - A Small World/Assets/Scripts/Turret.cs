using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    public int health = 10;

    public List<GameObject> enemiesInRange = new List<GameObject>();        // All enemies within the range of the turret

    public GameObject projectile;
    GameObject projectileSpawn;
    GameObject mount;
    GameObject planet;
    public Transform closestEnemy;

    public GameObject scrap;

    float atkCD = 1.5f;
    float atkCDTimer;

    public AudioClip shoot;
    public AudioClip hit;
    public AudioClip death;

    bool dying = false;

    AudioSource audioS;

    Animator anim;
    Animator exAnim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();

        mount = transform.GetChild(0).gameObject;
        exAnim = mount.transform.GetChild(0).GetComponent<Animator>();

        projectileSpawn = mount.transform.GetChild(0).gameObject;
        planet = GameObject.Find("Planet");
        audioS = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.instance.paused) {
            anim.enabled = true;
            if (enemiesInRange.Count > 0) {
                closestEnemy = GetClosestEnemy(enemiesInRange);

                Aim(closestEnemy);

                if (atkCDTimer >= atkCD) {
                    Attack();
                    atkCDTimer = 0;
                } else {
                    atkCDTimer += Time.deltaTime;
                }
            }

            if (health <= 0 && !dying) {
                Death();
                dying = true;
            }

            if (health <= 0 && dying) {
                SpawnScrap();
                GameManager.instance.turretList.Remove(this.gameObject);
                Destroy(gameObject);
            }
        } else if (GameManager.instance.paused) {
            anim.enabled = false;
        }
	}

    // Adds enemies into the list if they move within range
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<EnemyShip>() != null || collision.gameObject.GetComponent<EnemySoldier>() != null) {
            enemiesInRange.Add(collision.gameObject);
        }
    }

    // Removes enemies from the list if they leave firing range
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<EnemyShip>() != null || collision.gameObject.GetComponent<EnemySoldier>() != null) {
            enemiesInRange.Remove(collision.gameObject);
        }
    }

    void Attack() {
        anim.SetTrigger("Attack");
        
    }

    void AttackSound() {
        audioS.PlayOneShot(shoot, .6f);
    }

    void BulletSpawn() {
        GameObject bullet = Instantiate(projectile, projectileSpawn.transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDir(closestEnemy);
        bullet.transform.SetParent(planet.transform);
        bullet.GetComponent<Bullet>().turretBullet = true;
    }

    public void Hurt() {
        anim.SetTrigger("Hurt");
        health--;
        audioS.PlayOneShot(hit, 1);
    }

    void Death() {
        anim.SetTrigger("Death");
        exAnim.SetTrigger("Explode");
        audioS.PlayOneShot(death, 1);
    }

    void Remove() {
        GameManager.instance.turretList.Remove(this.gameObject);
        Destroy(gameObject);
    }

    Transform GetClosestEnemy(List<GameObject> enemies) {
        Transform closest = null;
        float closestSq = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        for (int i = 0; i < enemies.Count; i++) {
            Vector3 directionToTarget = enemies[i].transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestSq) {
                closestSq = dSqrToTarget;
                closest = enemies[i].transform;
            }
        }

        return closest;
    }

    void Aim(Transform target) {
        mount.transform.up = target.position - mount.transform.position;
    }
    
    void SpawnScrap() {
        GameObject tempGO = Instantiate(scrap, transform.position, Quaternion.identity);
        tempGO.transform.SetParent(planet.transform);

        tempGO.transform.up = -planet.transform.position - -tempGO.transform.position;
        GameManager.instance.scrapList.Add(tempGO);
    }
}

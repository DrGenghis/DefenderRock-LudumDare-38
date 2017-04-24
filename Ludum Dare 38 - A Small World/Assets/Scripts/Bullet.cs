using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    Vector2 dir;
    int speed = 15;

    public bool playerBullet;
    public bool turretBullet;
    bool hit = true;

    float bulletLife = 5;
    float bulletTimer;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.instance.paused) {
            float toMove = speed * Time.deltaTime;
            transform.Translate(Vector3.right * toMove);

            bulletTimer += Time.deltaTime;

            if (bulletTimer >= bulletLife) {
                GameManager.instance.bullets.Remove(this.gameObject);

                Destroy(gameObject);
            }
        }
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        if (hit) {
            if (!playerBullet && !turretBullet) {
                if (collision.gameObject.GetComponent<PlayerController>() != null) {
                    if (!collision.gameObject.GetComponent<PlayerController>().invincible) {
                        collision.gameObject.GetComponent<PlayerController>().Hurt();
                        hit = false;
                    }
                } else if (collision.gameObject.GetComponent<Turret>() != null) {
                    collision.gameObject.GetComponent<Turret>().Hurt();
                    hit = false;
                }
            } else if (playerBullet) {
                if (collision.gameObject.GetComponent<EnemySoldier>() != null) {
                    collision.gameObject.GetComponent<EnemySoldier>().Hurt();
                    hit = false;
                } else if (collision.gameObject.GetComponent<EnemyShip>() != null) {
                    collision.gameObject.GetComponent<EnemyShip>().Hurt();
                    hit = false;
                }
            } else if (turretBullet) {
                if (collision.gameObject.GetComponent<EnemySoldier>() != null) {
                    collision.gameObject.GetComponent<EnemySoldier>().Hurt();
                    hit = false;
                } else if (collision.gameObject.GetComponent<EnemyShip>() != null) {
                    collision.gameObject.GetComponent<EnemyShip>().Hurt();
                    hit = false;
                }
            }
        }

        GameManager.instance.bullets.Remove(this.gameObject);
        Destroy(gameObject);
    }

    public void SetDir(GameObject target) {
        transform.right = target.transform.position - transform.position;
    }

    public void SetDir(Transform target) {
        transform.right = target.transform.position - transform.position;
    }

    public void SetDir(Vector2 target) {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = mousePos - transform.position;

        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

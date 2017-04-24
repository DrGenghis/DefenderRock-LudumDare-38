using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour {

    void Update() {
        if (!GameManager.instance.paused) {
            GetComponent<Animator>().enabled = true;
        } else if (GameManager.instance.paused) {
            GetComponent<Animator>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<PlayerController>() != null) {
            collision.gameObject.GetComponent<PlayerController>().scrap++;
            
            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(collision.gameObject.GetComponent<PlayerController>().pickup);

            GameManager.instance.scrapList.Remove(this.gameObject);
            Destroy(gameObject);
        }
    }

    public void End() {
        Destroy(gameObject);
    }
}

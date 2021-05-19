using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Globals globals;
    public Rigidbody2D rigidBody;
    public CapsuleCollider2D capsuleCollider;
    public SpriteRenderer spriteRenderer;
    public List<BulletSnapshot> history;
    public static float speed = 50f;
    public bool dead = false;
    BulletSnapshot currentFrameSnapshot = new BulletSnapshot();

    public GameObject hitEffect;
    

    void Awake() {
        globals = GameObject.Find("Globals").GetComponent<Globals>();
    }
    void Start()
    {
        history = new List<BulletSnapshot>();
        rigidBody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();


    }

    // Update is called once per frame
    void Update()
    {
        if (globals.rewinding) {
            if (history.Count == 0) {
                Destroy(gameObject);
                return;
            }
            BulletSnapshot snapshot = history[0];
            transform.position = snapshot.position;
            rigidBody.velocity = snapshot.velocity;
            for (int i = 0; i < snapshot.lambdasToExecute.Count; i++) {
                RewindFunc func = snapshot.lambdasToExecute[i];
                func(gameObject);
            }
            history.RemoveAt(0);
        } else {
            if (!dead) {
                rigidBody.simulated = true;
                capsuleCollider.enabled = true;
            }
            currentFrameSnapshot.position = (Vector2) rigidBody.position;
            currentFrameSnapshot.velocity = (Vector2) rigidBody.velocity;
        }
    }

    // void FixedUpdate() {
    //     if (!globals.rewinding) {
    //         currentFrameSnapshot.position = (Vector2) rigidBody.position;
    //         currentFrameSnapshot.velocity = (Vector2) rigidBody.velocity;
    //     }
    // }

    void LateUpdate() {
        if (!globals.rewinding){
            history.Insert(0, currentFrameSnapshot);
            currentFrameSnapshot = new BulletSnapshot();
            while (history.Count > globals.targetFramerate * globals.secondsOfRewind) {
                if (history[history.Count - 1].expiry) {
                    Destroy(gameObject);
                    return;
                }
                history.RemoveAt(history.Count - 1);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        GameObject effect = Object.Instantiate(hitEffect, rigidBody.transform.position, Quaternion.identity);
        // effect.GetComponent<Animator>().Play("Tag");
        // Destroy(effect, 10f);
        currentFrameSnapshot.lambdasToExecute.Add(Delegates.bulletUndoDeath);
        currentFrameSnapshot.expiry = true;
        death();
    }

    private void death() {
        dead = true;
        rigidBody.velocity = Vector2.zero;
        rigidBody.simulated = false;
        spriteRenderer.enabled = false;
        capsuleCollider.enabled = false;
    }
}
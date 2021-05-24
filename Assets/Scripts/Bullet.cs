using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Globals globals;
    public CapsuleCollider2D capsuleCollider;
    public SpriteRenderer spriteRenderer;
    public List<BulletSnapshot> history;
    public static float speed = 50f;
    public bool dead = false;
    BulletSnapshot currentFrameSnapshot = new BulletSnapshot();
    ContactFilter2D contactFilter = new ContactFilter2D();

    public GameObject hitEffect;
    

    void Awake() {
        globals = GameObject.Find("Globals").GetComponent<Globals>();
    }
    void Start()
    {
        history = new List<BulletSnapshot>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();


    }

    void Update()
    {
        if (globals.getRewinding())
        {
            if (history.Count == 0)
            {
                Destroy(gameObject);
                return;
            }
            BulletSnapshot snapshot = history[0];
            transform.position = getInterpolatedPosition();
            //Debug.Log(transform.position);
            for (int i = 0; i < snapshot.lambdasToExecute.Count; i++)
            {
                RewindFunc func = snapshot.lambdasToExecute[i];
                func(gameObject);
            }
            if (globals.getIsPopFrame())
            {
                history.RemoveAt(0);
            }
        }
        else
        {
            if (!dead)
            {
                capsuleCollider.enabled = true;
            }
            RaycastHit2D[] results = new RaycastHit2D[1];
            Physics2D.CapsuleCast(transform.position, capsuleCollider.size, CapsuleDirection2D.Vertical, 0f, transform.up, contactFilter.NoFilter(), results, 1f);
            if (results[0].collider == null)
            {
                transform.position = (Vector2)transform.position + (Vector2) transform.up * 1f;
            } else
            {
                transform.position = results[0].point;
            }
            currentFrameSnapshot.position = (Vector2) transform.position;
        }
    }

    private Vector2 getInterpolatedPosition()
    {
        if (history.Count < 2)
        {
            return history[0].position;
        } else
        {
            Vector2 snapshot1Position = history[0].position;
            Vector2 snapshot2Position = history[1].position;
            float factor = globals.rewindInterpolationFactor();
            return Vector2.Lerp(snapshot1Position, snapshot2Position, Mathf.Min(factor, 1f));
        }

    }

    // void FixedUpdate() {
    //     if (!globals.rewinding) {
    //         currentFrameSnapshot.position = (Vector2) rigidBody.position;
    //         currentFrameSnapshot.velocity = (Vector2) rigidBody.velocity;
    //     }
    // }

    void LateUpdate() {
        if (!globals.getRewinding()){
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
        GameObject effect = Object.Instantiate(hitEffect, transform.position, Quaternion.identity);
        // effect.GetComponent<Animator>().Play("Tag");
        // Destroy(effect, 10f);
        currentFrameSnapshot.lambdasToExecute.Add(Delegates.bulletUndoDeath);
        currentFrameSnapshot.expiry = true;
        death();
    }



    private void death() {
        dead = true;
        spriteRenderer.enabled = false;
        capsuleCollider.enabled = false;
    }
}

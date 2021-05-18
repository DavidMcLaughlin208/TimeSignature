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
    public Vector2 originPosition;
    public Vector2 direction;
    public float speed = 8f;
    public bool dead = false;

    delegate void disableComponents();

    BulletSnapshot currentFrameSnapshot = new BulletSnapshot();

    BulletFunc undoDeath;

    void Awake() {
        globals = GameObject.Find("Globals").GetComponent<Globals>();
    }
    void Start()
    {
        history = new List<BulletSnapshot>();
        rigidBody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Vector2 transformedDirection = transform.TransformDirection( direction );
        // rigidBody.AddForce(transformedDirection * speed, ForceMode2D.Impulse);

        undoDeath = () => {
            dead = false;
            spriteRenderer.enabled = true;
        };
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
            // if (snapshot.wait) {
            //     return;
            // }
            rigidBody.simulated = false;
            capsuleCollider.enabled = false;
            if (!(snapshot.position == Vector2.zero)) {
                transform.position = snapshot.position;
            }
            if (!(snapshot.velocity  == Vector2.zero)) {
                rigidBody.velocity = snapshot.velocity;
            }
            for (int i = 0; i < snapshot.lambdasToExecute.Count; i++) {
                BulletFunc func = snapshot.lambdasToExecute[i];
                func();
                Debug.Log("After executing undo death func");
            }
            history.RemoveAt(0);
            Debug.Log($"CurrentFrame position: {transform.position}");
            // Debug.Log($"Removing history. Count: {history.Count}");
        } else {
            if (!dead) {
                capsuleCollider.enabled = true;
                rigidBody.simulated = true;
                currentFrameSnapshot.position = (Vector2) rigidBody.position;
                currentFrameSnapshot.velocity = (Vector2) rigidBody.velocity;
                Debug.Log($"CurrentFrame position: {currentFrameSnapshot.position}");
            } else {
                currentFrameSnapshot.wait = true;
            }
            
        }
    }

    void LateUpdate() {
        if (!globals.rewinding){
            history.Insert(0, BulletSnapshot.clone(currentFrameSnapshot));
            currentFrameSnapshot = new BulletSnapshot();
            // Debug.Log($"Inserting to history. Count: {history.Count}");
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log($"Hit {other.name}");
        // Destroy(gameObject);
        // Debug.Log("Adding undo Death func");
        currentFrameSnapshot.lambdasToExecute.Add(undoDeath);
        death();
    }

    private void death() {
        Debug.Log("Dying");
        dead = true;
        rigidBody.velocity = Vector2.zero;
        rigidBody.simulated = false;
        // spriteRenderer.enabled = false;
        capsuleCollider.enabled = false;
    }
}

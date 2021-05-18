using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    float speed = 6f;
    Vector2 moveDirection = Vector3.zero;
    Vector2 mousePos = Vector2.zero;

    public Rigidbody2D rigidBody;
    public PlayerSnapshot currentFrameSnapshot = new PlayerSnapshot();

    public List<PlayerSnapshot> history;
    public Globals globals;
    public GameObject bulletPrefab;

    public Transform bulletSpawn;

    void Awake() {
        globals = GameObject.Find("Globals").GetComponent<Globals>();
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        history = new List<PlayerSnapshot>();
        bulletPrefab = globals.prefabManager.bulletPrefab;
        bulletSpawn = transform.Find("BulletSpawn");
    }

    void Update() {
        if (Input.GetKeyDown("r")) {
            globals.rewinding = true;
        }
        if (Input.GetKeyUp("r")) {
            globals.rewinding = false;
        }

        if (globals.rewinding) {
            if (history.Count == 0) {
                return;
            }
            PlayerSnapshot snapshot = history[0];
            transform.position = snapshot.position;
            rigidBody.rotation = snapshot.angle;
            history.RemoveAt(0);
        } else {
            if (Input.GetMouseButtonDown(0)) {
                fireBullet();
            }

            moveDirection.x = Input.GetAxisRaw("Horizontal");
            moveDirection.y = Input.GetAxisRaw("Vertical");
            moveDirection = moveDirection.normalized;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (mousePos - (Vector2) transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            rigidBody.rotation = angle;


            currentFrameSnapshot.position = transform.position;
            currentFrameSnapshot.angle = angle;

            
        }
    }

    void FixedUpdate() {
        rigidBody.MovePosition(rigidBody.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    void LateUpdate() {
        if (!globals.rewinding) {
            history.Insert(0, currentFrameSnapshot);
            currentFrameSnapshot = new PlayerSnapshot();
            while (history.Count > globals.targetFramerate * globals.secondsOfRewind) {
                history.RemoveAt(history.Count - 1);
            }
        }
    }

    private void fireBullet() {
        GameObject bullet = Object.Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bulletSpawn.up * bulletScript.speed, ForceMode2D.Impulse);

        
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    float speed = 6f;
    Vector2 moveDirection = Vector3.zero;
    Vector2 mousePos = Vector2.zero;

    public Rigidbody2D rigidBody;

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
            Debug.Log("Setting Rewind True");
            globals.rewinding = true;
        }
        if (Input.GetKeyUp("r")) {
            Debug.Log("Setting Rewind false");
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
            // moveDirection = transform.TransformDirection(moveDirection);
            // moveDirection *= speed;

            // rigidBody.velocity = moveDirection;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (mousePos - (Vector2) transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            rigidBody.rotation = angle;

            PlayerSnapshot snapshot = new PlayerSnapshot();
            snapshot.position = transform.position;
            snapshot.angle = angle;

            history.Insert(0, snapshot);
        }
    }

    void FixedUpdate() {
        rigidBody.MovePosition(rigidBody.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    private void fireBullet() {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePosition - (Vector2) transform.position).normalized;

        GameObject bullet = Object.Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        // bullet.transform.position = bulletSpawn.position;
        // Quaternion currentRotation = bullet.transform.rotation;
        // bullet.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, transform.eulerAngles.z);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        // bulletScript.direction = dir;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bulletSpawn.up * bulletScript.speed, ForceMode2D.Impulse);

        
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    float speed = 6f;
    Vector2 moveDirection = Vector3.zero;
    Vector2 mousePos = Vector2.zero;

    public Rigidbody2D rb;
    public PlayerSnapshot currentFrameSnapshot = new PlayerSnapshot();

    public List<PlayerSnapshot> history;
    public Globals globals;
    public GameObject bulletPrefab;

    public Transform bulletSpawn;

    public bool slowmo = false;

    // public bool halfSpeed = false;
    void Awake() {
        globals = GameObject.Find("Globals").GetComponent<Globals>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        history = new List<PlayerSnapshot>();
        bulletPrefab = globals.prefabManager.bulletPrefab;
        bulletSpawn = transform.Find("BulletSpawn");
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (!globals.getRewinding()) {
                if (slowmo) {
                    slowmo = false;
                    Time.timeScale = 1f;
                } else {
                    slowmo = true;
                    Time.timeScale = 0.5f;
                }
            }
        }

        if (Input.GetKeyDown("r")) {
            globals.setRewindingTrue();
            if (slowmo) {
                slowmo = false;
                Time.timeScale = 1f;
            } 
        }
        if (Input.GetKeyUp("r")) {
            globals.setRewindingFalse();
        }

        if (globals.getRewinding()) {
            if (history.Count == 0) {
                return;
            }
            PlayerSnapshot snapshot = history[0];
            transform.position = getInterpolatedPosition();
            rb.rotation = getInterpolatedAngle();
            if (globals.getIsPopFrame())
            {
                history.RemoveAt(0);
            }
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
            rb.rotation = angle;


            currentFrameSnapshot.position = transform.position;
            currentFrameSnapshot.angle = angle;

            
        }
    }

    private Vector2 getInterpolatedPosition()
    {
        if (history.Count < 2)
        {
            return history[0].position;
        }
        else
        {
            Vector2 snapshot1Position = history[0].position;
            Vector2 snapshot2Position = history[1].position;
            float factor = globals.rewindInterpolationFactor();
            return Vector2.Lerp(snapshot1Position, snapshot2Position, Mathf.Min(factor, 1f));
        }
    }

    private float getInterpolatedAngle()
    {
        if (history.Count < 2)
        {
            return history[0].angle;
        } else
        {
            return Mathf.Lerp(history[1].angle, history[0].angle, Mathf.Min(globals.rewindInterpolationFactor(), 1));
        }
    }

    void FixedUpdate() {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);  
    }

    void LateUpdate() {
        if (!globals.getRewinding()) {
            history.Insert(0, currentFrameSnapshot);
            currentFrameSnapshot = new PlayerSnapshot();
            while (history.Count > globals.targetFramerate * globals.secondsOfRewind) {
                history.RemoveAt(history.Count - 1);
            }
        }
    }

    private void fireBullet() {
        GameObject bullet = Object.Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        //Bullet bulletScript = bullet.GetComponent<Bullet>();

        //Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        //rb.velocity = bulletSpawn.up * Bullet.speed;
        // rb.AddForce(bulletSpawn.up * bulletScript.speed, ForceMode2D.Impulse);

        
        
    }

}

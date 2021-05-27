using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    float speed = 0.1f;
    Vector2 moveDirection = Vector3.zero;
    Vector2 mousePos = Vector2.zero;

    public PlayerSnapshot currentFrameSnapshot = new PlayerSnapshot();

    public List<PlayerSnapshot> history;
    public Globals globals;
    public GameObject bulletPrefab;

    public Transform bulletSpawn;

    CircleCollider2D circleCollider;

    // public bool halfSpeed = false;
    void Awake() {
        globals = GameObject.Find("Globals").GetComponent<Globals>();
    }

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        history = new List<PlayerSnapshot>();
        bulletPrefab = globals.prefabManager.bulletPrefab;
        bulletSpawn = transform.Find("BulletSpawn");
        history.Add(currentFrameSnapshot);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (!globals.isRewinding()) {
                if (globals.localTimescale.Value == 0.05f)
                {
                    globals.setTimescale(1f);
                } else
                {
                    globals.setTimescale(0.05f);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            globals.togglePause();
        }

        if (Input.GetKeyDown("r")) {
            globals.setRewindingTrue();
        }
        if (Input.GetKeyUp("r")) {
            globals.setRewindingFalse();
        }

        switch (globals.timeState.Value) {
            case (int)TimeState.PLAY:
                if (Input.GetMouseButtonDown(0))
                {
                    fireBullet();
                }

                moveDirection.x = Input.GetAxisRaw("Horizontal");
                moveDirection.y = Input.GetAxisRaw("Vertical");
                moveDirection = moveDirection.normalized;

                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dir = (mousePos - (Vector2)transform.position);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

                float timescaledSpeed = speed * globals.localTimescale.Value;

                RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCollider.radius * 4, moveDirection, timescaledSpeed);
                if (hit.collider != null)
                {

                }
                else
                {
                    Vector2 newPosition = (Vector2)transform.position + moveDirection * timescaledSpeed;
                    transform.position = newPosition;
                    transform.eulerAngles = new Vector3(0, 0, angle);
                }


                currentFrameSnapshot.position = transform.position;
                currentFrameSnapshot.angle = angle;
                break;
            case (int)TimeState.PAUSE:
                break;
            case (int)TimeState.REWIND:
                if (history.Count == 0)
                {
                    return;
                }
                PlayerSnapshot snapshot = history[0];
                transform.position = getInterpolatedPosition();
                transform.eulerAngles = new Vector3(0, 0, getInterpolatedAngle());

                for (int i = 0; i < snapshot.lambdasToExecute.Count; i++)
                {
                    RewindFunc func = snapshot.lambdasToExecute[i];
                    func(gameObject);
                }
                snapshot.lambdasToExecute.Clear();
                if (globals.getIsPopFrame())
                {
                    history.RemoveAt(0);
                }
                break;
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
            float snapshot1Angle = history[0].angle;
            float snapshot2Angle = history[1].angle;
            float factor = Mathf.Min(globals.rewindInterpolationFactor(), 1);
            return Mathf.Lerp(snapshot1Angle, snapshot2Angle, factor);
        }
    }

    void LateUpdate() {
        if (globals.timeState.Value == (int)TimeState.PLAY) {
            if (globals.getIsPopFrame())
            {
                float angle = currentFrameSnapshot.angle;
                currentFrameSnapshot = new PlayerSnapshot();
                currentFrameSnapshot.position = transform.position;
                currentFrameSnapshot.angle = angle;
                history.Insert(0, currentFrameSnapshot);
                
                while (history.Count > globals.targetFramerate * globals.secondsOfRewind)
                {
                    history.RemoveAt(history.Count - 1);
                }
            } else
            {
                history[0] = currentFrameSnapshot;
            }
        }
    }

    private void fireBullet() {
        GameObject bullet = Object.Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.currentFrameSnapshot.position = bulletSpawn.position;
        bulletScript.originPoint = bulletSpawn.position;

        //Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        //rb.velocity = bulletSpawn.up * Bullet.speed;
        // rb.AddForce(bulletSpawn.up * bulletScript.speed, ForceMode2D.Impulse);



    }

}

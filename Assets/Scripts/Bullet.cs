using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Globals globals;
    public CapsuleCollider2D capsuleCollider;
    public SpriteRenderer spriteRenderer;
    public List<BulletSnapshot> history;
    public static float speed = 1f;
    public bool dead = false;
    public BulletSnapshot currentFrameSnapshot = new BulletSnapshot();
    ContactFilter2D contactFilter = new ContactFilter2D();
    public Vector2 originPoint;

    public GameObject hitEffect;
    public LineRenderer trail;
    

    void Awake() {
        globals = GameObject.Find("Globals").GetComponent<Globals>();
    }
    void Start()
    {
        history = new List<BulletSnapshot>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponent<LineRenderer>();
        history.Add(currentFrameSnapshot);


    }

    void Update()
    {
        switch (globals.timeState.Value)
        {
            case (int)TimeState.PLAY:
                if (!dead)
                {
                    capsuleCollider.enabled = true;

                    float timescaledSpeed = speed * globals.localTimescale.Value;
                    RaycastHit2D[] results = new RaycastHit2D[1];
                    Physics2D.CapsuleCast(transform.position, capsuleCollider.size, CapsuleDirection2D.Vertical, 0f, transform.up, contactFilter.NoFilter(), results, timescaledSpeed);
                    if (results[0].collider == null)
                    {
                        transform.position = (Vector2)transform.position + (Vector2)transform.up * timescaledSpeed;
                    }
                    else
                    {
                        Vector2 normal = results[0].normal;
                        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg + 90;
                        GameObject effect = Instantiate(hitEffect, results[0].point, new Quaternion(0,0,angle,0));
                        Destroy(effect, 1);
                        transform.position = results[0].point;
                        setTrailPoints();
                        death();
                    }
                    setTrailPoints();
                }
                currentFrameSnapshot.position = transform.position;
                break;
            case (int)TimeState.PAUSE:
                break;
            case (int)TimeState.REWIND:
                if (history.Count == 0)
                {
                    Destroy(gameObject);
                    return;
                }
                BulletSnapshot snapshot = history[0];
                transform.position = getInterpolatedPosition();
                
                for (int i = 0; i < snapshot.lambdasToExecute.Count; i++)
                {
                    RewindFunc func = snapshot.lambdasToExecute[i];
                    func(gameObject);
                }
                snapshot.lambdasToExecute.Clear();
                
                setTrailPoints();
                if (globals.getIsPopFrame() || history.Count == 1)
                {
                    history.RemoveAt(0);
                }
                break;
        }

    }

    private void setTrailPoints()
    {
        trail.SetPosition(0, transform.position);
        Vector2 secondPosition;
        if (Vector2.Distance(transform.position, transform.position + transform.up * -2) < Vector2.Distance(transform.position, originPoint)) {
            secondPosition = transform.position + transform.up * -2;
        } else
        {
            secondPosition = originPoint;
        }
        trail.SetPosition(1, secondPosition);
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
        if (globals.timeState.Value == (int) TimeState.PLAY) {
            if (globals.getIsPopFrame())
            {
                currentFrameSnapshot = new BulletSnapshot();
                currentFrameSnapshot.position = transform.position;
                history.Insert(0, currentFrameSnapshot);
                while (history.Count > globals.targetFramerate * globals.secondsOfRewind)
                {
                    if (history[history.Count - 1].expiry)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    history.RemoveAt(history.Count - 1);
                }
            } else
            {
                history[0] = currentFrameSnapshot;
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
        currentFrameSnapshot.expiry = true;
        trail.enabled = false;
        currentFrameSnapshot.lambdasToExecute.Add(Delegates.bulletUndoDeath);
    }
}

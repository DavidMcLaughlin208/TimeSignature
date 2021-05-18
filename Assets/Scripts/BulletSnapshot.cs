using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletSnapshot {
    public bool wait = false;
    public Vector2 position;
    public Vector2 velocity;
    public List<RewindFunc> lambdasToExecute = new List<RewindFunc>();

    public static BulletSnapshot clone(BulletSnapshot toClone) {
        return new BulletSnapshot() {
            wait = toClone.wait,
            position = toClone.position,
            velocity = toClone.velocity,
            lambdasToExecute = toClone.lambdasToExecute
        };
    }
}
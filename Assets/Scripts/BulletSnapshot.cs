using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletSnapshot {
    public bool wait = false;
    public Vector2 position;
    public List<RewindFunc> lambdasToExecute = new List<RewindFunc>();
    public bool expiry = false;

    public static BulletSnapshot clone(BulletSnapshot toClone) {
        return new BulletSnapshot() {
            wait = toClone.wait,
            position = toClone.position,
            lambdasToExecute = toClone.lambdasToExecute
        };
    }
}
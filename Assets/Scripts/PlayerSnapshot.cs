using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSnapshot {
    public Vector2 position;
    public float angle;

    public List<RewindFunc> lambdasToExecute = new List<RewindFunc>();
}
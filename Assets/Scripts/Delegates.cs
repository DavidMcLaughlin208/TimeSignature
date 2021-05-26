using UnityEngine;
public delegate void RewindFunc(GameObject instance);

public static class Delegates {
    public static RewindFunc bulletUndoDeath = (bulletInstance) => {
            Bullet bulletInstanceScript = bulletInstance.GetComponent<Bullet>();
            bulletInstanceScript.dead = false;
            bulletInstanceScript.spriteRenderer.enabled = true;
            bulletInstanceScript.trail.enabled = true;
    };
}

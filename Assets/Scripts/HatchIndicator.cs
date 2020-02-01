using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatchIndicator : MonoBehaviour {
    private const float loadTime = 2f;

    private SpriteMask mask;
    private float t;
    internal Player player;

    void Awake () {
        mask = GetComponentInChildren<SpriteMask>();
        t = 0;
    }
    
    void Update () {
        t += Time.deltaTime;
        mask.alphaCutoff = t / loadTime;
        if (t > loadTime) {
            player.HatcingComplete();
            Destroy(gameObject);
        }
    }
}

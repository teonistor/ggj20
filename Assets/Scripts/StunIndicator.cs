using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunIndicator : MonoBehaviour {
    private const float loadTime = 2f;

    private float t;
    internal Player player;
    private SpriteMask mask;

    void Awake () {
        t = 0;
        mask = GetComponentInChildren<SpriteMask>();
    }


    void Update () {
        t += Time.deltaTime;
        mask.alphaCutoff = 1- t / loadTime;
        if (t > loadTime) {
            player.StunOver();
            Destroy(gameObject);
        }
    }
}

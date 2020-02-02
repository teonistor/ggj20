using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunIndicator : MonoBehaviour {
    private const float loadTime = 2f;

    private float t;
    internal Player player;

    void Awake () {
        t = 0;
    }


    void Update () {
        t += Time.deltaTime;
        // TODO move sth
        if (t > loadTime) {
            player.StunOver();
            Destroy(gameObject);
        }
    }
}

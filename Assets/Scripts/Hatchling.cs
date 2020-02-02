using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatchling : MonoBehaviour {
    public Color spriteColorGreen, spriteColorYellow, spriteColorRed;
    public PairColor pairColor;

    void Start() {
        SpriteRenderer rnd = GetComponent<SpriteRenderer>();
        switch (Random.Range(0, 3)) {
            case 0: pairColor = PairColor.Green; rnd.color = spriteColorGreen; break;
            case 1: pairColor = PairColor.Yellow; rnd.color = spriteColorYellow; break;
            case 2: pairColor = PairColor.Red; rnd.color = spriteColorRed; break;
        }
    }
}

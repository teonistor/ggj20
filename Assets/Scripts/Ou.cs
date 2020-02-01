using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ou : MonoBehaviour {

    private Rigidbody2D r2d;
    private int wallsLayer;
    private bool isHeld;

    void Awake() {
        r2d = GetComponent<Rigidbody2D>();
        wallsLayer = LayerMask.NameToLayer("Walls");
        isHeld = false;
    }

   
    void Update()  {
        
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (!isHeld && other.gameObject.layer == wallsLayer) {
            print("Egg be dead");
            r2d.bodyType = RigidbodyType2D.Static;
        }
    }

    internal bool grabHold(Transform holder) {
        if (!isHeld) {
            isHeld = true;
            r2d.bodyType = RigidbodyType2D.Kinematic;
            transform.parent = holder;
            transform.localPosition = new Vector3(1f, 1f, 0f);
            return true;
        }
        return false;
    }

    //void OnClossisionEnter2D (Collision2D collision) {
    //    print(collision.gameObject.layer);
    //}
}

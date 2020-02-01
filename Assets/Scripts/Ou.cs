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

    internal bool GrabHold(Transform holder) {
        if (!isHeld) {
            isHeld = true;
            r2d.bodyType = RigidbodyType2D.Kinematic;
            r2d.velocity = Vector2.zero;
            transform.parent = holder;
            transform.localPosition = new Vector3(1f, 1f, 0f);
            return true;
        }
        return false;
    }

    internal bool Throw(Vector2 velo) {
        if (isHeld) {
            isHeld = false;
            transform.parent = null;
            r2d.bodyType = RigidbodyType2D.Dynamic;
            r2d.velocity = velo;
            return true;
        }
        return false;
    }

    internal bool PutInNest(Transform nest) {
        if (isHeld) {
            // Another bool ??
            transform.parent = nest;
            // TODO Fit around; n.b. Nest is rotated because capsule
            transform.localPosition = new Vector3(0.8f, 0f, 0f);
        }
        return false;
    }

    //void OnClossisionEnter2D (Collision2D collision) {
    //    print(collision.gameObject.layer);
    //}
}

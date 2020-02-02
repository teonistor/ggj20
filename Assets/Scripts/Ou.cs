﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ou : MonoBehaviour {
    //enum State { Initial, Grabbed, Nest, Hatching}

    private const float maxSlideInTime = 3f;

    public Sprite realSprite;
    public PairColor pairColor;

    private Rigidbody2D r2d;
    private int wallsLayer;
    private int nestLayer;
    private bool isHeld;
    

    void Awake() {
        r2d = GetComponent<Rigidbody2D>();
        wallsLayer = LayerMask.NameToLayer("Walls"); 
        nestLayer = LayerMask.NameToLayer("Nest");

        isHeld = false;
    }

   
    void Update()  {
        
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (!isHeld && other.gameObject.layer == wallsLayer) {
            Destroy(gameObject);
        }
    }

    internal void SlideIn(Vector3 leftmost, Vector3 rightmost) {
        float rnd = Random.value;
        transform.position = rightmost;
        StartCoroutine(SlideInCoroutine(rightmost,Vector3.Lerp(rightmost, leftmost, rnd), maxSlideInTime*rnd));
    }

    private IEnumerator SlideInCoroutine (Vector3 start, Vector3 end, float time) {
        
        for (float t=0f; t< time; t += Time.deltaTime) {
            transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t / time));
            yield return new WaitForEndOfFrame();
        }
        r2d.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<SpriteRenderer>().sprite = realSprite;
    }

    internal void GrabHold(Transform holder) {
        isHeld = true;
        r2d.bodyType = RigidbodyType2D.Kinematic;
        r2d.velocity = Vector2.zero;
        transform.parent = holder;
    }

    internal void Throw(Vector2 velo) {
        isHeld = false;
        transform.parent = null;
        r2d.bodyType = RigidbodyType2D.Dynamic;
        r2d.velocity = velo;
    }
}

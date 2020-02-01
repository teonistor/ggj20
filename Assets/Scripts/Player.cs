﻿using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))][RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour {

    [SerializeField]
    private float moveSpeed = 60f, jumpIntensity = 270f,
                            baseJumpDuration = 0.15f, resizeRate = 2.75f,
                            minScale = 1f,
                            maxScale = 4.5f, currentScale = 2f;
    [SerializeField] private LayerMask wallsLayer;

    private Vector3 baseScale;

    private Rigidbody2D r2d;
    private float jumpDurationRemaining;
    private bool controlsEnabled = true;

    [SerializeField] private AudioClip bounce, fall, burn, inflate, levelUp, collect;
    private new AudioSource audio;

    public bool isJumpAllowed { get { return /*TutorialElement.isJumpAllowed*/ true; } }

    private Collider2D[] touchCheckBuffer = new Collider2D[5]; // Needed to do non-alloc collision detection (more memory-efficient)
    private bool touchesFloor {
        get {
            Vector3 floorPoint = transform.position;
            floorPoint.y -= transform.localScale.y * 0.5f;
            return Physics2D.OverlapCircleNonAlloc(floorPoint, 0.2f, touchCheckBuffer, wallsLayer) > 0;
        }
    }

    private bool touchesCeiling {
        get {
            Vector3 ceilingPoint = transform.position;
            ceilingPoint.y += transform.localScale.y * 0.5f;
            return Physics2D.OverlapCircleNonAlloc(ceilingPoint, 0.01f, touchCheckBuffer, wallsLayer) > 0;
        }
    }

    public float minY { get { return -60f; } }

    void Start () {
        baseScale = transform.localScale / currentScale;
        r2d = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
    }

    void FixedUpdate () {
        if (controlsEnabled) {
            // Horizontal movement
            r2d.AddForce(new Vector2(Input.GetAxis("Horizontal1") * moveSpeed, 0f));
            //r2d.velocity = new Vector2(Input.GetAxis("Move") * 4f, 0f) ; // An adaptation of this could be used instead, especially if we go for tilted ramps 

            // Vertical movement
            if (touchesCeiling) {
                // Empty jump "fuel" when hitting ceiling
                jumpDurationRemaining = 0f;
            }
            else if (isJumpAllowed && Input.GetButton("Jump")) {
                // Mid-jump or jump starting now: add vertical force while there still is jump "fuel"
                if (jumpDurationRemaining > 0f) {
                    jumpDurationRemaining -= Time.fixedDeltaTime;
                    r2d.AddForce(new Vector2(0f, jumpIntensity));
                    jumpIntensity *= 0.98f;

                    // Perform bounce sound when leaving floor
                    if (touchesFloor && !audio.isPlaying) {
                        audio.clip = bounce;
                        audio.Play();
                    }
                }
            }
            else if (touchesFloor) {
                // Refill jump when touching the floor and not trying to jump
                jumpDurationRemaining = baseJumpDuration * currentScale;
                jumpIntensity = 270f;
            }
            else {
                // Empty jump "fuel" when in the air and no longer jumping
                jumpDurationRemaining = 0f;
            }

            // Die if fallen off-screen
            //if (transform.position.y < minY) {
            //    if (audio.clip != fall) {
            //        audio.clip = fall;
            //        audio.Play();
            //    }
            //    else if (!audio.isPlaying) {
            //        Universe.BeginLevel();
            //        Destroy(gameObject);
            //    }
            //}
        }
    }

    void Update () {
        if (controlsEnabled) {
            // Inflate / deflate
            //if (Pump.isActive) {
            //    if (Input.GetButton("Inflate")) {
            //        currentScale *= Mathf.Pow(resizeRate, Time.deltaTime);
            //        currentScale = Mathf.Min(currentScale, maxScale);
            //        transform.localScale = baseScale * currentScale;

            //        if (!audio.isPlaying) {
            //            audio.clip = inflate;
            //            audio.Play();
            //        }
            //    }
            //    else if (Input.GetButton("Deflate")) {
            //        currentScale /= Mathf.Pow(resizeRate, Time.deltaTime);
            //        currentScale = Mathf.Max(currentScale, minScale);
            //        transform.localScale = baseScale * currentScale;

            //        if (!audio.isPlaying) {
            //            audio.clip = inflate;
            //            audio.Play();
            //        }
            //    }
            //}
        }

        // Handle pause
        if (Input.GetButtonDown("Pause")) {
            //if (controlsEnabled) {
            //    freeze();
            //    Universe.TogglePause(() => defrost());
            //}
            //else {
            //    Universe.TogglePause();
            //}
        }
    }

    internal void itemCollected () {
        audio.clip = collect;
        audio.Play();
    }

    public void freeze () {
        r2d.bodyType = RigidbodyType2D.Static;
        controlsEnabled = false;
    }

    public void defrost () {
        controlsEnabled = true;
        r2d.bodyType = RigidbodyType2D.Dynamic;
    }

    //public void performLevelFinishAnimation () {
    //    StartCoroutine(levelFinishAnimation());
    //}

    //private IEnumerator levelFinishAnimation () {
    //    freeze();
    //    audio.clip = levelUp;
    //    audio.Play();
    //    WaitForSeconds wait = new WaitForSeconds(0.0166f);
    //    Vector3 scaleS = transform.localScale;
    //    Vector3 scaleE = scaleS * 1.25f;
    //    for (int k = 0; k < 4; k++) {
    //        for (float t = 0f; t < 1f; t += 0.0833f) {
    //            transform.localScale = Vector3.Lerp(scaleS, scaleE, t);
    //            yield return wait;
    //        }
    //        for (float t = 0f; t < 1f; t += 0.0833f) {
    //            transform.localScale = Vector3.Lerp(scaleE, scaleS, t);
    //            yield return wait;
    //        }
    //    }
    //    Universe.FinishLevel();
    //}

    //public void performBurnAnimation () {
    //    StartCoroutine(burnAnimation());
    //}

    //private IEnumerator burnAnimation () {
    //    freeze();
    //    audio.clip = burn;
    //    audio.Play();

    //    WaitForSeconds wait = new WaitForSeconds(0.01665f);
    //    Vector3 iScale = transform.localScale,
    //            fScale = Vector3.zero;

    //    for (float t = 0; t <= 1; t += 0.025f) {
    //        transform.localScale = Vector3.Lerp(iScale, fScale, t);
    //        yield return wait;
    //    }

    //    while (audio.isPlaying)
    //        yield return new WaitForFixedUpdate();

    //    Universe.BeginLevel();
    //}
}
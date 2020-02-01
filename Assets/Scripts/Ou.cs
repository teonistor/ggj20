using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ou : MonoBehaviour {

    private const float maxSlideInTime = 3f;

    private Rigidbody2D r2d;
    private int wallsLayer;
    private int nestLayer;
    private bool isHeld;

    private Player lastPlayer;

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
        else if (!isHeld && other.gameObject.layer == nestLayer && lastPlayer != null)
        {
            if (other.gameObject.name == "BlueNest" && lastPlayer.whichPlayer == Player.WhichPlayer.Blue)
            {
                lastPlayer.score++;
                print(lastPlayer.score);
                Destroy(gameObject);
            }
            else if (other.gameObject.name == "RedNest" && lastPlayer.whichPlayer == Player.WhichPlayer.Red)
            {
                lastPlayer.score++;
                print(lastPlayer.score);
                Destroy(gameObject);

            }
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
    }

    internal Ou GrabHold(Transform holder) {
        if (!isHeld) {
            isHeld = true;
            lastPlayer = holder.GetComponent<Player>();
            r2d.bodyType = RigidbodyType2D.Kinematic;
            r2d.velocity = Vector2.zero;
            transform.parent = holder;
            transform.localPosition = new Vector3(1f, 1f, 0f);
            return this;
        }
        return null;
    }

    internal Ou Throw(Vector2 velo) {
        if (isHeld) {
            isHeld = false;
            transform.parent = null;
            r2d.bodyType = RigidbodyType2D.Dynamic;
            r2d.velocity = velo;
            return null;
        }
        return this;
    }

    internal Ou PutInNest(Transform nest) {
        if (isHeld) {
            // Another bool ??
            transform.parent = nest;
            // TODO Fit around; n.b. Nest is rotated because capsule
            transform.localPosition = new Vector3(0.6f + Random.value * 0.4f, -0.1f + Random.value * 0.2f, 0f);
            return null;
        }
        return this;
    }

    //void OnClossisionEnter2D (Collision2D collision) {
    //    print(collision.gameObject.layer);
    //}
}

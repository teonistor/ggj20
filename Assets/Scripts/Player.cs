using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))][RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour {
    private const int scoreGainOnMatching = 5;
    private const int scoreGainOnNonmatching = 1;
    private static readonly Vector3 carryPosition = new Vector3 (1f, 1f, 0f);
    private static readonly Vector3 hatchPosition = new Vector3(0f, -0.4f, -0.4f);
    private static readonly Vector2 vrum = new Vector2(150f, 0f);

    public int score;
    public float magic_floor_check_value = 4.5f;
    public enum WhichPlayer { Red, Blue }

    [SerializeField] public WhichPlayer whichPlayer;

    [SerializeField]
    private float moveSpeed = 25f,
                  jumpSpeed = 100f,
        baseJumpIntensity = 100f,
        jumpIntensity = 100f,
        baseJumpDuration = 0.4f;
    [SerializeField] private GameObject stunIndicator;
    [SerializeField] private GameObject hatchIndicator;
    [SerializeField] private GameObject hatchling;

    private bool facing_left = false;
    private Rigidbody2D r2d;
    private LayerMask wallsLayer;
    private Animator anim;
    private int ouLayer;
    private int nestLayer;
    private int houseLayer;
    private Ou ouHeld;
    private Hatchling hatchlingHeld;
    private float jumpDurationRemaining;
    //private bool controlsEnabled = true;
    private bool couldHatch;
    private StunIndicator stunIndicatorInProgress;
    private HatchIndicator hatchIndicatorInProgress;

    [SerializeField] private AudioClip hatching, hatched, shoot;
    private new AudioSource audio;

    private Collider2D[] touchCheckBuffer = new Collider2D[5]; // Needed to do non-alloc collision detection (more memory-efficient)

    private bool touchesFloor {
        get {
            Vector3 floorPoint = transform.position;
            floorPoint.y -= transform.localScale.y * magic_floor_check_value;
            //print(whichPlayer + " floor point " + floorPoint);
            return Physics2D.OverlapCircleNonAlloc(floorPoint, 0.2f, touchCheckBuffer, wallsLayer) > 0;
        }
    }

    private bool touchesCeiling {
        get {
            Vector3 ceilingPoint = transform.position;
            ceilingPoint.y += transform.localScale.y * 4.56f;
            return Physics2D.OverlapCircleNonAlloc(ceilingPoint, 0.01f, touchCheckBuffer, wallsLayer) > 0;
        }
    }

    public float minY { get { return -60f; } }

    void Start () {
        r2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallsLayer = LayerMask.GetMask("Walls");
        ouLayer = LayerMask.NameToLayer("Ou");
        nestLayer = LayerMask.NameToLayer("Nest");
        houseLayer = LayerMask.NameToLayer("House");
        ouHeld = null;
        couldHatch = false;
        hatchIndicatorInProgress = null;
        audio = GetComponent<AudioSource>();
    }

    void FixedUpdate () {
      Vector2 velo = r2d.velocity;
        if (stunIndicatorInProgress == null) {
            velo.x = Input.GetAxis(whichPlayer + "Horizontal") * moveSpeed;
        }
            if (velo.x != 0){
              facing_left = velo.x < 0;
            }


            // Vertical movement
            if (touchesCeiling) {
                // Empty jump "fuel" when hitting ceiling
                jumpDurationRemaining = 0f;
                //print(whichPlayer + " touches ceiling");
            }
            else if (stunIndicatorInProgress == null && Input.GetButton(whichPlayer + "Jump")) {
                //print(whichPlayer + " jump " + jumpDurationRemaining);
                // Mid-jump or jump starting now: add vertical force while there still is jump "fuel"
                //print("Jumping for duration " + jumpDurationRemaining);
                if (jumpDurationRemaining > 0f) {
                    jumpDurationRemaining -= Time.fixedDeltaTime;
                    velo.y = jumpSpeed;
                    jumpIntensity *= 0.98f;
                    //print("Velo is :" + velo + "jump speed "+ jumpSpeed);
                }
            }
            else if (touchesFloor) {
                // Refill jump when touching the floor and not trying to jump
                jumpDurationRemaining = baseJumpDuration;
                jumpIntensity = baseJumpIntensity;
                //print(whichPlayer + " touches floor, can jump");
            }
            else {
                // Empty jump "fuel" when in the air and no longer jumping
                //print(whichPlayer + " no touches floor, no jump");
                jumpDurationRemaining = 0f;
            }
            r2d.velocity = velo;
    }

    void Update () {
      Vector2 velo = r2d.velocity;
      velo.x = Input.GetAxis(whichPlayer + "Horizontal") * moveSpeed;

        if (stunIndicatorInProgress == null) {
            if (hatchIndicatorInProgress == null) {
                if (Input.GetButtonDown(whichPlayer + "Hatch") && couldHatch) {
                    ouHeld.transform.localPosition = hatchPosition;
                    hatchIndicatorInProgress = Instantiate(hatchIndicator, transform, false).GetComponent<HatchIndicator>();
                    hatchIndicatorInProgress.player = this;
                    audio.PlayOneShot(hatching);

                } else if (Input.GetButtonDown(whichPlayer + "Fire") && ouHeld != null) {
                    ouHeld.Throw((facing_left ? -1 : 1) * vrum);
                    ouHeld = null;
                    audio.PlayOneShot(shoot);
                }

            }  else {
                if (Input.GetButtonUp(whichPlayer + "Hatch")) {
                    Destroy(hatchIndicatorInProgress.gameObject);
                    hatchIndicatorInProgress = null;
                    ouHeld.transform.localPosition = carryPosition;
                }
            }
        }

        if (Mathf.Abs(velo.y) >= 1) { // if jumping
            anim.SetBool("isJumpingRight", !facing_left);
            anim.SetBool("isJumpingLeft", facing_left);
            anim.SetBool("isWalkingLeft", false);
            anim.SetBool("isWalkingRight", false);
        }
        else {
          anim.SetBool("isJumpingRight", false);
          anim.SetBool("isJumpingLeft", false);

          if (velo.x != 0){
            facing_left = velo.x < 0;
            anim.SetBool("isWalkingLeft", facing_left);
            anim.SetBool("isWalkingRight", !facing_left);
          }
          else {
            anim.SetBool("isWalkingRight", false);
            anim.SetBool("isWalkingLeft", false);
          }
        }


        // Handle pause
        if (Input.GetButtonDown("Cancel")) {
            //if (controlsEnabled) {
            //    freeze();
            //    Universe.TogglePause(() => defrost());
            //}
            //else {
            //    Universe.TogglePause();
            //}
        }


    }

    void OnTriggerEnter2D (Collider2D other) {

        if (ouHeld != null) {
            if (other.gameObject.layer == nestLayer && ouHeld.pairColor == other.GetComponent<Nest>().pairColor) {
                couldHatch = true;
            }
        }

        else if (other.gameObject.layer == ouLayer) {
            Ou otherOu = other.GetComponent<Ou>();

            if (!otherOu.isHeld) {
                if (otherOu.owner == null || otherOu.owner == this) {
                    if (hatchlingHeld == null) {
                        ouHeld = otherOu;
                        ouHeld.GrabHold(this);
                        ouHeld.transform.localPosition = carryPosition;
                    }

                } else {
                    Destroy(other.gameObject);
                    if (stunIndicatorInProgress == null) {
                        stunIndicatorInProgress = Instantiate(stunIndicator).GetComponent<StunIndicator>();
                        stunIndicatorInProgress.player = this;
                    }
                    if (hatchlingHeld != null) {
                        Destroy(hatchlingHeld.gameObject);
                        hatchlingHeld = null;
                    }
                }
            }
        }

        if (hatchlingHeld != null) {
            if (other.gameObject.layer == houseLayer) {
                if (other.GetComponent<House>().pairColor == hatchlingHeld.pairColor) {
                    score += scoreGainOnMatching;
                } else {
                    score += scoreGainOnNonmatching;
                }
                Destroy(hatchlingHeld.gameObject);
                hatchlingHeld = null;
            }
        }
    }

    void OnTriggerExit2D (Collider2D other) {
        if (other.gameObject.layer == nestLayer) {
            couldHatch = false;
            if (hatchIndicatorInProgress != null) {
                Destroy(hatchIndicatorInProgress.gameObject);
                hatchIndicatorInProgress = null;
                ouHeld.transform.localPosition = carryPosition;
            }
        }
    }

    internal void StunOver() {
        stunIndicatorInProgress = null;
    }

    internal void HatcingComplete() {
        hatchIndicatorInProgress = null;
        Destroy(ouHeld.gameObject);
        ouHeld = null;
        hatchlingHeld = Instantiate(hatchling, transform).GetComponent<Hatchling>();
        hatchlingHeld.transform.localPosition = carryPosition;
        couldHatch = false;
        audio.PlayOneShot(hatched);
    }

    //internal void itemCollected () {
        //audio.clip = collect;
        //audio.Play();
    //}

    //public void freeze () {
    //    r2d.bodyType = RigidbodyType2D.Static;
    //    controlsEnabled = false;
    //}

    //public void defrost () {
    //    controlsEnabled = true;
    //    r2d.bodyType = RigidbodyType2D.Dynamic;
    //}

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

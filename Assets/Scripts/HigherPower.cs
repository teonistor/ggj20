using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HigherPower : MonoBehaviour {

    private static readonly Vector3 leftmost = new Vector3(-77f, 43f, 0f);
    private static readonly Vector3 rightmost = new Vector3(77f, 43f, 0f);
    
    [SerializeField] private GameObject[] ous;
 
    IEnumerator Start() {
        while (true) {
            GameObject newOu = Instantiate(ous[Random.Range(0,ous.Length)]);
            newOu.GetComponent<Ou>().SlideIn(leftmost,rightmost);

            yield return new WaitForSeconds(5f);
        }
    }

    
    void Update() {
        
    }
}

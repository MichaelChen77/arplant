using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
    float timer;
    const float time = 1;
    public bool rotated = false;
	
	// Update is called once per frame
	void Update () {
        if (rotated)
        {
            transform.Rotate(Vector3.up, 1);
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = time;
                //GetComponent<Outline>().enabled = !GetComponent<Outline>().enabled;
            }
        }
    }
}

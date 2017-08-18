using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour {

    public float changedTime = 1f;

    float duration = 0;
    Vector3 pos;
    Vector3 prePos;

    private void Start()
    {
        SetRandomChange();
    }

    // Update is called once per frame
    void Update () {
        if (duration < changedTime)
        {
            duration += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(prePos, pos, duration / changedTime);
            if (duration > changedTime)
            {
                duration = 0;
                SetRandomChange();
            }
        }
	}

    void SetRandomChange()
    {
        float fx = Random.Range(-0.5f, 0.5f);
        float fy = Random.Range(-0.1f, 0.1f);
        float fz = Random.Range(-0.5f, 0.5f);
        prePos = transform.localPosition;
        pos = prePos + new Vector3(fx, fy, fz);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActived {
    bool SetActive(bool toNext);
    void DeActive();
}

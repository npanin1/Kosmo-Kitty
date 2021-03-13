using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Coins_collect : MonoBehaviour
{

    public static int crystallCount;
    private Text crystallCounter;

    void Start()
    {
        crystallCounter = GetComponent<Text>();
        crystallCount = 0;
    }

    void Update()
    {
        crystallCounter.text = "X " + crystallCount;
    }
}

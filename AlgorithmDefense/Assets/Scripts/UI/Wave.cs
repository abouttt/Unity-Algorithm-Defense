using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Wave : MonoBehaviour
{
    [SerializeField]
    private Text WaveText;
    private int _count;


    private void Start()
    {
        _count = 1;
        WaveText.text = "Wave " + _count.ToString();
    }


    public void WaveCountPlus()
    {
        _count++;
        WaveText.text = "Wave "+_count.ToString();

        //if 6����� ����

    }



}
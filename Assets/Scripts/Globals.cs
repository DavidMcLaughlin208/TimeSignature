using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
 
    private bool rewinding = false;

    public int secondsOfRewind = 5;

    public int targetFramerate = 60;

    private int rewindingSlowmoFrameCount = 0;
    private int rewindingThreshold = 1;

    public float localTimescale = 1f;

    private float timescaleAccumulator = 0f;
    private float accumulatorThreshold = 1f;

    private bool isPopFrame = false;

    public bool paused = false;

    void Awake() {
        Application.targetFrameRate = targetFramerate;
        prefabManager = GameObject.Find("PrefabManager").GetComponent<PrefabManager>();
    }

    public PrefabManager prefabManager;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rewindingSlowmoFrameCount++;
        if (rewinding)
        {
            
            timescaleAccumulator += localTimescale;
            timescaleAccumulator = Mathf.Round(timescaleAccumulator * 100)/100.0f;
            if (timescaleAccumulator >= accumulatorThreshold)
            {
                accumulatorThreshold += 1;
                isPopFrame = true;
            } else
            {
                isPopFrame = false;
            }
                     
            
        }
    }

    public bool getIsPopFrame()
    {
        return isPopFrame;
    }

    internal void togglePause()
    {
        setRewindingFalse();
        paused = !paused;
    }

    public float rewindInterpolationFactor()
    {
        if (isPopFrame)
        {
            return 1;
        }
        else
        {
            return timescaleAccumulator % 1;
        }       
    }

    public void setTimescale(float scale)
    {
        localTimescale = scale;
        rewindingThreshold = (int) Mathf.Round(1 / localTimescale);
    }

    public bool getRewinding()
    {
        return rewinding;
    }

    public void setRewindingTrue()
    {
        if (rewinding)
        {
            return;
        } else
        {
            paused = false;
            timescaleAccumulator = 0f;
            accumulatorThreshold = 0f;
            setTimescale(0.1f);
            //float timescale = Random.Range(0.01f, 0.7f);
            //timescale = Mathf.Round(timescale * 100f) / 100.0f;
            //Debug.Log(timescale);
            //setTimescale(timescale);
            rewinding = true;
        }
    }

    public void setRewindingFalse()
    {
        if (!rewinding)
        {
            return;
        } else
        {
            paused = true;
            timescaleAccumulator = 0f;
            accumulatorThreshold = 0f;
            rewinding = false;
            setTimescale(1f);
        }
    }
}

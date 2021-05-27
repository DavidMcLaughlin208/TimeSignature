using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Globals : MonoBehaviour
{

    public IntReactiveProperty rewindCount = new IntReactiveProperty(0);

    public int secondsOfRewind = 5;

    public int targetFramerate = 60;

    public FloatReactiveProperty localTimescale = new FloatReactiveProperty(1f);

    private float timescaleAccumulator = 0f;
    private float accumulatorThreshold = 1f;

    private bool isPopFrame = false;

    public IntReactiveProperty timeState = new IntReactiveProperty((int) TimeState.PLAY);

    

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
        switch (timeState.Value)
        {
            case (int) TimeState.PLAY:
                rewindCount.Value++;
                if (rewindCount.Value > secondsOfRewind * targetFramerate)
                {
                    rewindCount.Value = secondsOfRewind * targetFramerate;
                }
                runTimescaleAccumulation();
                break;
            case (int) TimeState.PAUSE:
                break;
            case (int) TimeState.REWIND:
                runTimescaleAccumulation();
                if (rewindCount.Value <= 0)
                {
                    rewindCount.Value = 0;
                    setRewindingFalse();
                }
                break;
        }
    }

    private void runTimescaleAccumulation()
    {
        timescaleAccumulator += localTimescale.Value;
        timescaleAccumulator = Mathf.Round(timescaleAccumulator * 100) / 100.0f;
        if (timescaleAccumulator >= accumulatorThreshold)
        {
            if (timeState.Value == (int)TimeState.PLAY)
            {
                rewindCount.Value++;
            } else if (timeState.Value == (int) TimeState.REWIND)
            {
                rewindCount.Value--;
            }
            accumulatorThreshold += 1;
            isPopFrame = true;
        }
        else
        {
            isPopFrame = false;
        }
    }

    public bool getIsPopFrame()
    {
        return isPopFrame;
    }

    internal void togglePause()
    {
        setRewindingFalse();
        if (timeState.Value != (int) TimeState.PAUSE)
        {
            timeState.Value = (int)TimeState.PAUSE;
        } else
        {
            timeState.Value = (int)TimeState.PLAY;
        }
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
        timescaleAccumulator = 0f;
        accumulatorThreshold = 1f;
        if (scale > 1f)
        {
            localTimescale.Value = 1f;
        } else if (scale < .05f)
        {
            localTimescale.Value = .1f;
        } else { 
            localTimescale.Value = scale;
        }
    }

    public bool isRewinding()
    {
        return timeState.Value == (int) TimeState.REWIND;
    }

    public void setRewindingTrue()
    {
        if (timeState.Value == (int) TimeState.REWIND)
        {
            return;
        } else
        {
            timescaleAccumulator = 0f;
            accumulatorThreshold = 0f;
            setTimescale(0.25f);
            //float timescale = Random.Range(0.01f, 0.7f);
            //timescale = Mathf.Round(timescale * 100f) / 100.0f;
            //Debug.Log(timescale);
            //setTimescale(timescale);
            timeState.Value = (int) TimeState.REWIND;
        }
    }

    public void setRewindingFalse()
    {
        if (timeState.Value != (int) TimeState.REWIND)
        {
            return;
        } else
        {
            timescaleAccumulator = 0f;
            accumulatorThreshold = 0f;
            setTimescale(1f);
            timeState.Value = (int) TimeState.PLAY;
        }
    }
}

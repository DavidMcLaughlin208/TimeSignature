using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Globals : MonoBehaviour
{

    public IntReactiveProperty rewindCount = new IntReactiveProperty(0);
 
    private bool rewinding = false;

    public int secondsOfRewind = 5;

    public int targetFramerate = 60;

    public FloatReactiveProperty localTimescale = new FloatReactiveProperty(1f);

    private float timescaleAccumulator = 0f;
    private float accumulatorThreshold = 1f;

    private bool isPopFrame = false;

    public BoolReactiveProperty paused = new BoolReactiveProperty(false);

    

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
        if (!paused.Value)
        {
            rewindCount.Value++;
            if (rewindCount.Value <= 0)
            {
                setRewindingFalse();
            }
            else if (rewindCount.Value > secondsOfRewind * targetFramerate)
            {
                rewindCount.Value = secondsOfRewind * targetFramerate;
            }
            timescaleAccumulator += localTimescale.Value;
            timescaleAccumulator = Mathf.Round(timescaleAccumulator * 100) / 100.0f;
            if (timescaleAccumulator >= accumulatorThreshold)
            {
                accumulatorThreshold += 1;
                rewindCount.Value--;
                isPopFrame = true;
            }
            else
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
        paused.Value = !paused.Value;
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
        } else if (scale < .1f)
        {
            localTimescale.Value = .1f;
        } else { 
            localTimescale.Value = scale;
        }
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
            paused.Value = false;
            timescaleAccumulator = 0f;
            accumulatorThreshold = 0f;
            setTimescale(0.3f);
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
            timescaleAccumulator = 0f;
            accumulatorThreshold = 0f;
            rewinding = false;
            setTimescale(1f);
        }
    }
}

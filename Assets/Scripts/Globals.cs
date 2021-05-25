using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Globals : MonoBehaviour
{

    public int rewindCount = 0;
 
    private bool rewinding = false;

    public int secondsOfRewind = 5;

    public int targetFramerate = 60;

    public float localTimescale = 1f;

    private float timescaleAccumulator = 0f;
    private float accumulatorThreshold = 1f;

    private bool isPopFrame = false;

    public bool paused = false;

    public Slider rewindSlider;

    void Awake() {
        Application.targetFrameRate = targetFramerate;
        prefabManager = GameObject.Find("PrefabManager").GetComponent<PrefabManager>();
        rewindSlider = GameObject.Find("Canvas/RewindSlider").GetComponent<Slider>();
    }

    public PrefabManager prefabManager;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rewindSlider.value = rewindCount;

        if (rewinding)
        {

            timescaleAccumulator += localTimescale;
            timescaleAccumulator = Mathf.Round(timescaleAccumulator * 100) / 100.0f;
            if (timescaleAccumulator >= accumulatorThreshold)
            {
                accumulatorThreshold += 1;
                rewindCount--;
                isPopFrame = true;
            }
            else
            {
                isPopFrame = false;
            }


        }
        else
        {
            if (!paused)
            {
                rewindCount++;
                if (rewindCount <= 0)
                {
                    setRewindingFalse();
                }
                else if (rewindCount > secondsOfRewind * targetFramerate)
                {
                    rewindCount = secondsOfRewind * targetFramerate;
                }
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
        if (scale > 1f)
        {
            localTimescale = 1f;
        } else if (scale < .1f)
        {
            localTimescale = .1f;
        } else { 
            localTimescale = scale;
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
            paused = false;
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

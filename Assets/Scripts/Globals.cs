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

    private bool isPopFrame = false;

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
            if (timescaleAccumulator % 1 == 0f)
            {
                Debug.Log("Setting Is Pop Frame True");
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

    public float rewindInterpolationFactor()
    {
        if (timescaleAccumulator > 0f && timescaleAccumulator % 1 == 0)
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

    public bool getIsRecordingFrame()
    {
        if (rewindingSlowmoFrameCount % rewindingThreshold == 0)
        {
            rewindingSlowmoFrameCount = 0;
            return true;
        } else
        {
            return false;
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
            rewindingSlowmoFrameCount = 0;
            setTimescale(0.05f);
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
            rewinding = false;
            setTimescale(1f);
            rewindingSlowmoFrameCount = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIHandler : MonoBehaviour
{
    public Slider rewindSlider;
    public Globals globals;
    public Text timescaleText;

    void Awake()
    {
        globals = GameObject.Find("Globals").GetComponent<Globals>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rewindSlider = GameObject.Find("Canvas/RewindSlider").GetComponent<Slider>();
        timescaleText = GameObject.Find("Canvas/TimeScale").GetComponent<Text>();
        globals.rewindCount.Subscribe(val =>
        {
            rewindSlider.value = (int) val;
        });
        globals.localTimescale.Subscribe(val =>
        {
            timescaleText.text = string.Format("{0:N2}x", val);
        });
        globals.paused.Subscribe(val =>
        {
            if (val)
            {
                timescaleText.text = "0x";
            } else
            {
                timescaleText.text = string.Format("{0:N2}x", globals.localTimescale.Value);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

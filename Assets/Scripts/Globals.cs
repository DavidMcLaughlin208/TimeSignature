using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
 
    public bool rewinding = false;
    public int secondsOfRewind = 5;
    public int targetFramerate = 60;

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
        
    }
}

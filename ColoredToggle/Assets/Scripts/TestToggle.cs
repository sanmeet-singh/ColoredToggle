using System.Collections;
using System.Collections.Generic;
using ColoredToggleUI;
using UnityEngine;

public class TestToggle : MonoBehaviour
{

    public ColoredToggle coloredToggle;

    void Start()
    {
        //coloredToggle.onValueChanged.AddListener(delegate
        //{
        //    Ok(coloredToggle);
        //});
    }

    public void Ok(ColoredToggle toggle)
    {
        Debug.Log("ColoredToggle : " + toggle.isOn);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

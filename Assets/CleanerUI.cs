using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CleanerUI : MonoBehaviour
{

    public GeometryEditorUI geometryEditorUI;
    public TMP_InputField text;
    public bool setLayer1;
    public bool setLayer2;
    public bool setLayer3;
    public GameObject startButton;
    public int targetHeight;

    public Image imageL1;
    public Image imageL2;
    public Image imageL3;

    public Sprite layerActive;
    public Sprite layerInActive;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        startButton.SetActive((setLayer1 || setLayer2 || setLayer3) && Int32.TryParse(text.GetComponent<TMP_InputField>().text, out targetHeight));
    }
    public void Clean()
    {
        geometryEditorUI.controller.CleanUnusedSectionHeights(targetHeight, setLayer1,setLayer2,setLayer3);
    }
    public void FlipSetLayer1()
    {
        setLayer1 = !setLayer1;
        if (setLayer1)
        {
            imageL1.sprite = layerActive;
        } else
        {
            imageL1.sprite = layerInActive;
        }
    }
    public void FlipSetLayer2()
    {
        setLayer2 = !setLayer2;
        if (setLayer2)
        {
            imageL2.sprite = layerActive;
        }
        else
        {
            imageL2.sprite = layerInActive;
        }
    } 
    public void FlipSetLayer3()
    {
        setLayer3 = !setLayer3;
        if (setLayer3)
        {
            imageL3.sprite = layerActive;
        }
        else
        {
            imageL3.sprite = layerInActive;
        }
    }
}

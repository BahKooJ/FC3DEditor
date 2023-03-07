using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputGuideSwitch : MonoBehaviour
{
    public List<GameObject> help;
    public TextMeshProUGUI dynamicHelp;
    public Main main;

    bool isDisplay;

    // Start is called before the first frame update
    void Start()
    {
        DisplayHelp(false);
        isDisplay = false;
        main = FindObjectOfType<Main>();
        main.dynamicInputsDisplay = this;
        UpdateDynamicDisplay(main.dynamicInputsDefault);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SwitchMenu();
        }
    }

    public void SwitchMenu()
    {
        isDisplay = !isDisplay;
        DisplayHelp(isDisplay);
    }

    public void DisplayHelp(bool active)
    {
        foreach (GameObject go in help)
        {
            go.SetActive(active);
        }
    }
    public void UpdateDynamicDisplay(string newValue) {
        dynamicHelp.SetText(newValue);
    }
}

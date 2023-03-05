using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGuideSwitch : MonoBehaviour
{
    public List<GameObject> menu1;
    public List<GameObject> menu2;

    bool isInMenu1;

    // Start is called before the first frame update
    void Start()
    {
        UpdateMenu1(true);
        UpdateMenu2(false);
        isInMenu1 = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchMenu()
    {
        isInMenu1 = !isInMenu1;
        UpdateMenu1(isInMenu1);
        UpdateMenu2(!isInMenu1);
    }

    public void UpdateMenu1(bool active)
    {
        foreach (GameObject go in menu1)
        {
            go.SetActive(active);
        }
    }

    public void UpdateMenu2(bool active)
    {
        foreach (GameObject go in menu2)
        {
            go.SetActive(active);
        }
    }
}

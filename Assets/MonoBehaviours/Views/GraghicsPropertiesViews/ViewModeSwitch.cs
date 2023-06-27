using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModeSwitch : MonoBehaviour
{
    public GameObject tab0_TextureManager;
    public GameObject tab1_CoordinateManager;
    public GameObject tab2_TexturElementManager;
    public GameObject tab3_TexturCombiner;
    public GameObject tilePrewiew;

    public GraphicsPropertiesView graphicsPropertiesView;
    // Start is called before the first frame update
    void Start()
    {
        setToTab3();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setToTab0()
    {
        tab0_TextureManager.SetActive(true);
        tab1_CoordinateManager.SetActive(false);
        tab2_TexturElementManager.SetActive(false);
        tab3_TexturCombiner.SetActive(false);
        tilePrewiew.SetActive(false);
    }
    public void setToTab1()
    {
        tab0_TextureManager.SetActive(false);
        tab1_CoordinateManager.SetActive(true);
        tab2_TexturElementManager.SetActive(false);
        tab3_TexturCombiner.SetActive(false);
        tilePrewiew.SetActive(true);
        graphicsPropertiesView.UpdateDynamicTextureOffsets();
    }
    public void setToTab2()
    {
        tab0_TextureManager.SetActive(false);
        tab1_CoordinateManager.SetActive(false);
        tab2_TexturElementManager.SetActive(true);
        tab3_TexturCombiner.SetActive(false);
        tilePrewiew.SetActive(false);
    }
    public void setToTab3()
    {
        tab0_TextureManager.SetActive(false);
        tab1_CoordinateManager.SetActive(false);
        tab2_TexturElementManager.SetActive(false);
        tab3_TexturCombiner.SetActive(true);
        tilePrewiew.SetActive(true);
    }
}

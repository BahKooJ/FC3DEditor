using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FileSelection : MonoBehaviour
{
    public TMP_InputField TMP_IF;
    public string fileName;
    // Start is called before the first frame update
    void Start()
    {
        fileName = PlayerPrefs.GetString("SrcFileName");
        if (fileName == null)
        {
            Debug.Log("No SrcFileName was found. Loading default config");
            PlayerPrefs.SetString("SrcFileName",Application.dataPath);
            fileName = PlayerPrefs.GetString("SrcFileName");
        }
        UpdateFileData();
        TMP_IF.text =(fileName);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenFileSelector()
    {
        fileName = EditorUtility.OpenFilePanel("Select FC mission file",fileName,"");
        UpdateFileData();
        TMP_IF.text = (fileName);
    }
    public void UpdateFileData()
    {
        StaticFileDate.SRC_FILE_NAME = fileName;
    }

    public void StartMainScene()
    {
        (fileName) = TMP_IF.text;
        PlayerPrefs.SetString("SrcFileName", fileName);
        Debug.Log("Setting default file to \"" + PlayerPrefs.GetString("SrcFileName") + "\"");
        UpdateFileData();
        SceneManager.LoadScene("Main Scene");
    }
}

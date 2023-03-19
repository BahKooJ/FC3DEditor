using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreenView : MonoBehaviour {

    public FileManagerMain main;

    public void OnClickOpen() {
        main.OpenFile();
    }

}

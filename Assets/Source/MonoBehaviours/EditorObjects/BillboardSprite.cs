

using UnityEngine;

public class BillboardSprite : MonoBehaviour {

    void Update() {
        
        transform.LookAt(Camera.main.transform);

    }

}
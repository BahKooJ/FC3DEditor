

using FCopParser;
using UnityEngine;

public class NavNodePoint : MonoBehaviour {

    public NavNode node;

    void Start() {

        transform.position = new Vector3(node.x / 32f,0f,-(node.y / 32f));

    }

}
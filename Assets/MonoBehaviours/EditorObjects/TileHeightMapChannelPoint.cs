

using FCopParser;
using UnityEngine;

public class TileHeightMapChannelPoint : MonoBehaviour {

    public HeightPoints heightPoints;
    public int channel;
    public VertexPosition corner;
    public LevelMesh section;

    public bool isSelected = false;

    public BoxCollider boxCollider;
    Material material;

    void Start() {

        boxCollider = GetComponent<BoxCollider>();

        material = GetComponent<MeshRenderer>().material;

        switch (channel) {
            case 1:
                material.color = Color.blue;
                break;
            case 2:
                material.color = Color.green;
                break;
            case 3:
                material.color = Color.red;
                break;
        }


    }

}
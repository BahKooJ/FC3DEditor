

using FCopParser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ObjectDebug : MonoBehaviour {

    public GameObject cube;

    public GameObject meshPrefab;

    IFFParser iffFile = new IFFParser(File.ReadAllBytes("MissionFiles/Mp"));
    IFFFileManager fileManager;

    List<FCopObject> objects = new();

    List<GameObject> cubes = new();

    ObjectMesh mesh;

    void Start() {

        fileManager = iffFile.parsedData;

        var rawObjectFiles = fileManager.files.Where(file => {

            return file.dataFourCC == "Cobj";

        }).ToList();

        foreach (var rawFile in rawObjectFiles) {
            objects.Add(new FCopObject(rawFile));
        }

        ShowObjectVerticies(objects[0]);


    }

    int objIndex = 0;

    void Update() {

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            DestroyCubes();
            objIndex++;
            ShowObjectVerticies(objects[objIndex]);
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            DestroyCubes();
            objIndex--;
            ShowObjectVerticies(objects[objIndex]);

        }

    }

    void DestroyCubes() {

        foreach (var gameCube in cubes) {
            Destroy(gameCube.gameObject);
        }

        cubes.Clear();

        Destroy(mesh.gameObject);

    }

    void ShowObjectVerticies(FCopObject obj) {

        var gameObj = Instantiate(meshPrefab);

        var script = gameObj.GetComponent<ObjectMesh>();

        script.fCopObject = obj;

        mesh = script;

    }


}


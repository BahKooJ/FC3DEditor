

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParameterNodeView : MonoBehaviour {

    // - Unity Prefabs -
    public GameObject paddingPrefab;
    public GameObject parameterTextFab;

    // - Unity Refs -
    public Transform endBracket;

    // - Parameters -
    [HideInInspector]
    public List<ExpressionNodeView> expressionNodes = new();

    void Start() {

        void Pad() {
            var padObj = Instantiate(paddingPrefab);
            padObj.transform.SetParent(transform, false);
            padObj.SetActive(true);
        }

        foreach (var node in expressionNodes) {

            if (node.parameterNode.parameterName != "") {

                var textObj = Instantiate(parameterTextFab, transform, false);
                textObj.GetComponent<TMP_Text>().text = node.parameterNode.parameterName + ": ";
                textObj.SetActive(true);

            }

            Pad();

            node.transform.SetParent(transform, false);
            node.Init();

            Pad();

        }

        // Removes the extra pad
        Destroy(transform.GetChild(transform.childCount - 1).gameObject);

        endBracket.transform.SetSiblingIndex(transform.childCount - 1);

    }

}
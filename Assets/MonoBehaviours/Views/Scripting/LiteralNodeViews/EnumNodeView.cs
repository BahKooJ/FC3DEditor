
using UnityEngine;
using FCopParser;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using static FCopParser.ActorMethodNode;

public class EnumNodeView : ExpressionNodeView {

    // - Unity Refs -
    public TMP_Dropdown caseDropDown;

    Type enumType;
    string[] cases;
    Array values;

    LiteralNode literalNode;

    bool refuseCallback = false;
    public override void Init() {

        enumType = ((EnumParameterNode)parameterNode).enumType;
        literalNode = (LiteralNode)parameterNode.scriptNode;

        refuseCallback = true;

        if (enumType == typeof(ActorMethod)) {

        }

        refuseCallback = false;

    }

    void StandardInit() {

        cases = Enum.GetNames(enumType);
        values = Enum.GetValues(enumType);

        caseDropDown.ClearOptions();

        var value = 0;
        foreach (var c in cases) {

            caseDropDown.AddOptions(new List<string>() { Utils.AddSpacesToString(c) });

            if (Enum.GetName(enumType, literalNode.value) == c) {
                caseDropDown.value = value;
            }

            value++;
        }

    }

    void ActorMethodInit() {

        var parentNode = (ActorMethodNode)parameterNode.parent;

        var actorRefNode = parentNode.GetActorRefProperty();

        if (actorRefNode != null) {
            StandardInit();
            return;
        }

    }

    public void OnChange() {

        if (refuseCallback) return;

        literalNode.value = (int)values.GetValue(caseDropDown.value);

        if (((EnumParameterNode)parameterNode).affectsParameters) {
            currentLine.RebuildScriptNode();
        }

    }

}

using UnityEngine;
using FCopParser;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using static FCopParser.ActorMethodNode;
using System.Linq;
using System.Collections;
using UnityEngine.UIElements;

public class EnumNodeView : ExpressionNodeView {

    // - Unity Refs -
    public TMP_Dropdown caseDropDown;

    Type enumType;
    string[] cases;
    Array values;

    LiteralNode literalNode;

    int GetNodeValue() {

        if (parameterNode.bitCount != BitCount.NA) {
            var bitField = new BitArray(new byte[] { (byte)literalNode.value });
            var bits = Utils.CopyBitsOfRange(bitField, parameterNode.bitPosition, parameterNode.bitPosition + (byte)parameterNode.bitCount);
            return Utils.BitsToInt(bits);
        }
        else {
            return literalNode.value;
        }

    }

    void SetNodeValue(int value) {

        if (parameterNode.bitCount != BitCount.NA) {
            var shiftedValue = value << parameterNode.bitPosition;

            var andBits = ~(((int)Math.Pow(2, (int)parameterNode.bitCount) - 1) << parameterNode.bitPosition);

            literalNode.value = (literalNode.value & andBits) + shiftedValue;
        }
        else {
            literalNode.value = value;
        }

    }

    bool refuseCallback = false;
    public override void Init() {

        enumType = ((EnumParameterNode)parameterNode).enumType;
        literalNode = (LiteralNode)parameterNode.scriptNode;

        refuseCallback = true;

        if (enumType == typeof(ActorMethod)) {
            ActorMethodInit();
        }
        else {
            StandardInit();
        }

        refuseCallback = false;

    }

    void AddCases() {

        caseDropDown.ClearOptions();

        var value = 0;
        foreach (var c in cases) {

            caseDropDown.AddOptions(new List<string>() { Utils.AddSpacesToString(c) });

            if (Enum.GetName(enumType, GetNodeValue()) == c) {
                caseDropDown.value = value;
            }

            value++;
        }

    }

    void StandardInit() {

        cases = Enum.GetNames(enumType);
        values = Enum.GetValues(enumType);

        AddCases();

    }

    void ActorMethodInit() {

        var parentNode = (ActorMethodNode)parameterNode.parent;

        var actorRefNode = parentNode.GetActorRefProperty();

        if (actorRefNode == null) {
            StandardInit();
            return;
        }

        var id = ((LiteralNode)actorRefNode.scriptNode).value;
        var main = FindAnyObjectByType<Main>();

        List<Type> types = new();
        if (actorRefNode.dataType == ScriptDataType.Actor) {
            types = main.level.sceneActors.FindAllDerivedTypesFromActorBehavior(id);
        }
        else if (actorRefNode.dataType == ScriptDataType.Group) {
            types = main.level.sceneActors.FindAllDerviedTypesFromGroup(id);
        }
        else if (actorRefNode.dataType == ScriptDataType.Team) {
            types = main.level.sceneActors.FindAllDerviedTypesFromTeam(id);
        }

        if (types.Count == 0) {
            StandardInit();
            return;
        }

        var methods = new List<ActorMethod>();

        foreach (var type in types) {

            if (ActorMethodNode.methods.TryGetValue(type, out var _methods)) {
                methods.AddRange(_methods);
            }

        }

        if (!Enum.IsDefined(typeof(ActorMethod), GetNodeValue())) {
            Debug.LogWarning("Method " + GetNodeValue() + " is not defined in type: " + types[0].ToString());
        }

        if (!methods.Contains((ActorMethod)GetNodeValue())) {
            StandardInit();
            return;
        }

        var cases = new List<string>();

        foreach (var method in methods) {
            cases.Add(method.ToString());
        }

        this.cases = cases.ToArray();
        values = methods.ToArray();

        AddCases();

    }

    public void OnChange() {

        if (refuseCallback) return;

        SetNodeValue((int)values.GetValue(caseDropDown.value));

        if (((EnumParameterNode)parameterNode).affectsParameters) {
            currentLine.RebuildScriptNode();
        }

    }

}
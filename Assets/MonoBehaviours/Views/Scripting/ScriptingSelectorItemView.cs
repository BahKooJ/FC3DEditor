
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FCopParser;
using System.Linq;

public class ScriptingSelectorItemView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text nameText;
    public TMP_InputField nameInput;
    public Image background;
    public ContextMenuHandler contextMenu;

    // - Parameter -
    [HideInInspector]
    public ScriptingPanelView view;
    [HideInInspector]
    public FCopScript script;
    [HideInInspector]
    public int id;

    void Start() {

        nameText.text = script.name;

        contextMenu.items = new() {
            ("Rename", Rename),
            ("Delete", Delete)
        };

    }

    public void Rename() {
        nameInput.text = script.name;
        nameInput.gameObject.SetActive(true);
        nameText.gameObject.SetActive(false);
        nameInput.Select();
    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Script", "Are you sure you would like to delete this script?" +
            "Actors that depend on this script may no longer work properly", () => {

                view.level.scripting.rpns.RemoveScript(id);


                view.Refresh();
                view.scriptingWindow.Clear();

                return true;
            });

    }

    public void Unselect() {

        background.color = Main.mainColor;

    }

    // - Unity Callbacks -
    public void OnStartNameType() {

        Main.ignoreAllInputs = true;

    }

    public void OnEndNameType() {

        Main.ignoreAllInputs = false;

        if (nameInput.text == "") {
            
            script.name = "Script " + id;

        }
        else {

            var rpnsValues = view.level.scripting.rpns.codeByOffset.Values.ToList();

            var existingName = rpnsValues.FirstOrDefault(fcs => fcs.name == nameInput.text);

            if (existingName != null) {
                //QuickLogHandler.Log("Name is already in use!", LogSeverity.Info);
            }
            else {
                script.name = nameInput.text;
            }

        }

        nameInput.text = script.name;
        nameText.text = script.name;
        nameInput.gameObject.SetActive(false);
        nameText.gameObject.SetActive(true);

    }

    public void OnClick() {

        if (view.isScriptTab) {
            view.SelectScript(id);
        }
        else {
            view.SelectFunc(view.level.scripting.functionParser.functions[id]);
        }

        background.color = Main.selectedColor;

    }

    public void OnReceiverDrag() {

        if (Main.draggingElement.TryGetComponent<ScriptingSelectorItemView>(out var viewItem)) {

            var indexOfDragged = view.level.scripting.rpns.code.IndexOf(viewItem.script);
            var indexOfThis = view.level.scripting.rpns.code.IndexOf(script);

            view.ReOrderScript(indexOfDragged, indexOfThis);

            if (indexOfThis > indexOfDragged) {
                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            }
            else {
                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());
            }

        }

    }

}
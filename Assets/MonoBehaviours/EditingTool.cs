

public class EditingTool {

    public string name;
    public bool selectionChanges = false;

    public EditingTool(string name) {
        this.name = name;
    }

    public EditingTool(string name, bool selectionChanges) : this(name) {
        this.selectionChanges = selectionChanges;
    }

}
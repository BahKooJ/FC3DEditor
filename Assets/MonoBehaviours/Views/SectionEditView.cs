

using UnityEngine;

public class SectionEditView : MonoBehaviour {

    public SectionEditMode controller;

    public void OnClickCopySectionDataButton() {
        controller.CopySectionData();
    }

    public void OnClickPasteSectionDataButton() {
        controller.PasteSectionData();
    }

    public void OnClickMirrorSectionVerticallyButton() {
        controller.MirrorSectionVertically();
    }

    public void OnClickMirrorSectionHorizontallyButton() {
        controller.MirrorSectionHorizontally();
    }

    public void OnClickMirrorSectionDiagonallyButton() {
        controller.MirrorSectionDiagonally();
    }

}
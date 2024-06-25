

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
        controller.MirrorSectionHorizontally();
    }

    public void OnClickMirrorSectionHorizontallyButton() {
        controller.MirrorSectionVertically();
    }

    public void OnClickMirrorSectionDiagonallyButton() {
        controller.MirrorSectionDiagonally();
    }

    public void OnClickRemoveShadersFromSectionButton() {
        controller.RemoveShadersFromSection();
    }

    public void OnClickRotateSectionClockwise() {
        controller.RotateSectionClockwise();
    }

    public void OnClickRotateSectionCounterClockwise() {
        controller.RotateSectionCounterClockwise();
    }

}
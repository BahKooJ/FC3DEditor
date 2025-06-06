

using FCopParser;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TexturePropertiesView : MonoBehaviour {

    // - Unity Refs -
    public Image texturePreview;

    // - Parameters -
    public FCopTexture texture;
    [HideInInspector]
    public Main main;

    private void Start() {
        Refresh();
    }

    void Refresh() {

        texturePreview.sprite = main.bmpTextures[texture.DataID - 1];

    }

    public void ExportBitmap() {

        OpenFileWindowUtil.SaveFile("FCEAssets/Textures", "Bitmap Texture", path => {

            var bitmap = texture.BitmapWithHeader();

            File.WriteAllBytes(Utils.RemoveExtensionFromFileName(path) + ".bmp", bitmap);

        });

    }

    public void ExportColorPalette() {

        OpenFileWindowUtil.SaveFile("FCEAssets/Textures/Color Palettes", "Color Palette", path => {

            var data = texture.CbmpColorPaletteData();

            File.WriteAllBytes(Utils.RemoveExtensionFromFileName(path) + ".plut", data);

        });

    }

    public void ExportCbmp() {

        OpenFileWindowUtil.SaveFile("FCEAssets/Textures/Cbmp", "Cbmp", path => {

            var data = texture.rawFile.data;

            File.WriteAllBytes(Utils.RemoveExtensionFromFileName(path) + ".Cbmp", data.ToArray());

        });

    }

    public void ImportTexture() {

        OpenFileWindowUtil.OpenFile("FCEAssets/Textures", "", path => {

            try {

                texture.ImportBMP(File.ReadAllBytes(path));

                main.RefreshTextures();

                foreach (var section in main.sectionMeshes) {
                    section.RefreshTexture();
                    section.RefreshMesh();
                }

                Refresh();

            }
            catch {

                DialogWindowUtil.Dialog("Invalid File", "Please select a valid bitmap file. The bitmap file must be in XRGB555 format.");

            }

        });

    }

    public void ImportColorPalette() {

        OpenFileWindowUtil.OpenFile("FCEAssets/Textures/Color Palettes", "", path => {

            try {
                texture.ImportColorPaletteData(File.ReadAllBytes(path));
            }
            catch {
                DialogWindowUtil.Dialog("Invalid File", "Please select a valid color palette file.");
            }

        });

    }

    public void ImportCbmp() {

        OpenFileWindowUtil.OpenFile("FCEAssets/Textures/Cbmp", "", path => {

            try {

                texture.ImportCbmp(File.ReadAllBytes(path));

                main.RefreshTextures();

                foreach (var section in main.sectionMeshes) {
                    section.RefreshTexture();
                    section.RefreshMesh();
                }

                Refresh();

            }
            catch {

                DialogWindowUtil.Dialog("Invalid File", "Please select a valid Cbmp file.");

            }

        });

    }

    public void OnClickGenerateColorPalette() {

        DialogWindowUtil.Dialog("Color Palette Warning",
            "Are you sure you would like to generate a color palette? This will overwrite the existing color palette. " +
            "For existing Future Cop textures, it is recommended to use already existing Future Cop color palettes. ", () => {

                var counts = texture.CreateColorPalette();

                texture.ClearLookUpData(counts.Item1, counts.Item2);

                QuickLogHandler.Log("Color palette created for bitmap " + texture.DataID.ToString(), LogSeverity.Success);

                return true;

            });

    }

}
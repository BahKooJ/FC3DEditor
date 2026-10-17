

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextureSnippetsView : MonoBehaviour {

    // - Prefabs -
    public GameObject snippetListItem;

    // - Unity Refs -
    public Transform listContent;
    public Transform addSnippetButtonTrans;
    public UniversalUVMapperView universalUVMapper;

    // - Parameters -
    [HideInInspector]
    public Main main;
    public FCopLevel level;

    List<TextureSnippetItemView> listItems = new();

    void Start() {

        universalUVMapper.main = main;

        Refresh();

    }

    public void Refresh() {

        foreach (var item in listItems) {
            Destroy(item.gameObject);
        }

        listItems.Clear();

        foreach (var snippet in level.textureSnippets) {

            var obj = Instantiate(snippetListItem);
            obj.transform.SetParent(listContent.transform, false);
            obj.SetActive(true);

            var snippetItem = obj.GetComponent<TextureSnippetItemView>();
            snippetItem.main = main;
            snippetItem.view = this;
            snippetItem.textureSnippet = snippet;
            snippetItem.level = level;

            listItems.Add(snippetItem);

        }

        addSnippetButtonTrans.SetAsLastSibling();

    }

    public void RequestSnippetEdit(TextureSnippetItemView snippetItem) {

        universalUVMapper.gameObject.SetActive(true);
        universalUVMapper.bmpID = snippetItem.textureSnippet.texturePaletteID;
        universalUVMapper.uvs = snippetItem.textureSnippet.ConvertToUVs();
        universalUVMapper.forceRect = true;
        universalUVMapper.Refresh();

        universalUVMapper.onFinishCallback = (uvs, bmpID) => {

            var x = uvs.Min(uv => { return TextureCoordinate.GetXPixel(uv); });
            var y = uvs.Min(uv => { return TextureCoordinate.GetYPixel(uv); });

            var maxX = uvs.Max(uv => { return TextureCoordinate.GetXPixel(uv); });
            var maxY = uvs.Max(uv => { return TextureCoordinate.GetYPixel(uv); });

            snippetItem.textureSnippet.x = x;
            snippetItem.textureSnippet.y = y;
            snippetItem.textureSnippet.width = maxX - x;
            snippetItem.textureSnippet.height = maxY - y;
            snippetItem.textureSnippet.texturePaletteID = bmpID;

            Refresh();
            universalUVMapper.gameObject.SetActive(false);

        };


    }

    public void AddSnippet() {

        if (level.textureSnippets.Count == 0) {
            level.AddTextureSnippet(20, 20, 20, 20, 0);

        }
        else {

            var lastSnippet = level.textureSnippets.Last();

            level.AddTextureSnippet(lastSnippet.x, lastSnippet.y, lastSnippet.width, lastSnippet.height, lastSnippet.texturePaletteID);

        }



        Refresh();


    }


}
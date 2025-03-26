

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

    // - Parameters -
    [HideInInspector]
    public Main main;
    public FCopLevel level;

    List<TextureSnippetItemView> listItems = new();

    void Start() {

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

    public void AddSnippet() {

        

        Refresh();

        listItems.Last().Rename();

    }


}
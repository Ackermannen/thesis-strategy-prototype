using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

    [SerializeField] private Tilemap tilemapWithData = null;

    [SerializeField] private TextMeshProUGUI debugTextWindow = null;

    [NonSerialized] public TileBase activeTile = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setActiveTile(Vector3Int pos) {
        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, this, "Active tile is at: ({0},{1})", pos.x, pos.y);
        TileBase tile = tilemapWithData.GetTile(pos);
        activeTile = tile;

        if(tile.name != null) {
            Debug.LogFormat("You've clicked on a city named {0}", tile.name);
            debugTextWindow.SetText(tile.name);
        }
    }

    public void removeActiveTile() {
        activeTile = null;
        debugTextWindow.SetText("");
    }
}

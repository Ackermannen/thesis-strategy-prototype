using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProvinceHandler : MonoBehaviour
{

    private Tilemap map;

    // Start is called before the first frame update
    void Start() {
        map = this.GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector3Int tilemapPos = map.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            Tile tile = map.GetTile<Tile>(tilemapPos);


            if (tile) {
                Debug.Log(string.Format("Tile is: {0}", tile.name));
            }
        }
    }
}

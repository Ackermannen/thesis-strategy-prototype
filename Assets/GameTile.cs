using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTile : Tile
{
    public enum TileType {
        City = 0,
        Railroad = 1,
        Environment = 2,
    }

    public class Route {
        public string start;
        public string end;
    }


    public TileType type { get; set; }

    //only availalble when tile is a city
    public int cityIndex;

    //only availalble when tile is a railraod
    public Route route;

}

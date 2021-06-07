using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardParser : MonoBehaviour {

    [Serializable]
    public class Route {
        public int start;
        public int end;
    }

    public Tilemap paintedTerrain;

    public TileSet terrainTypes;

    public Tilemap paintedCities;

    public List<string> cityNames;

    public string outputName;

    public Route[] routes;

    public Sprite[] cityPlaceholders;

    public int player;

    // Start is called before the first frame update
    void Start()
    {

        //Initial preperation of the document
        XmlDocument doc = new XmlDocument();
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmlDeclaration, root);
        int cityCount = 0;
        paintedTerrain.CompressBounds();

        XmlElement mapNode = doc.CreateElement(string.Empty, "Map", string.Empty);
        mapNode.SetAttribute("player", player + "");
        doc.AppendChild(mapNode);

        //Appends bounds
        XmlElement boundsNode = doc.CreateElement(string.Empty, "Bounds", string.Empty);
        BoundsInt b = paintedTerrain.cellBounds;
        boundsNode.SetAttribute("xMin", b.xMin + "");
        boundsNode.SetAttribute("xMax", b.xMax + "");
        boundsNode.SetAttribute("yMin", b.yMin + "");
        boundsNode.SetAttribute("yMax", b.yMax + "");
        mapNode.AppendChild(boundsNode);




        //Create connections
        XmlElement connectionsNode = doc.CreateElement(string.Empty, "Connections", string.Empty);
        
        foreach (Route route in routes) {
            XmlElement routeNode = doc.CreateElement(string.Empty, "Route", string.Empty);
            routeNode.SetAttribute("start", route.start + "");
            routeNode.SetAttribute("end", route.end + "");
            connectionsNode.AppendChild(routeNode);
        }
        mapNode.AppendChild(connectionsNode);




        XmlElement tilesNode = doc.CreateElement(string.Empty, "Tiles", string.Empty);
        mapNode.AppendChild(tilesNode);

        for(int y = paintedTerrain.cellBounds.min.y; y < paintedTerrain.cellBounds.max.y; y++) {

            //append parallel
            XmlElement parallelNode = doc.CreateElement(string.Empty, "Parallel", string.Empty);
            parallelNode.SetAttribute("index", y + "");
            tilesNode.AppendChild(parallelNode);


            for (int x = paintedTerrain.cellBounds.min.x; x < paintedTerrain.cellBounds.max.x; x++) {
                try {
                    //z always 0
                    Tile currentTile = paintedTerrain.GetTile<Tile>(new Vector3Int(x,y, 0));

                    if (currentTile == null) throw new NullReferenceException();

                    XmlElement tileNode = doc.CreateElement(string.Empty, "Tile", string.Empty);
                    tileNode.SetAttribute("index", x + "");
                    parallelNode.AppendChild(tileNode);

                    if (currentTile.sprite == terrainTypes.desert) {
                        XmlElement cellNode = doc.CreateElement(string.Empty, "Nature", string.Empty);
                        cellNode.SetAttribute("type", "desert");
                        tileNode.AppendChild(cellNode);
                    } else if (currentTile.sprite == terrainTypes.grasslands) {
                        XmlElement cellNode = doc.CreateElement(string.Empty, "Nature", string.Empty);
                        cellNode.SetAttribute("type", "grasslands");
                        tileNode.AppendChild(cellNode);
                    } else if (currentTile.sprite == terrainTypes.water) {
                        XmlElement cellNode = doc.CreateElement(string.Empty, "Nature", string.Empty);
                        cellNode.SetAttribute("type", "water");
                        tileNode.AppendChild(cellNode);
                    }

                    try {
                        //If city can be found in this location, append city
                        Tile currentCity = paintedCities.GetTile<Tile>(new Vector3Int(x,y, 0));

                        if (currentCity == null) throw new NullReferenceException();

                        XmlElement cityNode = doc.CreateElement(string.Empty, "City", string.Empty);
                        cityNode.SetAttribute("index", getCityIndex(currentCity) + "");
                        cityNode.InnerText = cityNames[cityCount];
                        cityCount++;

                        tileNode.AppendChild(cityNode);

                        //Ignoring as it simply means field will not be present in xml, which is intended.
                    } catch (NullReferenceException) { }


                    //Ignoring as this just means that there's no tile here.
                } catch(NullReferenceException) { }

            }
        
            //Removing parallel if no children exist
            if(!parallelNode.HasChildNodes) {
                Debug.Log("Empty, removing parallel");
                tilesNode.RemoveChild(parallelNode);
            }
        }
        string[] paths = { Directory.GetCurrentDirectory(), "Assets", "Resources" };
        string path = Path.Combine(paths);
        string fileName = Path.Combine(path, outputName + ".xml");

        doc.Save(fileName);
        Debug.Log("File saved");

    }

    private int getCityIndex(Tile currentCity) {
        int index = 1;
        foreach(Sprite placeholder in cityPlaceholders) {
            if(currentCity.sprite == placeholder) {
                return index;
            }
            index++;
        }

        throw new NullReferenceException("Placeholder for city is missing for city tile");
    }
}

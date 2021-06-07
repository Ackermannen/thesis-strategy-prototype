using BezierSolution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


public class XMLParser : MonoBehaviour
{
    [SerializeField]
    public TextAsset XMLMap;

    [SerializeField]
    public Tilemap terrainMap;

    [SerializeField]
    public TileSet EnvironmentTiles;

    [SerializeField]
    public GameObject cityModel;

    [SerializeField]
    public GameObject playerCityModel;

    public DealPanel dealPanelPrefab;
    public GameObject dealPanelContainer;
    public GameObject hammerPrefab;

    [SerializeField] public CameraController panLimits;

    [SerializeField] public GameObject splines;

    [SerializeField] public GameObject cities;

    [SerializeField] public Material roadMaterial;

    [NonSerialized] public static bool parsingComplete = false;

    [NonSerialized] public static Dictionary<Connection, Rail> connectionMap;

    [NonSerialized] public static Dictionary<int, string> cityNameMap;

    void Start()
    {
        string data = XMLMap.text;
        connectionMap = new Dictionary<Connection, Rail>();
        cityNameMap = new Dictionary<int, string>();
        parseMap(data);
        
    }

    private GameTile createEnvironmentTile(string env, string cityName, string index)
    {
        GameTile tile = ScriptableObject.CreateInstance<GameTile>();

        tile.name = cityName != null ? cityName : env;

        tile.type = cityName != null ? GameTile.TileType.City : GameTile.TileType.Environment;

        tile.cityIndex = index != null ? int.Parse(index) : -1;

        switch(env)
        {
            case "grasslands":
                tile.sprite = EnvironmentTiles.grasslands;
                break;
            case "desert":
                tile.sprite = EnvironmentTiles.desert;
                break;
            case "water":
                tile.sprite = EnvironmentTiles.water;
                break;
        }
        return tile;
    }

    private void addCityToTile(int x, int y, string name, int index) {
        Vector3 worldCoordinates = terrainMap.CellToWorld(new Vector3Int(x, y, 0));
        GameObject city = Instantiate(PlayerDataController.playerCity == index ? playerCityModel : cityModel, worldCoordinates, Quaternion.Euler(0F, 0F, UnityEngine.Random.Range(0F, 360F)));

        BoxCollider2D collider = city.AddComponent<BoxCollider2D>();

        //Textmesh for ease finding of city.
        GameObject textMeshContainer = new GameObject();
        textMeshContainer.transform.position = city.transform.position + new Vector3(0f, -0.5f, -0.2f);
        textMeshContainer.transform.parent = city.transform;
        textMeshContainer.transform.eulerAngles = Vector3.zero;
        TextMeshPro textMesh = textMeshContainer.AddComponent<TextMeshPro>();
        textMesh.text = name;
        textMesh.fontSize = 8;
        textMesh.sortingLayerID = SortingLayer.NameToID("Cities");
        textMesh.alignment = TextAlignmentOptions.Center;

        IntegratedDealController idc = city.AddComponent<IntegratedDealController>();
        idc.city = index;
        idc.DealContainerPrefab = dealPanelContainer;

        collider.size = new Vector2(1, 1);

        Tooltippable t = city.AddComponent<Tooltippable>();
        t.content = name;
        city.transform.parent = cities.transform;
    }

    private void parseMap(string xmlFile)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xmlFile));

        XmlNode map = xmlDoc.SelectSingleNode("/Map");
        PlayerDataController.playerCity = int.Parse(map.Attributes.GetNamedItem("player").Value);


        XmlNode bounds = xmlDoc.SelectSingleNode("/Map/Bounds");
        Vector3Int minimumPan = new Vector3Int(int.Parse(bounds.Attributes.GetNamedItem("xMin").Value), int.Parse(bounds.Attributes.GetNamedItem("yMin").Value), 0);
        Vector3Int maximumPan = new Vector3Int(int.Parse(bounds.Attributes.GetNamedItem("xMax").Value), int.Parse(bounds.Attributes.GetNamedItem("yMax").Value), 0);

        panLimits.minPanLimit.x = terrainMap.CellToWorld(minimumPan).x;
        panLimits.minPanLimit.y = terrainMap.CellToWorld(minimumPan).y;
        panLimits.maxPanLimit.x = terrainMap.CellToWorld(maximumPan).x;
        panLimits.maxPanLimit.y = terrainMap.CellToWorld(maximumPan).y;



        XmlNodeList parallels = xmlDoc.SelectNodes("/Map/Tiles/Parallel");

        int x = int.Parse(bounds.Attributes.GetNamedItem("xMin").Value);
        int y = int.Parse(bounds.Attributes.GetNamedItem("yMin").Value);


        Dictionary<int, Vector3Int> cities = new Dictionary<int, Vector3Int>();

        foreach (XmlNode parallel in parallels)
        {
            foreach (XmlNode cell in parallel.ChildNodes)
            {
                string env = cell["Nature"].Attributes.GetNamedItem("type").Value;
                string name;
                string cityIndex;

                try {
                    cityIndex = cell["City"].Attributes.GetNamedItem("index").Value;
                    name = int.Parse(cityIndex) == PlayerDataController.playerCity ? "Your city" : cell["City"].InnerText;
                    //if reached this point, add city to the tile
                    cities.Add(int.Parse(cityIndex), new Vector3Int(x, y, 0));
                    addCityToTile(x, y, name, int.Parse(cityIndex));
                    cityNameMap.Add(int.Parse(cityIndex), name);
                } catch (NullReferenceException) {
                    name = null;
                    cityIndex = null;
                }

                GameTile newTile = createEnvironmentTile(env, name, cityIndex);


                terrainMap.SetTile(new Vector3Int(x, y, 0), newTile);

                

                x++;
            }

            x = int.Parse(bounds.Attributes.GetNamedItem("xMin").Value);
            y++;
        }

        XmlNodeList routeNodes = xmlDoc.SelectNodes(("/Map/Connections/Route"));
        foreach(XmlNode routeNode in routeNodes) {
            int startIndex = int.Parse(routeNode.Attributes.GetNamedItem("start").Value);
            int endIndex = int.Parse(routeNode.Attributes.GetNamedItem("end").Value);

            Vector3Int startPos = cities[startIndex];
            Vector3Int endPos = cities[endIndex];

            Vector3 realStartPos = terrainMap.CellToWorld(startPos);
            Vector3 realEndPos = terrainMap.CellToWorld(endPos);

            GameObject newCurve = new GameObject("Curve");
            newCurve.transform.parent = splines.transform;

            //Offset by .01 up
            Vector3 ts = newCurve.transform.position;
            ts.z -= .01F;
            newCurve.transform.position = ts;

            //Adds info about the city connection
            Connection c = new Connection();
            c.start = startIndex;
            c.end = endIndex;

            BezierSpline spline = newCurve.AddComponent<BezierSpline>();
            Rail rail = newCurve.AddComponent<Rail>();
            rail.spline = spline;
            rail.Health = 15;
            rail.maxHealth = 15;

            connectionMap.Add(c, rail);

            GameObject railroad = new GameObject("Railroad");
            railroad.transform.parent = newCurve.transform;
            MeshFilter filter = railroad.AddComponent<MeshFilter>();
            MeshRenderer renderer = railroad.AddComponent<MeshRenderer>();
            MeshCollider collider = railroad.AddComponent<MeshCollider>();
            IntegratedRailController irc = railroad.AddComponent<IntegratedRailController>();

            //Uses same prefab
            irc.RailContainerPrefab = dealPanelContainer;
            irc.connection = c;
            
            
            rail.railroadMesh = renderer;
            

            //Offset by .005 down
            //TODO make railroad show
            Vector3 rr = railroad.transform.position;
            rr.z += .01F;
            railroad.transform.position = rr;

            SplineMesher mesher = new SplineMesher();
            renderer.material = roadMaterial;


            spline.Initialize(2);

            spline[0].position = realStartPos;
            spline[1].position = realEndPos;
            Vector3 inbetween = spline[0].position + (spline[1].position - spline[0].position) * 0.5f;
            irc.targetPosition = inbetween;

            GameObject hammer = Instantiate(hammerPrefab, newCurve.transform);
            hammer.transform.position = inbetween;
            rail.hammer = hammer;
            hammer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            hammer.transform.rotation = Quaternion.Euler(new Vector3(90, 90, -90));
            hammer.SetActive(false);

            Vector2[] positions = { spline[0].position, spline[1].position};

            Mesh roadMesh = mesher.CreateRoadMesh(positions, false);
            filter.mesh = roadMesh;
            collider.sharedMesh = roadMesh;

            int textureRepeat = Mathf.RoundToInt(positions.Length);
            renderer.sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);

            spline.ConstructLinearPath();

            
        }


        //offset railroad slightly above rest of objects
        Vector3 splinePos = splines.transform.position;
        splinePos.z -= .05F;
        splines.transform.position = splinePos;


        //Signifies all objects from file are instanced.
        parsingComplete = true;
    }

}

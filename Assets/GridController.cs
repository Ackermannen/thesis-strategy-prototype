using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridController : MonoBehaviour {
    private Grid grid;
    private Vector3Int previousClickLocation;

    [SerializeField] private Tilemap interactiveMap = null;
    [SerializeField] private Tilemap activeTileMap = null;
    [SerializeField] private Tilemap terrainMap = null;
    [SerializeField] private Tile hoverTile = null;
    [SerializeField] private Tile activeTile = null;

    private Vector3Int previousMousePos = new Vector3Int();

    // Start is called before the first frame update
    void Start() {
        grid = gameObject.GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update() {
        //Only do if playerState is normal play.
        if(PlayerDataController.playerState == PlayerInteractionState.REGULAR) {
            Vector3Int mousePos = GetMousePosition();

            //moves marker around
            if (!mousePos.Equals(previousMousePos)) {
                interactiveMap.SetTile(previousMousePos, null); // Remove old hoverTile

                //No marker if outside of the map or on GUI
                if (terrainMap.GetTile(mousePos) != null && !EventSystem.current.IsPointerOverGameObject()) {
                    interactiveMap.SetTile(mousePos, hoverTile);
                }

                previousMousePos = mousePos;
            }

            // Left mouse click -> add path tile
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                handleLeftClick(mousePos);
            }

            // Right mouse click -> remove path tile
            if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject()) {
                handleRightClick(mousePos);
            }
        }
    }

    Vector3Int GetMousePosition() {
        Vector3 mouseWorldPos = GetWorldPositionOnPlane(Input.mousePosition, 0);
        return grid.WorldToCell(mouseWorldPos);
    }

    private Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    private void handleLeftClick(Vector3Int target) {

        try {
            if (terrainMap.GetTile(target) == null) {
                throw new NullReferenceException("Object is outside of the playable map");
            };
        } catch(NullReferenceException) {
            handleRightClick(target);
            return;
        }

        if(previousClickLocation != null) {
            activeTileMap.SetTile(previousClickLocation, null);
        }
        

        activeTileMap.SetTile(target, activeTile);

        previousClickLocation = target;
    }

    private void handleRightClick(Vector3Int target) {
        activeTileMap.SetTile(target, null);

        //Remove previous location
        if (previousClickLocation != null) {
            activeTileMap.SetTile(previousClickLocation, null);
        }
    }
}
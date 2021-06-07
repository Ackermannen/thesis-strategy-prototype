using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 minPanLimit;
    public Vector2 maxPanLimit;
    public float scrollSpeed = 2f;

    public float maxAngle = 40f;
    public float minScrollZ = 1f;
    public float maxScrollZ = 10f;

    public Canvas worldSpaceCanvas;

    public bool panByMouse = false;


    private void Start()
    {
        transform.eulerAngles = new Vector3(359f, 0f);

    }

    public void MoveTo(Vector2 location) {
        transform.position = new Vector3(location.x, location.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Vector3 worldSpaceScale = worldSpaceCanvas.transform.localScale;

        if (Input.GetKey("w") || Input.GetKey("up") ||panByMouse && Input.mousePosition.y >= Screen.height - panBorderThickness &&
            !EventSystem.current.IsPointerOverGameObject()) {
            pos.y += panSpeed * Mathf.Abs(Time.unscaledDeltaTime *pos.z*0.5f);
        }

        if (Input.GetKey("s") || Input.GetKey("down") || panByMouse && Input.mousePosition.y <= panBorderThickness &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            pos.y -= panSpeed * Mathf.Abs(Time.unscaledDeltaTime * pos.z * 0.5f);
        }


        if (Input.GetKey("d") || Input.GetKey("right") || panByMouse && Input.mousePosition.x >= Screen.width - panBorderThickness && !EventSystem.current.IsPointerOverGameObject())
        {
            pos.x += panSpeed * Mathf.Abs(Time.unscaledDeltaTime * pos.z * 0.5f);
        }

        if (Input.GetKey("a") || Input.GetKey("left") || panByMouse && Input.mousePosition.x <= panBorderThickness && !EventSystem.current.IsPointerOverGameObject())
        {
            pos.x -= panSpeed * Mathf.Abs(Time.unscaledDeltaTime * pos.z * 0.5f);
        }

        //Only change z axis if not using GUI
        if(!EventSystem.current.IsPointerOverGameObject() || (EventSystem.current.IsPointerOverGameObject() && PlayerDataController.mouseOnNonIntrusive)) {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            pos.z += scrollInput * scrollSpeed * 100f * Time.unscaledDeltaTime;
            if (pos.z >= -(minScrollZ + 2)) {
                //Dividing by pos.z makes so that rotations happen quicker and at a higher angle when zoomed in further
                float maxClampedAngle = Mathf.Clamp(Mathf.Abs(maxAngle / pos.z), 0, maxAngle);
                rot = Quaternion.Slerp(rot, Quaternion.Euler(-maxClampedAngle, 0, 0), Time.unscaledDeltaTime * Mathf.Abs(10 / pos.z));
            } else {
                rot = Quaternion.Slerp(rot, Quaternion.Euler(0, 0, 0), Time.unscaledDeltaTime * 2F);
                
            }
        }

        worldSpaceScale = Vector3.Lerp(worldSpaceScale, new Vector3(0,0, 0.01f) + new Vector3(0.01f, 0.01f, 0f)*Mathf.Abs(Mathf.Min(pos.z, -2)*0.12f), Time.unscaledDeltaTime * 5F);

        //Positional clamping
        pos.x = Mathf.Clamp(pos.x, minPanLimit.x, maxPanLimit.x);
        pos.y = Mathf.Clamp(pos.y, minPanLimit.y, maxPanLimit.y);
        pos.z = Mathf.Clamp(pos.z, -maxScrollZ, -minScrollZ);

        transform.position = pos;
        transform.rotation = rot;
        worldSpaceCanvas.transform.localScale = worldSpaceScale;
    }
}

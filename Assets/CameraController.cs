using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public Vector2 minPanLimit;
    public Vector2 maxPanLimit;
    public float scrollSpeed = 2f;

    public float maxAngle = 40f;
    public float minScrollZ = 1f;
    public float maxScrollZ = 10f;



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

        
        if (Input.GetKey("w") || Input.GetKey("up")) {
            pos.y += panSpeed * Mathf.Abs(Time.unscaledDeltaTime *pos.z*0.5f);
        }

        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            pos.y -= panSpeed * Mathf.Abs(Time.unscaledDeltaTime * pos.z * 0.5f);
        }


        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            pos.x += panSpeed * Mathf.Abs(Time.unscaledDeltaTime * pos.z * 0.5f);
        }

        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            pos.x -= panSpeed * Mathf.Abs(Time.unscaledDeltaTime * pos.z * 0.5f);
        }

        //If not on gui, or if on gui then only on nonintrusive elements.
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



        //Positional clamping
        pos.x = Mathf.Clamp(pos.x, minPanLimit.x, maxPanLimit.x);
        pos.y = Mathf.Clamp(pos.y, minPanLimit.y, maxPanLimit.y);
        pos.z = Mathf.Clamp(pos.z, -maxScrollZ, -minScrollZ);

        transform.position = pos;
        transform.rotation = rot;
    }
}

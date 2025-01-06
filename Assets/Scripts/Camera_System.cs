using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_System : MonoBehaviour
{
    public GameObject the_eye;
    public GameObject character;
    // Start is called before the first frame update
    void Start()
    {
        if (the_eye == null)
        {
            Debug.LogError("Camera Prefab is not assigned!");
            return;
        }

        if (character == null)
        {
            Debug.LogError("Parent GameObject is not assigned!");
            return;
        }
        // Tạo camera mới từ prefab
        GameObject newCamera = Instantiate(the_eye);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

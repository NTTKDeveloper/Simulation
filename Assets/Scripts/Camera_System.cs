using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_System : MonoBehaviour
{
    public GameObject the_eye;
    public GameObject character;
    // Start is called before the first frame update
    public GameObject gound;
    private Camera left_cam;
    private Camera right_cam;
    public GameObject stereodataExporter;
    
    void Awake()
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
        //Tạo nhân vật
        GameObject nhanvat = Instantiate(character);
        nhanvat.transform.position = gound.transform.position;
        // Tạo camera mới từ prefab
        GameObject left_Eye = Instantiate(the_eye);
        GameObject right_Eye = Instantiate(the_eye);
        
        left_cam = left_Eye.GetComponent<Camera>();
        right_cam = right_Eye.GetComponent<Camera>();

        //Add Camera vào script 
        CharacterControllerSystem script = nhanvat.GetComponent<CharacterControllerSystem>();
        script.leftCamera = left_cam;
        script.rightCamera = right_cam;

        //Đặt tên và tag cho camera
        left_Eye.tag = "left_Eye";
        left_Eye.name = "left_Eye";
        right_Eye.tag = "right_Eye";
        right_Eye.name = "right_Eye";

        //Đặt nhanvat làm cha của camera
        left_Eye.transform.SetParent(nhanvat.transform);
        right_Eye.transform.SetParent(nhanvat.transform);                               
        //Setup vị trí cho eye
        // Lấy Capsule Collider từ nhân vật
        CapsuleCollider capsuleCollider = nhanvat.GetComponent<CapsuleCollider>();
        // Tính toán vị trí trên đỉnh đầu
        float characterHeight = capsuleCollider.height;
        Vector3 R_topPosition = new Vector3(0.03f, characterHeight - 1.2f, 0);
        Vector3 L_topPosition = new Vector3(-0.03f, characterHeight - 1.2f, 0);
        // Đặt vị trí của camera (trong không gian local)
        left_Eye.transform.localPosition = L_topPosition;
        right_Eye.transform.localPosition = R_topPosition;


        StereoDataExporter stereoDataExporter = stereodataExporter.GetComponent<StereoDataExporter>();
        stereoDataExporter.leftCamera = left_cam;
        stereoDataExporter.rightCamera = right_cam;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

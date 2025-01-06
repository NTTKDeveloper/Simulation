using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps800x600 : MonoBehaviour
{
    void Awake()
    {
        // Đặt giới hạn FPS (ví dụ: 60 FPS)
        Application.targetFrameRate = 60;
        // Đặt kích thước cửa sổ game là 800x600 và không cho phép thay đổi kích thước cửa sổ
        Screen.SetResolution(800, 600, false);
    }
}

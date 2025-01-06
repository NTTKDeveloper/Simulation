using System.Collections;
using System.IO;
using UnityEngine;

public class StereoDataExporter : MonoBehaviour
{
    public Camera leftCamera;
    public Camera rightCamera;
    public string outputPath = "StereoData";

    void Start()
    {
        // Kiểm tra nếu các camera được gán
        if (leftCamera == null || rightCamera == null)
        {
            Debug.LogError("Vui lòng gán LeftCamera và RightCamera.");
            return;
        }

        // Tạo thư mục lưu trữ nếu chưa tồn tại
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        // Lưu ảnh và thông số
        StartCoroutine(CaptureStereoData());
    }

    IEnumerator CaptureStereoData()
    {
        yield return new WaitForEndOfFrame();

        // Chụp ảnh từ các camera
        SaveCameraImage(leftCamera, Path.Combine(outputPath, "left.png"));
        SaveCameraImage(rightCamera, Path.Combine(outputPath, "right.png"));

        // Lưu thông số camera
        SaveCameraParameters();
    }

    void SaveCameraImage(Camera cam, string filePath)
    {
        // Render camera ra RenderTexture
        RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
        cam.targetTexture = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        cam.Render();
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Lưu ảnh thành file PNG
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        // Dọn dẹp
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(texture);

        Debug.Log($"Saved image to {filePath}");
    }

    void SaveCameraParameters()
    {
        // Thu thập thông số nội tại
        Matrix4x4 leftIntrinsic = GetIntrinsicMatrix(leftCamera);
        Matrix4x4 rightIntrinsic = GetIntrinsicMatrix(rightCamera);

        // Thu thập thông số ngoại tại
        Matrix4x4 leftToWorld = leftCamera.transform.localToWorldMatrix;
        Matrix4x4 rightToWorld = rightCamera.transform.localToWorldMatrix;

        Matrix4x4 worldToRight = rightToWorld.inverse;
        Matrix4x4 leftToRight = worldToRight * leftToWorld;

        // Tính baseline (khoảng cách ngang giữa hai camera)
        float baseline = Vector3.Distance(leftCamera.transform.position, rightCamera.transform.position);

        // Tạo đối tượng JSON
        var cameraParams = new
        {
            leftCameraIntrinsic = leftIntrinsic,
            rightCameraIntrinsic = rightIntrinsic,
            leftToRightExtrinsic = leftToRight,
            baseline = baseline,
            resolution = new { width = 1920, height = 1080 },
            fieldOfView = leftCamera.fieldOfView,
            nearClip = leftCamera.nearClipPlane,
            farClip = leftCamera.farClipPlane
        };

        // Lưu vào file JSON
        string json = JsonUtility.ToJson(cameraParams, true);
        File.WriteAllText(Path.Combine(outputPath, "camera_params.json"), json);

        Debug.Log($"Saved camera parameters to {Path.Combine(outputPath, "camera_params.json")}");
    }

    Matrix4x4 GetIntrinsicMatrix(Camera cam)
    {
        float focalLength = 0.5f * cam.pixelHeight / Mathf.Tan(0.5f * cam.fieldOfView * Mathf.Deg2Rad);
        return new Matrix4x4(
            new Vector4(focalLength, 0, 0, 0),
            new Vector4(0, focalLength, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1)
        );
    }
}

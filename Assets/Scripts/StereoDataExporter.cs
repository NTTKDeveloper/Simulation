using System.Collections;
using System.IO;
using UnityEngine;

public class StereoDataExporter : MonoBehaviour
{
    public Camera leftCamera;
    public Camera rightCamera;
    public string outputPath;
    private int captureCount = 1;

    void Awake()
    {
        outputPath = Path.Combine(Application.persistentDataPath, "StereoData");
    }

    void Start()
    {
        if (leftCamera == null || rightCamera == null)
        {
            Debug.LogError("Vui lòng gán LeftCamera và RightCamera.");
            return;
        }

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        Debug.Log($"Data will be saved in: {Path.GetFullPath(outputPath)}");
        InvokeRepeating("CaptureStereoData", 0f, 5f);
    }

    void CaptureStereoData()
    {
        string leftImagePath = Path.Combine(outputPath, $"left_{captureCount}.png");
        string rightImagePath = Path.Combine(outputPath, $"right_{captureCount}.png");

        SaveCameraImage(leftCamera, leftImagePath);
        SaveCameraImage(rightCamera, rightImagePath);

        SaveCameraParameters(leftImagePath, rightImagePath);

        captureCount++;
    }

    void SaveCameraImage(Camera cam, string filePath)
    {
        RenderTexture renderTexture = new RenderTexture(800, 600, 24);
        cam.targetTexture = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        cam.Render();
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(texture);

        Debug.Log($"Image saved successfully at: {filePath}");
    }

    void SaveCameraParameters(string leftImagePath, string rightImagePath)
    {
        var leftIntrinsic = new SerializableMatrix4x4(GetIntrinsicMatrix(leftCamera));
        var rightIntrinsic = new SerializableMatrix4x4(GetIntrinsicMatrix(rightCamera));

        Matrix4x4 leftToWorld = leftCamera.transform.localToWorldMatrix;
        Matrix4x4 rightToWorld = rightCamera.transform.localToWorldMatrix;
        var leftToRight = new SerializableMatrix4x4(rightToWorld.inverse * leftToWorld);

        float baseline = Vector3.Distance(leftCamera.transform.position, rightCamera.transform.position);

        var cameraParams = new CameraParams
        {
            leftCameraIntrinsic = leftIntrinsic,
            rightCameraIntrinsic = rightIntrinsic,
            leftToRightExtrinsic = leftToRight,
            baseline = baseline,
            resolution = new CameraParams.Resolution { width = 800, height = 600 },
            fieldOfView = leftCamera.fieldOfView,
            nearClip = leftCamera.nearClipPlane,
            farClip = leftCamera.farClipPlane,
            leftCameraPosition = leftCamera.transform.position,
            rightCameraPosition = rightCamera.transform.position,
            leftImageName = Path.GetFileName(leftImagePath),
            rightImageName = Path.GetFileName(rightImagePath),
            timestamp = System.DateTime.Now.ToString("o")
        };

        string filePath = Path.Combine(outputPath, $"camera_params_{captureCount}.json");
        string json = JsonUtility.ToJson(cameraParams, true);
        File.WriteAllText(filePath, json);

        Debug.Log($"Camera parameters saved successfully at: {filePath}");
    }

    Matrix4x4 GetIntrinsicMatrix(Camera cam)
    {
        float aspectRatio = (float)cam.pixelWidth / cam.pixelHeight;
        float focalLength = 0.5f * cam.pixelHeight / Mathf.Tan(0.5f * cam.fieldOfView * Mathf.Deg2Rad);

        return new Matrix4x4(
            new Vector4(focalLength, 0, cam.pixelWidth * 0.5f, 0),
            new Vector4(0, focalLength * aspectRatio, cam.pixelHeight * 0.5f, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1)
        );
    }
}

[System.Serializable]
public class CameraParams
{
    public SerializableMatrix4x4 leftCameraIntrinsic;
    public SerializableMatrix4x4 rightCameraIntrinsic;
    public SerializableMatrix4x4 leftToRightExtrinsic;
    public float baseline;
    public Resolution resolution;
    public float fieldOfView;
    public float nearClip;
    public float farClip;
    public Vector3 leftCameraPosition;
    public Vector3 rightCameraPosition;
    public string leftImageName;
    public string rightImageName;
    public string timestamp;

    [System.Serializable]
    public struct Resolution
    {
        public int width;
        public int height;
    }
}


[System.Serializable]
public class SerializableMatrix4x4
{
    public float[] values;

    public SerializableMatrix4x4(Matrix4x4 matrix)
    {
        values = new float[]
        {
            matrix.m00, matrix.m01, matrix.m02, matrix.m03,
            matrix.m10, matrix.m11, matrix.m12, matrix.m13,
            matrix.m20, matrix.m21, matrix.m22, matrix.m23,
            matrix.m30, matrix.m31, matrix.m32, matrix.m33
        };
    }

    public Matrix4x4 ToMatrix4x4()
    {
        return new Matrix4x4(
            new Vector4(values[0], values[1], values[2], values[3]),
            new Vector4(values[4], values[5], values[6], values[7]),
            new Vector4(values[8], values[9], values[10], values[11]),
            new Vector4(values[12], values[13], values[14], values[15])
        );
    }
}

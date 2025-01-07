// using System.Collections;
// using System.IO;
// using UnityEngine;

// public class StereoDataExporter : MonoBehaviour
// {
//     public Camera leftCamera;
//     public Camera rightCamera;
//     public string outputPath;
//     private int captureCount = 1; // Biến để theo dõi số lần chụp

//     void Awake()
//     {
//         // Gán giá trị cho outputPath trong Awake hoặc Start
//         outputPath = Path.Combine(Application.persistentDataPath, "StereoData");
//     }

//     void Start()
//     {
//         // Kiểm tra nếu các camera được gán
//         if (leftCamera == null || rightCamera == null)
//         {
//             Debug.LogError("Vui lòng gán LeftCamera và RightCamera.");
//             return;
//         }

//         // Tạo thư mục lưu trữ nếu chưa tồn tại
//         if (!Directory.Exists(outputPath))
//         {
//             Directory.CreateDirectory(outputPath);
//         }

//         Debug.Log($"Data will be saved in: {Path.GetFullPath(outputPath)}");

//         // Bắt đầu chụp ảnh mỗi giây
//         InvokeRepeating("CaptureStereoData", 0f, 5f);  // Gọi hàm CaptureStereoData mỗi giây
//     }

//     void CaptureStereoData()
//     {
//         // Lưu ảnh và thông số
//         string leftImagePath = Path.Combine(outputPath, $"left_{captureCount}.png");
//         string rightImagePath = Path.Combine(outputPath, $"right_{captureCount}.png");

//         SaveCameraImage(leftCamera, leftImagePath);
//         SaveCameraImage(rightCamera, rightImagePath);

//         // Lưu thông số camera
//         SaveCameraParameters();

//         // Tăng số lần chụp
//         captureCount++;
//     }

//     void SaveCameraImage(Camera cam, string filePath)
//     {
//         // Render camera ra RenderTexture
//         RenderTexture renderTexture = new RenderTexture(800, 600, 24);
//         cam.targetTexture = renderTexture;
//         Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

//         cam.Render();
//         RenderTexture.active = renderTexture;
//         texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
//         texture.Apply();

//         // Lưu ảnh thành file PNG
//         byte[] bytes = texture.EncodeToPNG();
//         File.WriteAllBytes(filePath, bytes);

//         // Dọn dẹp
//         cam.targetTexture = null;
//         RenderTexture.active = null;
//         Destroy(renderTexture);
//         Destroy(texture);

//         // Xác nhận đã lưu
//         if (File.Exists(filePath))
//         {
//             Debug.Log($"Image saved successfully at: {filePath}");
//         }
//         else
//         {
//             Debug.LogError($"Failed to save image at: {filePath}");
//         }
//     }

//     void SaveCameraParameters()
//     {
//         // Thu thập thông số nội tại
//         var leftIntrinsic = new SerializableMatrix4x4(GetIntrinsicMatrix(leftCamera));
//         var rightIntrinsic = new SerializableMatrix4x4(GetIntrinsicMatrix(rightCamera));

//         // Thu thập thông số ngoại tại
//         Matrix4x4 leftToWorld = leftCamera.transform.localToWorldMatrix;
//         Matrix4x4 rightToWorld = rightCamera.transform.localToWorldMatrix;
//         var leftToRight = new SerializableMatrix4x4(rightToWorld.inverse * leftToWorld);

//         // Tính baseline
//         float baseline = Vector3.Distance(leftCamera.transform.position, rightCamera.transform.position);

//         // Gán vào lớp `CameraParams`
//         var cameraParams = new CameraParams
//         {
//             leftCameraIntrinsic = leftIntrinsic,
//             rightCameraIntrinsic = rightIntrinsic,
//             leftToRightExtrinsic = leftToRight,
//             baseline = baseline,
//             resolution = new CameraParams.Resolution { width = 800, height = 600 },
//             fieldOfView = leftCamera.fieldOfView,
//             nearClip = leftCamera.nearClipPlane,
//             farClip = leftCamera.farClipPlane
//         };

//         // Serialize và lưu JSON
//         string filePath = Path.Combine(outputPath, $"camera_params_{captureCount}.json");
//         string json = JsonUtility.ToJson(cameraParams, true);
//         Debug.Log($"Serialized JSON: {json}");
//         File.WriteAllText(filePath, json);

//         if (File.Exists(filePath))
//         {
//             Debug.Log($"Camera parameters saved successfully at: {filePath}");
//         }
//         else
//         {
//             Debug.LogError($"Failed to save camera parameters at: {filePath}");
//         }
//     }
//     string MatrixToString(Matrix4x4 matrix)
//     {
//         return JsonUtility.ToJson(new
//         {
//             m00 = matrix.m00, m01 = matrix.m01, m02 = matrix.m02, m03 = matrix.m03,
//             m10 = matrix.m10, m11 = matrix.m11, m12 = matrix.m12, m13 = matrix.m13,
//             m20 = matrix.m20, m21 = matrix.m21, m22 = matrix.m22, m23 = matrix.m23,
//             m30 = matrix.m30, m31 = matrix.m31, m32 = matrix.m32, m33 = matrix.m33
//         });
//     }

//     Matrix4x4 GetIntrinsicMatrix(Camera cam)
//     {
//         float aspectRatio = (float)cam.pixelWidth / cam.pixelHeight;
//         float focalLength = 0.5f * cam.pixelHeight / Mathf.Tan(0.5f * cam.fieldOfView * Mathf.Deg2Rad);

//         return new Matrix4x4(
//             new Vector4(focalLength, 0, cam.pixelWidth * 0.5f, 0),
//             new Vector4(0, focalLength * aspectRatio, cam.pixelHeight * 0.5f, 0),
//             new Vector4(0, 0, 1, 0),
//             new Vector4(0, 0, 0, 1)
//         );
//     }
// }


// [System.Serializable]
// public class SerializableMatrix4x4
// {
//     public float[] values;

//     public SerializableMatrix4x4(Matrix4x4 matrix)
//     {
//         values = new float[]
//         {
//             matrix.m00, matrix.m01, matrix.m02, matrix.m03,
//             matrix.m10, matrix.m11, matrix.m12, matrix.m13,
//             matrix.m20, matrix.m21, matrix.m22, matrix.m23,
//             matrix.m30, matrix.m31, matrix.m32, matrix.m33
//         };
//     }
// }

// [System.Serializable]
// public class CameraParams
// {
//     public SerializableMatrix4x4 leftCameraIntrinsic;
//     public SerializableMatrix4x4 rightCameraIntrinsic;
//     public SerializableMatrix4x4 leftToRightExtrinsic;
//     public float baseline;
//     public Resolution resolution;
//     public float fieldOfView;
//     public float nearClip;
//     public float farClip;

//     [System.Serializable]
//     public struct Resolution
//     {
//         public int width;
//         public int height;
//     }
// }
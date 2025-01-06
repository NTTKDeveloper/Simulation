using UnityEngine;

public class CharacterControllerSystem : MonoBehaviour
{
    // Tốc độ di chuyển và lực nhảy
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    // Tham chiếu đến camera
    public Camera leftCamera;
    public Camera rightCamera;

    // Rigidbody của nhân vật
    private Rigidbody rb;

    // Kiểm tra trạng thái nhân vật (trên mặt đất hay không)
    private bool isGrounded;

    // Độ nhạy của chuột
    public float mouseSensitivity = 100f;

    // Lưu trữ góc xoay theo chuột
    private float verticalRotation = 0f;

    void Start()
    {
        // Lấy Rigidbody từ đối tượng
        rb = GetComponent<Rigidbody>();

        if (leftCamera == null || rightCamera == null)
        {
            Debug.LogError("Cần gán camera left và right trong Inspector!");
            return;
        }

        // Bật cả hai camera
        SetActiveCamera(leftCamera, true);
        SetActiveCamera(rightCamera, true);

        // Khóa con trỏ chuột ở giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Xử lý chuyển động
        HandleMovement();

        // Xử lý xoay camera
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        // Lấy input từ bàn phím
        float moveX = Input.GetAxis("Horizontal"); // Di chuyển trái/phải
        float moveZ = Input.GetAxis("Vertical");   // Di chuyển trước/sau

        // Tạo vector chuyển động
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Di chuyển nhân vật bằng cách điều chỉnh vận tốc của Rigidbody
        Vector3 velocity = rb.velocity;
        rb.velocity = new Vector3(move.x * moveSpeed, velocity.y, move.z * moveSpeed);

        // Xử lý nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleMouseLook()
    {
        // Lấy thông tin di chuyển chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xử lý xoay theo trục dọc (giới hạn góc nhìn lên xuống)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Xoay nhân vật theo trục ngang
        transform.Rotate(Vector3.up * mouseX);

        // Xoay camera theo trục dọc
        leftCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        rightCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Xác định nếu nhân vật đang chạm đất
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Xác định nếu nhân vật không còn chạm đất
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Bật hoặc tắt camera
    void SetActiveCamera(Camera camera, bool isActive)
    {
        if (camera != null)
        {
            camera.enabled = isActive;
        }
    }
}

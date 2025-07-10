using UnityEngine;

/*
 * HƯỚNG DẪN SETUP CỔNG DỊCH CHUYỂN (TELEPORTER)
 * 
 * BƯỚC 1: TẠO CỔNG DỊCH CHUYỂN
 * 1. Tạo một GameObject mới trong scene
 * 2. Đặt tên là "Teleporter" hoặc tên bạn muốn
 * 3. Thêm các component sau:
 *    - SpriteRenderer (nếu có sprite)
 *    - Animator (nếu có animation biến mất)
 *    - BoxCollider2D (đặt IsTrigger = true)
 *    - Teleporter script
 * 
 * BƯỚC 2: SETUP ANIMATION (TÙY CHỌN)
 * 1. Tạo Animation Controller cho teleporter
 * 2. Tạo animation "Disappear" với hiệu ứng biến mất
 * 3. Thêm trigger parameter "Disappear" trong Animator
 * 4. Gán Animator vào teleporterAnimator field
 * 
 * BƯỚC 3: TẠO UI CHỌN MÀN CHƠI
 * 
 * PHƯƠNG ÁN A: SỬ DỤNG LevelSelectionUI (Tự động tạo buttons)
 * 1. Tạo GameObject mới, đặt tên "LevelSelectionUI"
 * 2. Thêm component LevelSelectionUI
 * 3. Tạo child GameObject "ButtonContainer" (nếu muốn tùy chỉnh vị trí buttons)
 * 4. Gán ButtonContainer vào buttonContainer field
 * 5. Tạo close button và gán vào closeButton field
 * 6. Tạo title text và gán vào titleText field
 * 7. Đặt tag "LevelSelectionUI" cho GameObject này
 * 
 * PHƯƠNG ÁN B: SỬ DỤNG SimpleLevelSelectionUI (Setup thủ công)
 * 1. Tạo GameObject mới, đặt tên "SimpleLevelSelectionUI"
 * 2. Thêm component SimpleLevelSelectionUI
 * 3. Tạo các Button cho từng level và gán vào levelButtons array
 * 4. Tạo close button và gán vào closeButton field
 * 5. Tạo title text và gán vào titleText field
 * 6. Đặt tag "LevelSelectionUI" cho GameObject này
 * 
 * BƯỚC 4: SETUP TELEPORTER SCRIPT
 * 1. Gán Animator vào teleporterAnimator field (nếu có)
 * 2. Gán UI GameObject vào levelSelectionUI field
 * 3. Chọn useSimpleUI = true nếu dùng SimpleLevelSelectionUI
 * 4. Điều chỉnh các thông số khác:
 *    - interactionRange: khoảng cách tương tác
 *    - disappearDelay: thời gian chờ trước khi hiện UI
 *    - availableLevels: tên các scene có thể chuyển đến
 *    - levelDisplayNames: tên hiển thị trong UI
 * 
 * BƯỚC 5: TESTING
 * 1. Đảm bảo Player có tag "Player"
 * 2. Đảm bảo Player có PlayerController1 component với interactAction
 * 3. Chạy game và di chuyển player đến gần teleporter
 * 4. Nhấn E để kích hoạt teleporter
 * 5. Kiểm tra animation biến mất và UI hiện lên
 * 
 * LƯU Ý:
 * - Đảm bảo tất cả scene được liệt kê trong Build Settings
 * - Player phải có Collider2D để trigger hoạt động
 * - UI phải có Canvas component và được setup đúng
 * - Nếu dùng animation, đảm bảo trigger parameter đúng tên
 */

public class TeleporterSetupGuide : MonoBehaviour
{
    // Script này chỉ chứa hướng dẫn, không có logic gì
    void Start()
    {
        Debug.Log("TeleporterSetupGuide: Vui lòng đọc comment trong script này để setup Teleporter");
    }
} 
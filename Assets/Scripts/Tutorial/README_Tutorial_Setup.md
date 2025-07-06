# Tutorial System Setup Guide

## Tổng quan
Hệ thống tutorial này cho phép tạo các hướng dẫn tương tác cho người chơi, yêu cầu họ thực hiện đúng input để tiến tới bước tiếp theo.

## Các file chính

### 1. TutorialManager.cs
- **Chức năng**: Quản lý toàn bộ hệ thống tutorial
- **Tính năng**:
  - Hiển thị text hướng dẫn
  - Kiểm tra input từ người chơi
  - Chuyển đổi giữa các bước tutorial
  - Hiệu ứng fade in/out
  - Âm thanh cho từng bước

### 2. Map1TutorialSetup.cs
- **Chức năng**: Setup các bước tutorial cụ thể cho Map 1
- **Tính năng**:
  - Định nghĩa 11 bước tutorial cơ bản
  - Kết nối với Input Actions của player
  - Thiết lập điều kiện cho từng bước

### 3. TutorialUISetup.cs
- **Chức năng**: Tạo UI elements cho tutorial
- **Tính năng**:
  - Tạo tutorial panel
  - Tạo buttons và text
  - Tạo arrow indicator
  - Setup Canvas tự động

## Cách Setup trong Unity

### Bước 1: Tạo Tutorial UI
1. Tạo một empty GameObject trong scene
2. Thêm component `TutorialUISetup`
3. Click chuột phải vào component → "Create Tutorial UI"
4. Click "Setup Tutorial in Scene"

### Bước 2: Kết nối Input Actions
1. Chọn GameObject có `Map1TutorialSetup`
2. Trong Inspector, kéo các Input Actions từ PlayerController1:
   - Move Action
   - Jump Action
   - Attack Action
   - Attack2 Action
   - Attack3 Action
   - Spell1 Action
   - Spell2 Action
   - Defend Action
   - Dash Action
   - Spell3 Action

### Bước 3: Setup Tutorial Objects
1. Tạo các GameObject cho tutorial:
   - **Enemy Target**: Enemy để player tấn công
   - **Platform Target**: Platform để player nhảy lên
   - **Spell Target**: Target để player cast spell
2. Kéo các GameObject này vào các slot tương ứng trong `Map1TutorialSetup`

### Bước 4: Tùy chỉnh Tutorial Steps
Trong `Map1TutorialSetup`, bạn có thể:
- Thay đổi text hướng dẫn
- Điều chỉnh vị trí tutorial panel
- Thêm/bớt các bước tutorial
- Tùy chỉnh điều kiện cho từng bước

## Các bước Tutorial hiện tại

### 1. Welcome & Movement
- **Input**: WASD
- **Mô tả**: Hướng dẫn di chuyển cơ bản

### 2. Jumping
- **Input**: SPACE
- **Điều kiện**: Player phải ở gần platform
- **Mô tả**: Hướng dẫn nhảy lên platform

### 3. Basic Attack
- **Input**: LEFT CLICK
- **Điều kiện**: Player phải ở gần enemy
- **Mô tả**: Hướng dẫn tấn công cơ bản

### 4. Attack 2
- **Input**: RIGHT CLICK
- **Mô tả**: Hướng dẫn tấn công mạnh hơn

### 5. Attack 3
- **Input**: Q
- **Mô tả**: Hướng dẫn tấn công mạnh nhất

### 6. Defend
- **Input**: SHIFT (giữ)
- **Thời gian giữ**: 1.5 giây
- **Mô tả**: Hướng dẫn phòng thủ

### 7. Dash
- **Input**: E
- **Mô tả**: Hướng dẫn dash

### 8. Spell 1
- **Input**: R
- **Điều kiện**: Player phải ở gần spell target
- **Mô tả**: Hướng dẫn bắn chưởng lửa

### 9. Spell 2
- **Input**: F
- **Mô tả**: Hướng dẫn spell mạnh nhất

### 10. Spell 3 (Transform)
- **Input**: T
- **Mô tả**: Hướng dẫn biến hình

### 11. Final Instructions
- **Input**: Không cần
- **Mô tả**: Thông báo hoàn thành tutorial

## Tùy chỉnh nâng cao

### Thêm Tutorial Step mới
```csharp
TutorialStep newStep = new TutorialStep
{
    stepName = "Custom Step",
    instructionText = "Hướng dẫn tùy chỉnh...",
    inputDisplayName = "CUSTOM KEY",
    requiredInput = customInputAction,
    tutorialPanelPosition = new Vector2(0, 200),
    requireHold = false
};
tutorialManager.AddTutorialStep(newStep);
```

### Điều kiện tùy chỉnh
- `requirePlayerGrounded`: Player phải ở trên mặt đất
- `requirePlayerInRange`: Player phải ở gần target object
- `rangeDistance`: Khoảng cách tối đa

### Visual Settings
- `showArrow`: Hiển thị mũi tên chỉ hướng
- `arrowPosition`: Vị trí mũi tên
- `arrowRotation`: Góc xoay mũi tên
- `tutorialPanelPosition`: Vị trí panel hướng dẫn

## Troubleshooting

### Tutorial không hiển thị
1. Kiểm tra Canvas có tồn tại không
2. Kiểm tra TutorialManager có được kết nối đúng không
3. Kiểm tra Input Actions có được assign đúng không

### Input không hoạt động
1. Kiểm tra Input Actions có được Enable không
2. Kiểm tra Input System có được setup đúng không
3. Kiểm tra PlayerController1 có Input Actions tương ứng không

### Tutorial bị stuck
1. Kiểm tra điều kiện của tutorial step
2. Kiểm tra target objects có tồn tại không
3. Reset tutorial bằng `ResetTutorial()`

## API Reference

### TutorialManager
```csharp
// Public Properties
bool IsTutorialActive { get; }
int CurrentStepIndex { get; }
int TotalSteps { get; }

// Public Methods
void StartTutorial()
void ResetTutorial()
void AddTutorialStep(TutorialStep step)
void SkipTutorial()

// Events
OnTutorialStart
OnTutorialComplete
OnTutorialStepComplete
```

### Map1TutorialSetup
```csharp
// Public Methods
void StartTutorial()
void ResetTutorial()
bool IsTutorialActive()
```

## Tips & Best Practices

1. **Test từng bước**: Kiểm tra từng tutorial step riêng lẻ
2. **Sử dụng Debug.Log**: Thêm log để debug
3. **Tối ưu performance**: Disable tutorial khi không cần thiết
4. **User Experience**: Đảm bảo tutorial không quá dài hoặc khó hiểu
5. **Accessibility**: Cung cấp option skip tutorial

## Ví dụ sử dụng

```csharp
// Trong script khác
public class GameManager : MonoBehaviour
{
    public TutorialManager tutorialManager;
    
    void Start()
    {
        // Bắt đầu tutorial khi game start
        if (tutorialManager != null)
        {
            tutorialManager.OnTutorialComplete += OnTutorialFinished;
            tutorialManager.StartTutorial();
        }
    }
    
    void OnTutorialFinished()
    {
        Debug.Log("Tutorial completed! Starting game...");
        // Bắt đầu game logic
    }
}
``` 
# Tutorial System Setup Guide

## Tổng quan
Hệ thống tutorial này cho phép tạo các hướng dẫn tương tác cho người chơi, yêu cầu họ thực hiện đúng input để tiến tới bước tiếp theo. **Hệ thống cũng có tính năng lưu trạng thái tutorial để không hiển thị lại nếu player đã hoàn thành.**

## Tính năng mới: Lưu trạng thái Tutorial
- **TutorialDataManager**: Quản lý việc lưu trữ trạng thái tutorial sử dụng PlayerPrefs
- **Tự động kiểm tra**: Tutorial sẽ không hiển thị lại nếu player đã hoàn thành
- **Version control**: Tự động reset tutorial khi có update mới
- **Debug tools**: Công cụ debug để test và quản lý tutorial

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

### 4. TutorialDataManager.cs (MỚI)
- **Chức năng**: Quản lý việc lưu trữ trạng thái tutorial
- **Tính năng**:
  - Lưu trạng thái hoàn thành tutorial
  - Kiểm tra version để reset khi có update
  - Singleton pattern để truy cập từ mọi nơi
  - API để quản lý tutorial data

### 5. TutorialDebugUI.cs (MỚI)
- **Chức năng**: Debug UI để test và quản lý tutorial
- **Tính năng**:
  - Hiển thị thông tin debug về trạng thái tutorial
  - Force show tutorial (bỏ qua trạng thái đã hoàn thành)
  - Reset tutorial data
  - Toggle debug panel với phím F1

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

### Bước 5: Setup Tutorial Data Manager (MỚI)
1. Tạo một empty GameObject trong scene
2. Thêm component `TutorialDataManager`
3. GameObject này sẽ tự động trở thành singleton và persist qua các scene

### Bước 6: Setup Debug UI (TÙY CHỌN)
1. Tạo một empty GameObject trong scene
2. Thêm component `TutorialDebugUI`
3. Tạo UI panel với các elements:
   - Text để hiển thị debug info
   - Button "Show Tutorial" để force hiển thị tutorial
   - Button "Reset Tutorial" để reset tutorial data
   - Button "Hide Debug" để ẩn debug panel
4. Kéo các UI elements vào các slot tương ứng trong `TutorialDebugUI`

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
4. **Kiểm tra TutorialDataManager có tồn tại không**
5. **Kiểm tra tutorial đã được hoàn thành chưa (sử dụng debug UI)**

### Tutorial không lưu trạng thái
1. Kiểm tra TutorialDataManager có được setup đúng không
2. Kiểm tra `checkTutorialCompletion` có được bật không
3. Kiểm tra tutorial có được complete đúng cách không
4. Sử dụng debug UI để kiểm tra trạng thái

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
bool ShouldShowTutorial() // MỚI
void ResetTutorialData() // MỚI
string GetTutorialDebugInfo() // MỚI

// Events
OnTutorialStart
OnTutorialComplete
OnTutorialStepComplete

// Settings
bool checkTutorialCompletion = true // MỚI
bool forceShowTutorial = false // MỚI
```

### TutorialDataManager
```csharp
// Singleton Access
TutorialDataManager.Instance

// Public Methods
bool IsTutorialCompleted()
void MarkTutorialCompleted()
void SaveTutorialStep(int stepIndex)
int GetLastTutorialStep()
void ResetTutorialData()
bool ShouldShowTutorial()
void ForceShowTutorial()
string GetDebugInfo()
```

### TutorialDebugUI
```csharp
// Public Methods
void ToggleDebugPanel()
void ShowDebugPanel()
void HideDebugPanel()
void ForceShowTutorial()
void ResetTutorialData()
void UpdateDebugInfo()

// Settings
KeyCode toggleDebugKey = KeyCode.F1
bool showDebugOnStart = false
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
6. **Lưu trạng thái**: Luôn bật `checkTutorialCompletion` để tránh hiển thị lại tutorial
7. **Debug tools**: Sử dụng TutorialDebugUI để test và quản lý tutorial
8. **Version control**: Tăng `CURRENT_TUTORIAL_VERSION` khi có update tutorial

## Sử dụng hệ thống lưu trạng thái Tutorial

### Cách hoạt động
1. **Lần đầu chơi**: Tutorial sẽ hiển thị bình thường
2. **Sau khi hoàn thành**: Tutorial sẽ được đánh dấu là đã hoàn thành
3. **Lần sau vào game**: Tutorial sẽ không hiển thị lại
4. **Khi có update**: Tutorial sẽ tự động reset và hiển thị lại

### Kiểm tra trạng thái tutorial
```csharp
// Kiểm tra tutorial đã hoàn thành chưa
bool isCompleted = TutorialDataManager.Instance.IsTutorialCompleted();

// Kiểm tra có nên hiển thị tutorial không
bool shouldShow = TutorialDataManager.Instance.ShouldShowTutorial();

// Force hiển thị tutorial (bỏ qua trạng thái đã hoàn thành)
TutorialDataManager.Instance.ForceShowTutorial();
```

### Reset tutorial data
```csharp
// Reset tutorial để hiển thị lại
TutorialDataManager.Instance.ResetTutorialData();

// Hoặc sử dụng từ TutorialManager
tutorialManager.ResetTutorialData();
```

### Debug tutorial
```csharp
// Lấy thông tin debug
string debugInfo = TutorialDataManager.Instance.GetDebugInfo();
Debug.Log(debugInfo);

// Sử dụng debug UI
TutorialDebugUI debugUI = FindObjectOfType<TutorialDebugUI>();
debugUI.ShowDebugPanel();
```

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
# WPF Dock Panel Project

C# WPF 애플리케이션으로 Visual Studio와 유사한 도킹 패널 시스템을 구현한 예제입니다.

## 주요 기능

### 1. 기본 도킹 패널
- **Toolbox** (좌측): 컨트롤 목록
- **Properties** (우측): 속성 편집기
- **Solution Explorer** (상단): 솔루션 탐색기
- **Output** (하단): 출력 창
- **Main Content** (중앙): 작업 영역

### 2. Dock/Undock 기능
- 📌 버튼 클릭으로 패널을 별도 창으로 분리
- 메뉴를 통한 패널 표시/숨김 제어
- 분리된 창을 닫으면 자동으로 원래 위치에 재도킹

### 3. 동적 크기 조절
- **GridSplitter** 사용으로 실시간 크기 조절
- 패널이 undock되면 나머지 영역이 자동으로 확장
- 최소 크기 제한으로 UI 안정성 보장

### 4. 드래그 앤 드롭 도킹 (구현 중)
- 플로팅 창을 드래그하여 도킹 위치 선택
- 도킹 인디케이터로 시각적 가이드 제공
- Visual Studio와 유사한 사용자 경험

## 프로젝트 구조

```
WpfDockProject/
├── MainWindow.xaml              # 메인 윈도우 레이아웃
├── MainWindow.xaml.cs           # 메인 윈도우 로직
├── FloatingWindow.xaml          # 플로팅 창 레이아웃
├── FloatingWindow.xaml.cs       # 플로팅 창 드래그 로직
├── DockIndicator.xaml           # 도킹 인디케이터 UI
├── DockIndicator.xaml.cs        # 도킹 인디케이터 로직
├── App.xaml                     # 앱 설정
├── App.xaml.cs                  # 앱 로직
└── WpfDockProject.csproj        # 프로젝트 파일
```

## 실행 방법

### 요구사항
- .NET 6.0 이상
- Windows 운영체제

### 빌드 및 실행
```bash
# 프로젝트 클론
git clone <repository-url>
cd WpfDockProject

# 빌드
dotnet build

# 실행
dotnet run
```

## 사용법

1. **패널 분리**: 각 패널 상단의 📌 버튼 클릭
2. **패널 숨기기/보이기**: 상단 메뉴의 Controls > 패널명 체크/해제
3. **크기 조절**: 회색 구분선(GridSplitter)을 마우스로 드래그
4. **재도킹**: 분리된 창을 닫거나 메인 창으로 드래그 (구현 중)

## 기술적 특징

### 사용된 WPF 컨트롤
- `Grid` + `GridSplitter`: 동적 크기 조절
- `DockPanel`: 기본 레이아웃 구조
- `Canvas`: 도킹 인디케이터 오버레이
- `Border`: 패널 컨테이너
- `MenuItem`: 패널 제어 메뉴

### 핵심 구현 내용
- **동적 그리드 관리**: 패널 상태에 따른 행/열 크기 자동 조절
- **플로팅 윈도우**: 커스텀 창으로 패널 내용 이동
- **드래그 감지**: 마우스 이벤트 기반 드래그 앤 드롭
- **상태 관리**: 패널별 원본 크기 및 위치 저장

## 알려진 제한사항

1. **드래그 앤 드롭 도킹**: 현재 구현 중이며 인디케이터가 제대로 표시되지 않을 수 있음
2. **레이아웃 복원**: 일부 상황에서 패널 위치가 완전히 복원되지 않을 수 있음
3. **다중 모니터**: 다중 모니터 환경에서의 테스트 미완료

## 향후 개선 방향

1. **AvalonDock 통합**: 더 안정적인 도킹 시스템을 위한 라이브러리 적용
2. **설정 저장**: 사용자 레이아웃 설정 저장/복원 기능
3. **테마 지원**: 다크/라이트 테마 전환 기능
4. **추가 도킹 위치**: 탭 기반 도킹 및 중첩 패널 지원

## 라이선스

MIT License

## 기여하기

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request
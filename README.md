## GameObjects To Terrain

**이 스크립트는 `Unity`에서 `GameObjects`의 `Mesh` 정보를 기반으로 `Terrain`을 만드는 스크립트입니다.**



<img width="511" alt="image" src="https://user-images.githubusercontent.com/46314169/162799034-716633dd-ae71-41ad-867a-d3480b657419.png">



### How To Use..

1. `Meshes2Terrain.cs`를 `UnityProject/Assets/Editor` 위치에 저장합니다.
2. `Scene Hierachy`에서 `Terrain`으로 옮기고 싶은 `Objects`를 선택한 후 유니티 상단 바에 추가된 `Terrain`버튼을 누르고 `Meshes To Terrain`을 누릅니다.
3. `Terrain`의 정밀도를 정의하는 `Resolution`값을 설정하고 `Create Terrain`버튼을 클릭합니다.
4. 그러면 `Scene Hierachy`에 `Terrain`이름으로 추출되는 것을 확인할 수 있습니다.


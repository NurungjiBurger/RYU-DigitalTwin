# 삼성 청년 SW 아카데미 공통 프로젝트 AtwoZ팀 - 류(流)

# 류 UCC
<img src="./images/류UCC.gif" height="400"/>

<br>

![류봇](https://github.com/user-attachments/assets/0a9328fb-1198-43b5-8e66-0dabf38b1a1e)


### 디지털 트윈
<img src="./images/디지털트윈기능.gif" height="400"/>

## 상세설명

#### Unity와 ROS2 통신

<img src="./images/ROS설정.png" height="400"/>

- **디지털 트윈**: Unity를 활용하여 가상 환경에서 로봇의 동작을 시뮬레이션하고, 이를 실제 환경과 동기화한다. 이를 위해 rosbridge_websocket을 통해 Unity와 ROS2 간의 통신을 구현한다.<br>

### 

- **맵핑**: Unity에서 생성된 가상 맵과 실제 환경의 맵을 매핑한다. 현실 세계의 좌표계를 일치시키기 위해 좌표 축을 재정렬하였으며, 위치 좌표는 X, Y, Z에서 -Y, Z, X로 매핑되고, 회전 좌표는 X, Y, Z, W에서 Y, -Z, X, W로 매핑된다.<br>

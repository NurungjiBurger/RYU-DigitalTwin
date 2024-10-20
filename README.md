# 삼성 청년 SW 아카데미 공통 프로젝트 AtwoZ팀 - 류(流)


![류봇](https://github.com/user-attachments/assets/0a9328fb-1198-43b5-8e66-0dabf38b1a1e)

## 작품 소개
주문 요청을 분석하여 입력된 지도를 기반으로 효율적인 경로를 계산한 후, 최적화된 동선으로 경로를 설계한다. 직원들은 각 섹터에서 대기하다 로봇이 도착하면 WearOS에 뜬 물건을 로봇 위에 옮겨담는 작업을 수행한 뒤 로봇에게 완료 신호를 전송한다.

모든 물건을 전달받은 로봇은 최종적으로 컨베이어 벨트에 물건을 하역한 뒤, 최초 대기 장소로 돌아오는 물류 자동화 시스템을 제공한다.


## 개발 목적
물류 창고에서 작업하는 직원들은 일반적으로 PDA를 사용한다. PDA를 조작할 때 두 손을 사용하므로 물건을 옮기거나 포장하는 등의 작업을 수행하기 어려우며, PDA를 떨어뜨렸을 때 기기가 손상될 위험도 존재한다.

이러한 문제를 해결하고자 PDA를 스마트워치로 대체하고자 하였다. 실시간으로 작업 지시를 손쉽게 확인할 수 있고, 기기의 손상 위험도 줄일 수 있으며 경량화된 기기로 장시간 작업 시 피로도를 낮출 수 있다.

물류 작업 전반에서도 문제가 발생할 수 있다. 기존 물류 프로세스에서는 섹터마다 카트를 끌고 물건을 운반한 후, 직접 컨베이어 벨트로 물건을 하역하는 과정을 거친다. 이 과정에서 **비효율적인 동선**으로 인해 불필요한 이동이 발생할 수 있으며, 좁은 작업 공간에서 다수의 직원이 동시에 작업할 경우 **사고 위험성이 증가**할 수 있다.

이를 해결하기 위해 자율 주행 로봇을 활용한 물류 자동화 시스템을 구축하였다. 최적화된 동선 설계를 통해 작업 효율성을 극대화하고, 직원들의 물리적 부담을 줄이며, 작업 환경의 안전성을 향상시키는 것을 목표로 하였다.

<br>

## 동작 사진

### 최적화 경로 탐색
<img src="https://github.com/user-attachments/assets/1e6291f2-d67d-4bd9-b78c-6468d0391ab7" width="400"/>
<img src="https://github.com/user-attachments/assets/ea3a7d25-4d34-4462-91ef-454c48aa037c" width="400"/>

### wear OS 활용
<img src="https://github.com/user-attachments/assets/948d3aea-3edc-419a-ab9a-3aac98cd911f" height="500"/>

### 자동 하역 시스템
<img src="https://github.com/user-attachments/assets/d8fd860e-7e86-43cb-bb0b-403afe16e461" height="500"/>

### 디지털 트윈
<img src="./images/디지털트윈기능.gif" height="400"/>

<!--

## 임베디드 활용 범위
1. 로봇 제어 및 경로 탐색 : 자율주행 및 경로 탐색을 임베디드 시스템으로 실시간 처리
2. 웹 기반 명령 제어 : 웹 인터페이스를 통해 로봇에 명령을 내리며, mqtt 프로토콜을 통해 임베디드 시스템과 통신
3. wear OS를 통한 제어 : wearOS를 활용해 로봇을 제어하고 임베디드 시스템이 이를 반영
4. 센서 데이터 처리 및 통신 : 로봇에 탑재된 센서 데이터를 실시간 처리

-->

## 작동 설명
#### 1. 하드웨어: 서보모터와 초음파 센서<br>
- **서보모터**: 로봇의 움직임을 제어한다. 제자리 회전, 정방향 후진, 미세 전진 등을 구현하기 위해 서보모터를 활용하여 로봇의 방향과 위치를 조정한다.<br>
- **초음파 센서**: 물품 하역 시 사용한다. ESP32 마이크로컨트롤러를 통해 초음파 센서로부터 거리를 측정한 후, 이 정보를 MQTT 프로토콜을 통해 전송한다. 로봇이 컨베이어벨트와의 거리가 일정 기준 이하로 가까워지면, 서보모터를 작동시켜 물품을 하역한다.

#### 2. 맵 매핑과 경로 탐색: SLAM과 Nav2
- **SLAM (Simultaneous Localization and Mapping)**:  라이다센서를 사용하여 로봇이 주행할 환경을 스캔하고 맵핑합니다. 이 과정에서 로봇은 자신의 위치를 파악하면서 동시에 주행 환경의 지도를 생성합니다.  
- **Nav2**: nav2를 이용해 생성된 맵을 기반으로 목적지 까지의 최적의 경로를 설정하고 로봇이 설정된 경로를 따라 주행할 수 있도록 합니다. 

#### 3. 물품 하역: 초음파 센서와 서보모터
- 초음파 센서를 통해 컨베이어벨트와의 거리를 측정하고, 일정 거리 이하로 가까워지면 하역 작업을 수행한다.
- 서보모터를 이용하여 로봇이 정확한 위치에서 물품을 하역할 수 있도록 돕는다.
  

#### 4. 회전 매커니즘
- U턴 형태로 회전하면서 제자리에서 방향을 전환한다. 이를 위해, 회전, 미세전진, 정방향 후진을 반복하는 과정을 통해 로봇의 위치(X, Y 좌표)가 바뀌지 않도록 제자리 회전을 구현하였다.

#### 5. WearOS와 주문 관리
- **WearOS**를 이용하여 주문 관리 시스템을 구축했다. 하나의 주문이 완료될 때까지 다른 주문은 MQTT를 통해 블로킹되며, 예외 상황에 대비해 물류 정보를 내부 데이터베이스에 저장한다. 싱글톤 패턴을 적용하여 애플리케이션의 라이프사이클을 관리한다.


#### 6. 디지털 트윈: Unity와 ROS2 통신
- **디지털 트윈**: Unity를 활용하여 가상 환경에서 로봇의 동작을 시뮬레이션하고, 이를 실제 환경과 동기화한다. 이를 위해 rosbridge_websocket을 통해 Unity와 ROS2 간의 통신을 구현한다.<br>
- **맵핑**: Unity에서 생성된 가상 맵과 실제 환경의 맵을 매핑한다. 현실 세계의 좌표계를 일치시키기 위해 좌표 축을 재정렬하였으며, 위치 좌표는 X, Y, Z에서 -Y, Z, X로 매핑되고, 회전 좌표는 X, Y, Z, W에서 Y, -Z, X, W로 매핑된다.<br>


## 시연 영상

[![image](https://github.com/user-attachments/assets/a20a8cb3-d86d-4bea-87bd-b7020dd34202)](https://www.youtube.com/watch?v=dOtYxKFDqmM)


## System Architecture
![시스템아키텍처](https://github.com/user-attachments/assets/620ec243-396c-47e3-ae7f-7ba7883bf9c6)
<br>

## 개발 환경 및 기술 스택
<table>
<tbody>
<tr>
<td>운영체제</td>
<td>Windows 11, Ubuntu 20.04.6 LTS</td>
</tr>
<tr>
<td>디바이스 구성</td>
<td>Raspberry Pi</td>
</tr>
<tr>
<td>IDE</td>
<td>Visual Studio Code, IntelliJ, Android Studio, Unity</td>
</tr>
<tr>
<td>개발 언어</td>
<td>Java 17.0.12, C/C++, Python, Kotlin, C#</td>
</tr>
<tr>
<td>데이터베이스</td>
<td>MongoDB, MariaDB</td>
</tr>
<tr>
<td>컨테이너화 도구</td>
<td>Docker 27.1.1 (Ubuntu 20.04.6 LTS)</td>
</tr>
<tr>
<td>프로젝트 관리 도구</td>
<td>Jira, Notion, Slack</td>
</tr>
<tr>
<td>임베디드 시스템</td>
<td>Raspberry Pi, Arduino</td>
</tr>
<tr>
<td>통신 프로토콜</td>
<td>MQTT, HTTP/HTTPS, UWB</td>
</tr>
</tbody>
</table>


<br>



## TEAM INFO


| 윤지욱 | 강형남 | 김재현 | 박상빈 | 이현주 |
| :---: | :---: | :---: | :---: | :---: |
| <a href="https://github.com/YunJiUk"><img src="https://avatars.githubusercontent.com/u/117324719?v=4" width="100px;" alt=""/></a> | <a href="https://github.com/sunkk8482"><img src="https://avatars.githubusercontent.com/u/86597542?v=4" width="100px;" alt=""/></a> | <a href="https://github.com/jaehyun565"><img src="https://avatars.githubusercontent.com/u/99954264?v=4" width="100px;" alt=""/></a> | <a href="https://github.com/NurungjiBurger"><img src="https://avatars.githubusercontent.com/u/44452761?v=4" width="100px;" alt=""/></a> | <a href="https://github.com/Labriever"><img src="https://avatars.githubusercontent.com/u/130520505?v=4" width="100px;" alt=""/></a> |
| <a href="https://github.com/YunJiUk">팀장</a> | <a href="https://github.com/sunkk8482">팀원</a> | <a href="https://github.com/jaehyun565">팀원</a> | <a href="https://github.com/NurungjiBurger">팀원</a> | <a href="https://github.com/Labriever">팀원</a> |



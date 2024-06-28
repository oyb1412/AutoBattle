### 

# Auto Battle


---

## Description

---


- 🔊프로젝트 소개

  Auto Battle은 유닛을 구매해 자동으로 전투를 진행하는 2D 오토배틀류 게임입니다. 옵저버 패턴을 적극적으로 활용해 Update 함수의 사용을 최소한으로 줄이며, UI의 변화 또한 액션 변수를 활용해 보다 최적화에 중점을 두었습니다.

       

- 개발 기간 : 2024.01.31 - 2024.02.07

- 🛠️사용 기술

   -언어 : C#

   -엔진 : Unity Engine

   -데이터베이스 : 로컬

   -개발 환경: Windows 10, Unity 2021.3.10f1



- 💻구동 화면

![스크린샷(18)](https://github.com/oyb1412/AutoBattle/assets/154235801/adb7abdd-9d95-48f1-86ea-86b9c4b54bd8)

## 목차

---

- 기획 의도
- 핵심 로직


### 기획 의도

---

- 전투가 자동으로 진행되는 '방치형' 게임 개발

- 옵저버 패턴을 적극 활용해, 보다 최적화된 게임 개발

### 핵심 로직

---
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・retry mechanism을 이용한 중복되지 않는 유닛 목록 호출 시스템

유닛 목록 호출 시, retry mechanism을 이용해 항상 중복되지 않는 별개의 유닛이 호출

![1](https://github.com/oyb1412/AutoBattle/assets/154235801/46655481-2f39-4ac0-bd04-255c7906891d)

![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)


### ・옵저버 패턴을 이용해 조건 체크

반복적으로 조건을 체크하는것이 아닌, 옵저버 패턴을 이용해 이벤트 형식으로 필요한 상황에만 조건을 체크

![그림5](https://github.com/oyb1412/AutoBattle/assets/154235801/1f9e1969-68f8-4bd8-bd97-b90bc249cdd9)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・레이캐스트를 이용한 마우스 오브젝트 선택

유닛, 아이템 등 각종 오브젝트들을 마우스로 선택할 수 있는 기능 구현

![4](https://github.com/oyb1412/AutoBattle/assets/154235801/d5307142-0cdf-4e26-9ee9-a3502b748997)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・ScriptableObject를 이용한 데이터 관리

고정 데이터를 스크립터블 오브젝트를 이용해 저장, 데이터 관리의 용이성 극대화

![11](https://github.com/oyb1412/AutoBattle/assets/154235801/2aceb4f5-c60b-44a9-a742-a1946ea49ba4)

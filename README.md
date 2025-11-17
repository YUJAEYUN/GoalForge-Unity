<h1 align="center">⚽ Unity Soccer Game Project</h1>

<p align="center">
  <b>1vs1 캐주얼 축구 게임</b><br/>
  Start → Team Select → Coin Toss → Main Match로 이어지는 깔끔한 게임 흐름 구현 프로젝트
</p>

---

## 📌 프로젝트 개요

이 프로젝트는 Unity를 사용하여 제작한 1vs1 축구 게임입니다.  
간단한 조작과 슈퍼 스킬을 활용해 경기의 재미를 강화한 것이 특징입니다.

---

## 🎮 주요 기능 (Features)

### 🔷 **1. Team Select**
- 플레이어가 사용할 팀 선택
- 선택 시 버튼 하이라이트 효과
- Confirm 버튼 활성화 / 비활성화 제어
- GameManager를 통한 팀 정보 유지

---
### 🔷 **2. Level Select**
- 쉬움 / 어려움 난이도
- 난이도에 따라 AI 패턴, 능력 강화

---

### 🔷 **3. Coin Toss**
- 동전 던지기 애니메이션
- 승패에 따라 킥오프 팀 결정
- MainScene에 결과 전달

---

### 🔷 **4. Main Match Scene**
- 경기장 구조, 포지션 배치
- 타이머(전반 / 후반), 점수판 UI
- 플레이어 조작
  - 이동, 패스, 슛, 플레이어 전환
- 공 물리 시스템
  - 드리블 / 패스 / 슛 상태
  - 충돌 후 소유권 전환
- 기본 AI 시스템
  - 공 쪽 접근
  - 패스/전진/후진 판단

---

### 🔷 **5. Super Skill System**
- 슈퍼 게이지
- 슈퍼 슛: 화염 이펙트, 카메라 줌, 슬로우 모션
- 슈퍼 갓 핸드: 골키퍼 특수 방어
- 슈퍼 vs 슈퍼 충돌 연출 (Space 연타)

---

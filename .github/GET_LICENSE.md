# Unity 라이선스 추출 방법 (Unity 6)

Unity 6는 새로운 라이선스 시스템을 사용합니다. GitHub Actions에서 사용하려면 다음 방법을 따르세요.

## 방법 1: Unity Editor에서 라이선스 추출

1. Unity Editor 열기 (프로젝트 아무거나)
2. 메뉴: **Unity → Manage License** (또는 **Help → Manage License**)
3. 라이선스 정보 확인
4. 터미널에서 다음 명령어 실행:

```bash
# Unity 6의 라이선스는 다른 위치에 있을 수 있습니다
ls -la ~/Library/Application\ Support/Unity/
ls -la /Library/Application\ Support/Unity/

# 또는 전체 검색
find ~ -name "*.ulf" 2>/dev/null | grep -i unity
```

## 방법 2: Unity CLI로 라이선스 생성 (권장)

Unity 6부터는 CLI를 통해 라이선스를 관리합니다:

```bash
# Unity 설치 경로 확인
ls /Applications/Unity/Hub/Editor/

# Unity CLI로 라이선스 반환 (XML 형식)
/Applications/Unity/Hub/Editor/6000.1.12f1/Unity.app/Contents/MacOS/Unity \
  -quit -batchmode -nographics \
  -logFile - \
  -username "YOUR_UNITY_EMAIL" \
  -password "YOUR_UNITY_PASSWORD"
```

## 방법 3: GitHub Actions에서 자동 활성화 (가장 간단)

라이선스 파일 없이 이메일/비밀번호만으로 자동 활성화:

GitHub Secrets에 다음만 추가:

- `UNITY_EMAIL`: Unity 계정 이메일
- `UNITY_PASSWORD`: Unity 계정 비밀번호

`deploy.yml`에서 `UNITY_LICENSE`를 제거하고 이메일/비밀번호만 사용하면 자동으로 활성화됩니다.

## 주의사항

- Personal 라이선스는 동시에 2개 기기에서만 활성화 가능
- GitHub Actions도 1개 기기로 카운트됨
- 라이선스 정보는 절대 공개하지 마세요

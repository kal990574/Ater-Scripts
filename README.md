 # 🗝 Ater (āter)                                                                                                                 
                                                                                                                                 
  > 어둠을 스캔해 단서를 잇는 1인칭 호러 퍼즐 탈출 게임                                                                            
                                                                                                                                   
  [![YouTube](https://img.shields.io/badge/YouTube-Trailer-red?logo=youtube)](https://www.youtube.com/watch?v=Sb3PPvMQYA8)         
                                                                                                                                   
  ## 📋 프로젝트 개요                                                                                                              
                  
  | 항목 | 내용 |                                                                                                                  
  |:---:|:---|                                                                                                                     
  | **개발 기간** | 2026.03.16 ~ 2026.04.24 (총 40일) |                                                                            
  | **팀 구성** | 클라이언트 3명, 기획 1명 |                                                                                       
  | **플랫폼** | Windows PC |                                                                                                      
  | **장르** | 1인칭 호러 / 방탈출 / 퍼즐 |                                                                                        
  | **담당 역할** | PM, 코어 아키텍처, 플레이어 컨트롤러, 소나 스캔, AI 힌트, 공포 구간, 세이브 시스템 |                           
                                                                                                                                   
  ## 🎮 게임 소개                                                                                                                  
                                                                                                                                   
  **Ater**는 극도로 어두운 공간에서 소나와 레이저 두 종류의 스캔을 활용하여 단서를 찾고 퍼즐을 풀어 탈출하는 1인칭 호러 게임입니다.
   
  플레이어는 제한된 소나 스캔으로 어둠 속 지형을 파악하고, 레이저 스캔으로 숨겨진 오브젝트를 활성화하여 단서를 수집합니다. 수집한  
  단서를 조합해 퍼즐을 풀고, 다음 챕터로 탈출하세요.
                                                                                                                                   
  ## ⭐ 주요 기능 및 콘텐츠

  ### 🔦 소나 스캔 시스템                                                                                                          
  - URP 풀스크린 셰이더 기반 부채꼴 파동 렌더링
  - 최대 4개 파동 동시 표현 (다중 링 구조)                                                                                         
  - OverlapSphere + cos 내적 필터링으로 부채꼴 범위 오브젝트 감지                                                                  
  - AnimationCurve 역함수 이분 탐색으로 파동-오브젝트 하이라이트 타이밍 동기화                                                     
                                                                                                                                   
  ### 🤖 AI 힌트 시스템 (STT → LLM → TTS)                                                                                          
  - 마이크 녹음 → Naver Clova STT → OpenAI GPT → Naver Clova TTS 비동기 파이프라인                                                 
  - 챕터별 진행 흐름 JSON + 플레이어 런타임 상태를 LLM에 동적 주입                                                                 
  - ISpeechToText, ILanguageModel, ITextToSpeech 인터페이스 기반 구현체 교체 가능 구조                                             
                                                                                                                                   
  ### 👻 공포 구간 추격 시스템                                                                                                     
  - FOV 확장 + HeadBob + 카메라 감쇄 회전 + 심장박동/숨소리 루프                                                                   
  - Viewport + Linecast 이중 판정 시야 기반 적 소멸                                                                                
  - ChaseEffectController에서 시각·청각·조작감 일괄 제어                                                                           
                                                                                                                                   
  ### 🎯 플레이어 시스템                                                                                                           
  - 모듈형 Ability 시스템 (Movement, Look, Footstep, Scan)                                                                         
  - 모드 전환 (Scan / Item / UI / Puzzle / Cutscene)                                                                               
  - Cinemachine 기반 카메라 피드백 (FOV Punch, Shake)                                                                              
                                                                                                                                   
  ### 💾 세이브 시스템                                                                                                             
  - ISaveRepository 인터페이스 기반 클린 아키텍처                                                                                  
  - Local JSON 저장                                                                                   
                                                                                                                                   
  ## 🕹️조작법                                                                                                                     
                                                                                                                                   
  | 키 | 동작 |   
  |:---:|:---|
  | `WASD` | 이동 |                                                                                                                
  | `Mouse` | 시점 회전 |
  | `좌클릭` | 소나 스캔 발사 |                                                                                                    
  | `우클릭` | 레이저 스캔 발사 |                                                                                                  
  | `E` | 오브젝트 상호작용 |
  | `Tab` | 수집 로그 / 단서 확인 |                                                                                                
  | `R` | AI 힌트 호출 |
                                                                                                                                   
  ## 🛠️기술 스택 
                                                                                                                                   
  | 항목 | 내용 | 
  |:---:|:---|
  | **Engine** | Unity 6 (6000.3.11f1) |
  | **Language** | C# |                                                                                                            
  | **Rendering** | URP (Universal Render Pipeline) |
  | **주요 패키지** | Cinemachine, Input System, UniTask, DOTween, AI Navigation |                                                 
  | **외부 API** | OpenAI GPT, Naver Clova STT/TTS |                                                                               
  | **버전 관리** | Git + GitHub |                                                                                                                                                                                
                                                                                                                                   
  > **Note:** 본 레포지토리는 소스 코드만 포함하고 있습니다.

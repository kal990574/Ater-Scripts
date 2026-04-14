using System.Collections.Generic;                      
using UnityEngine;                                     
using Cysharp.Threading.Tasks;                         
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;                         
using _02.Scripts._03.Outgame.SaveSystem.Manager;
using System;
using System.Linq;

namespace _02.Scripts.UI.Component
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _chapterCards;
        [SerializeField] private GameObject _newGameCard;
        [SerializeField] private GameObject _continueCard;
        private IGameManager _gameManager;
        private SaveManager _saveManager;
        private int _selectedChapter;

        private async void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _gameManager = Managers.Get<IGameManager>();
            _saveManager = Managers.Get<SaveManager>();
            await RefreshChapterUI();
        }

        public void SelectChapter(int chapter)
        {
            _selectedChapter = chapter;
        }

        public async void SelectContinue()
        {
            try
            {
                var data = await _saveManager.LoadGame();
                _selectedChapter = data.ClearedChapters.Count > 0
                    ? data.ClearedChapters.Max() + 1
                    : 1;
            }
            catch (Exception e)
            {
                Debug.LogError($"세이브 로드 실패 : {e.Message}");
                _selectedChapter = 1;
            }
        }             

        public void ConfirmLoadChapter()
        {
            StatisticsManager.Instance?.BeginRun(_selectedChapter);
            _gameManager.LoadChapter(_selectedChapter);
        }

        private async UniTask RefreshChapterUI()
        {
            try
            {
                _newGameCard.SetActive(true);

                var hasSave = await _saveManager.HasSave();
                if (!hasSave)
                {
                    ShowNewGameOnly();
                    return;
                }

                var data = await _saveManager.LoadGame();

                bool allCleared = data.ClearedChapters.Count >= _chapterCards.Count;
                _continueCard.SetActive(!allCleared);

                for (int i = 0; i < _chapterCards.Count; i++)
                {
                    bool unlocked = data.ClearedChapters.Contains(i + 1);
                    _chapterCards[i].SetActive(unlocked);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"세이브 로드 실패: {e.Message}");
                ShowNewGameOnly();
            }
        }

        private void ShowNewGameOnly()
        {
            _newGameCard.SetActive(true);
            _continueCard.SetActive(false);
            for (int i = 0; i < _chapterCards.Count; i++)
                _chapterCards[i].SetActive(false);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

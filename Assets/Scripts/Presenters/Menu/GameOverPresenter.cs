using System;
using Core.Messenger;
using Messages;
using Models.Interfaces;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Presenters.Menu
{
    /// <summary>
    /// Monobehaviour presenter that shows Game Over window with score data 
    /// </summary>
    public sealed class GameOverPresenter : MonoBehaviour, IMessageListener<MoveFailedMessage>
    {
        private const string GameSceneName = "Game";
        private const string MainSceneName = "Main";
        [SerializeField] private TMP_Text _currentScore;
        [FormerlySerializedAs("_maxScore")] [SerializeField] private TMP_Text _bestScore;
        [Space(5)]
        [SerializeField] private Button _returnToMainButton;
        [SerializeField] private Button _restartButton;
        private IGameScoreModel _gameScoreModel;

        [Inject]
        private void Inject(IGameScoreModel gameScoreModel)
        {
            _gameScoreModel = gameScoreModel;
            _returnToMainButton.OnClickAsObservable().Subscribe(ReturnToMainButtonClicked).AddTo(this);
            _restartButton.OnClickAsObservable().Subscribe(RestartButtonClicked).AddTo(this);
        }

        private void ReturnToMainButtonClicked(Unit _) => SceneManager.LoadScene(MainSceneName);
        
        private void RestartButtonClicked(Unit _) => SceneManager.LoadScene(GameSceneName);

        private void Awake()
        {
            Messenger.Subscribe(this);
            gameObject.SetActive(false);
        }

        private void OnDestroy() => Messenger.Unsubscribe(this);

        public void OnMessage(MoveFailedMessage message)
        {
            gameObject.SetActive(true);
            _currentScore.text = _gameScoreModel.CurrentScore.ToString();
            _bestScore.text = _gameScoreModel.BestScore.ToString();
        }
    }
}
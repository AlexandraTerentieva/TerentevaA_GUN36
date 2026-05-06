using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _holdTime = 1f;

    private float _currentFill = 0f;
    private bool _isRestarting = false;

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            if (!_isRestarting)
            {
                _isRestarting = true;
                _currentFill = 0f;
            }

            _currentFill += Time.deltaTime / _holdTime;
            if (_fillImage != null) _fillImage.fillAmount = _currentFill;

            if (_currentFill >= 1f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            if (_isRestarting)
            {
                _isRestarting = false;
                _currentFill = 0f;
                if (_fillImage != null) _fillImage.fillAmount = 0f;
            }
        }
    }
}
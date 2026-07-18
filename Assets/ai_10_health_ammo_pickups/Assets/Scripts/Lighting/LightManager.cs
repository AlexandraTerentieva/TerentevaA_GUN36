using UnityEngine;

public class LightManager : MonoBehaviour
{
    public GameObject dayLight;
    public GameObject nightLight;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            dayLight.SetActive(true);
            nightLight.SetActive(false);
            Debug.Log("День включён");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            dayLight.SetActive(false);
            nightLight.SetActive(true);
            Debug.Log("Ночь включена");
        }
    }
}
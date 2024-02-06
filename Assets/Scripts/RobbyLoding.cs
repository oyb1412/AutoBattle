using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class RobbyLoding : MonoBehaviour
{
    [SerializeField] Slider lodingSlider;
    [SerializeField] Text lodingText;
    [SerializeField] GameObject startPanel;
    float maxLodingTime;
    float currentLodingTime;

    private void Start()
    {
        maxLodingTime = 100f;
    }
    private void Update()
    {
        currentLodingTime += (Time.deltaTime * 20f);
        InLoding();
    }

    void InLoding()
    {
        lodingSlider.value = currentLodingTime / maxLodingTime;
        lodingText.text = string.Format("Loding... {0:F0}%", currentLodingTime);

        if(lodingSlider.value >= 1f)
        {
            lodingSlider.gameObject.SetActive(false);
            startPanel.SetActive(true);
        }
    }

}

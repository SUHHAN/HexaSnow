using UnityEngine;
using UnityEngine.UI;

public class SaveButtonSelfConnect : MonoBehaviour
{
    void OnEnable()
    {
        Button btn = GetComponent<Button>();
        if (btn != null && DataManager.Instance != null)
        {
            btn.onClick.RemoveAllListeners(); // 기존 연결 제거
            btn.onClick.AddListener(DataManager.Instance.SaveSlotData);
            Debug.Log("✔ [SaveButtonSelfConnect] 저장 버튼에 연결 완료");
        }
        else
        {
            Debug.LogWarning("⚠ [SaveButtonSelfConnect] 버튼 또는 DataManager 인스턴스를 찾을 수 없음");
        }
    }
}


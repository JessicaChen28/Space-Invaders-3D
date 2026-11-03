using UnityEngine;
using TMPro;

public class AlienCounter : MonoBehaviour
{
    TextMeshProUGUI uiText;

    // Awake (not Start) so uiText is ready before AlienFleet.Start calls SetCount
    void Awake()
    {
        uiText = GetComponent<TextMeshProUGUI>();
    }

    // Only runs when the count actually changes, instead of rebuilding the string every frame
    public void SetCount(int count)
    {
        uiText.text = $"Remaining Aliens: {count:#,0}";
    }
}
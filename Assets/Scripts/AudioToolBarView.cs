using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioToolBarView : MonoBehaviour
{
    [SerializeField] private Button btn_playSound;
    [SerializeField] private Button btn_record;
    [SerializeField] private Button btn_deleteHistory;
    [SerializeField] private Button btn_boxChat;

    public Action onClickPlaySoundButton; 
    public Action onClickRecordButton;
    public Action onClickDeleteHistoryButton;
    public Action onClickHideBoxChatButton;

    private void OnEnable()
    {
        btn_record.onClick.AddListener(() => onClickRecordButton.Invoke());
    }

    private void OnDisable()
    {
        btn_record.onClick.RemoveAllListeners();
    }
}

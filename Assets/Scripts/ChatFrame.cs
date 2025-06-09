using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatFrame : MonoBehaviour
{
    [SerializeField] private TMP_Text m_role; 
    [SerializeField] private TMP_Text m_text;

    public void Setup(string role, string text)
    {
        m_role.text = role;
        m_text.text = text;
    }
}

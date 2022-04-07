using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameState : NetworkBehaviour
{
    public TMP_Text BlueText;
    public TMP_Text RedText;
    public TMP_Text RedWon;
    public TMP_Text BlueWon;
    public Image MicroImage;

    [SyncVar(hook=nameof(OnBlueScoreChanged))] [HideInInspector] public int BlueScore;
    [SyncVar(hook=nameof(OnRedScoreChanged))] [HideInInspector] public int RedScore;
    
    public static GameState Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }

#if !UNITY_EDITOR && UNITY_WEBGL
    void Start()
    {
        GameManager.Instance.LiveKitNetwork.Room.LocalParticipant.IsSpeakingChanged += LocalSpeakingChanged;
    }
    
    void OnDestroy()
    {
        GameManager.Instance.LiveKitNetwork.Room.LocalParticipant.IsSpeakingChanged -= LocalSpeakingChanged;
    }
#endif
    
    void LocalSpeakingChanged(bool speaking)
    {
        MicroImage.enabled = speaking;
    }

    [ClientRpc]
    public void RpcShowRoundWin(Team team, float time)
    {
        StartCoroutine(ShowRoundWin(team, time));
    }

    IEnumerator ShowRoundWin(Team team, float time)
    {
        var text = team == Team.Red ? RedWon : BlueWon;
        text.gameObject.SetActive(true);

        yield return new WaitForSeconds(time);
        text.gameObject.SetActive(false);
    }
    
    void OnBlueScoreChanged(int old, int neww)
    {
        BlueText.text = neww.ToString();
    }

    void OnRedScoreChanged(int old, int neww)
    {
        RedText.text = neww.ToString();
    }
}
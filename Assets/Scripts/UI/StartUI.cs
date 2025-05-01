using UnityEngine;
using UnityEngine.UI;

public class StartUI : BaseUI
{
    [SerializeField] private InputField seedInputField;
    [SerializeField] private Button startButton;
    [SerializeField] private Text seedText;
    
    [Header("시드 설정")]
    [SerializeField] private int seedLength = 10;
    [SerializeField] private string validCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public override UIState State => UIState.Start;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        
        if (seedInputField != null)
        {
            seedInputField.placeholder.GetComponent<Text>().text = $"시드값을 입력하세요 ({seedLength}자리)";
        }
    }
    private bool IsValidSeed(string seed)
    {
        if (string.IsNullOrEmpty(seed)) return true;
        if (seed.Length != seedLength) return false;
        
        foreach (char c in seed)
        {
            if (!validCharacters.Contains(c.ToString()))
            {
                return false;
            }
        }
        
        return true;
    }

    private void OnStartButtonClicked()
    {
        string inputSeed = seedInputField.text;
        
        if (!IsValidSeed(inputSeed))
        {
            seedInputField.text = "";
            seedInputField.placeholder.GetComponent<Text>().text = "잘못된 시드값입니다. 다시 입력해주세요.";
            return;
        }
        
        if (!string.IsNullOrEmpty(inputSeed))
        {
            GameManager.Instance.seed = inputSeed;
        }
        
        GameManager.Instance.StartGame();
        seedText.text = GameManager.Instance.seed;
    }

    private void ResetInput()
    {
        if (seedInputField != null)
        {
            seedInputField.text = "";
            seedInputField.placeholder.GetComponent<Text>().text = $"시드값을 입력하세요 ({seedLength}자리)";
        }
        if(seedText != null)
        {
            seedText.text = "";
        }
    }

    protected override void Initialize()
    {
        ResetInput();
    }

    protected override void Cleanup()
    {

    }
} 
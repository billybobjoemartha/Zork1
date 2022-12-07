using UnityEngine;
using Zork.Common;
using Newtonsoft.Json;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI LocationText;

    [SerializeField]
    private TextMeshProUGUI ScoreText;

    [SerializeField]
    private TextMeshProUGUI MovesText;

    [SerializeField]
    private TextMeshProUGUI LivesText;

    [SerializeField]
    private UnityInputService InputService;

    [SerializeField]
    private UnityOutputService OutputService;

    private void Awake()
    {
        TextAsset gameJson = Resources.Load<TextAsset>("GameJson");
        _game = JsonConvert.DeserializeObject<Game>(gameJson.text);
        _game.Player.LocationChanged += Player_LocationChanged;
        _game.Run(InputService, OutputService);
    }

    private void Player_LocationChanged(object sender, Room location)
    {
        LocationText.text = location.Name;
    }

    private void Player_ScoreChanged()
    {
        ScoreText.text = $"Score: {_game.rewardScore}";
    }

    private void Player_MovesChanged()
    {
        MovesText.text = $"Moves: {_game.moves}";
    }

    private void Player_LivesChanged()
    {
        LivesText.text = $"Lives: {_game.playerHealth}/25";
    }


    private void Start()
    {
        InputService.SetFocus();
        LocationText.text = _game.Player.CurrentRoom.Name;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            InputService.ProcessInput();
            InputService.SetFocus();
            Player_ScoreChanged();
            Player_MovesChanged();
            Player_LivesChanged();
        }
    }

    private Game _game;
}

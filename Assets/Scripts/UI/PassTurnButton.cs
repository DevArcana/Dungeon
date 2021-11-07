using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class PassTurnButton : MonoBehaviour
  {
    public Button passTurnButton;

    private void Start()
    {
      var turnManager = TurnManager.instance;
      passTurnButton.onClick.AddListener(() =>
      {
        turnManager.NextTurn();
      });
      turnManager.TurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      passTurnButton.interactable = e.Entity is PlayerEntity;
    }
  }
}
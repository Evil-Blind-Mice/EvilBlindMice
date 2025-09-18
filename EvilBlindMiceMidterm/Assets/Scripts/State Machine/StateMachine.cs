using UnityEngine;
using TMPro;

public class StateMachine : MonoBehaviour
{
    [SerializeField] CustomState defaultState;
    CustomState currentState;

    [Header("Debugging")]
    [SerializeField] bool debug;
    [SerializeField] TMP_Text currentStateDisplay;

    private void Start()
    {
        ChangeToState(defaultState);
        if (debug) currentStateDisplay.gameObject.SetActive(true);
    }

    public void ChangeToState(CustomState _nextState)
    {
        if(currentState != null) currentState.OnExit();
        currentState = _nextState;
        currentState.OnEnter();
    }

    public void Update()
    {
        if (debug) ShowDebug();
        currentState.OnUpdate();
    }

    public void ShowDebug()
    {
        currentStateDisplay.text = currentState.gameObject.name;
    }
}

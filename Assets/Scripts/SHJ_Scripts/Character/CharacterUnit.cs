using UnityEngine;

public class CharacterUnit : CharacterClass
{
    [SerializeField] private UnitType classUnitType;

    private CharacterManager manager;

    protected override void Awake()
    {
        base.Awake();
        manager = GetComponent<CharacterManager>();
    }

    private void Start()
    {
        RequestMyData();
        ChangeState(UnitState.Idle);
    }

    public void OnSelected()
    {
        ChangeState(UnitState.Selected);
    }

    public void OnCommand()
    {
        ChangeState(UnitState.Command);
    }

    public void OnMove()
    {
        ChangeState(UnitState.Move);
    }

    public void OnAction()
    {
        ChangeState(UnitState.Action);
    }

    public void OnEnd()
    {
        ChangeState(UnitState.End);
    }

    private void RequestMyData()
    {
        if (manager == null)
        {
            Debug.LogWarning("CharacterManager ¾øÀ½!");
            return;
        }

        var result = manager.RequestUnitData(classUnitType);
        if (result != null)
        {
            Debug.Log($"[{classUnitType}] Atk:{result.attack}, Def:{result.defense}, Move:{result.moveCost}");
        }
    }
}

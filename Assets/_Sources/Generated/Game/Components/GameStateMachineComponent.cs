//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public StateMachineComponent stateMachine { get { return (StateMachineComponent)GetComponent(GameComponentsLookup.StateMachine); } }
    public bool hasStateMachine { get { return HasComponent(GameComponentsLookup.StateMachine); } }

    public void AddStateMachine(RSG.IState newFsm) {
        var index = GameComponentsLookup.StateMachine;
        var component = CreateComponent<StateMachineComponent>(index);
        component.fsm = newFsm;
        AddComponent(index, component);
    }

    public void ReplaceStateMachine(RSG.IState newFsm) {
        var index = GameComponentsLookup.StateMachine;
        var component = CreateComponent<StateMachineComponent>(index);
        component.fsm = newFsm;
        ReplaceComponent(index, component);
    }

    public void RemoveStateMachine() {
        RemoveComponent(GameComponentsLookup.StateMachine);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherStateMachine;

    public static Entitas.IMatcher<GameEntity> StateMachine {
        get {
            if (_matcherStateMachine == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.StateMachine);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherStateMachine = matcher;
            }

            return _matcherStateMachine;
        }
    }
}

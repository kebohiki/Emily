using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class ProcessInputActionSystem : ReactiveSystem<InputEntity>
{
    private readonly IGroup<GameEntity> _controllableEntities;

    public ProcessInputActionSystem(Contexts contexts)
        : base(contexts.input)
    {
        _controllableEntities =
            contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Controllable, GameMatcher.StateMachine));
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.InputAction);
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasInputAction;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        for (int i = 0, length = entities.Count; i < length; ++i)
        {
            var e = entities[i];
            var inputAction = e.inputAction;

            ProcessAction(inputAction.state);
        }
    }

    private void ProcessAction(CharacterState newState)
    {
        foreach (var e in _controllableEntities.GetEntities())
        {
            e.stateMachine.fsm.TriggerEvent("ResetToIdle");
            e.stateMachine.fsm.TriggerEvent(Consts.GetStateString(newState));
        }
    }
}

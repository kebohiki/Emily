//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiInterfaceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public interface IPathFindingToPositionEntity {

    PathFindingToPositionComponent pathFindingToPosition { get; }
    bool hasPathFindingToPosition { get; }

    void AddPathFindingToPosition(float newX, float newY);
    void ReplacePathFindingToPosition(float newX, float newY);
    void RemovePathFindingToPosition();
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameContext {

    public GameEntity mapLoadEntity { get { return GetGroup(GameMatcher.MapLoad).GetSingleEntity(); } }

    public bool isMapLoad {
        get { return mapLoadEntity != null; }
        set {
            var entity = mapLoadEntity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().isMapLoad = true;
                } else {
                    entity.Destroy();
                }
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly MapLoadComponent mapLoadComponent = new MapLoadComponent();

    public bool isMapLoad {
        get { return HasComponent(GameComponentsLookup.MapLoad); }
        set {
            if (value != isMapLoad) {
                var index = GameComponentsLookup.MapLoad;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : mapLoadComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherMapLoad;

    public static Entitas.IMatcher<GameEntity> MapLoad {
        get {
            if (_matcherMapLoad == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.MapLoad);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherMapLoad = matcher;
            }

            return _matcherMapLoad;
        }
    }
}
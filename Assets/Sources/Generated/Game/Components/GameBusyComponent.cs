//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameContext {

    public GameEntity busyEntity { get { return GetGroup(GameMatcher.Busy).GetSingleEntity(); } }

    public bool isBusy {
        get { return busyEntity != null; }
        set {
            var entity = busyEntity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().isBusy = true;
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

    static readonly Game.BusyComponent busyComponent = new Game.BusyComponent();

    public bool isBusy {
        get { return HasComponent(GameComponentsLookup.Busy); }
        set {
            if (value != isBusy) {
                if (value) {
                    AddComponent(GameComponentsLookup.Busy, busyComponent);
                } else {
                    RemoveComponent(GameComponentsLookup.Busy);
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

    static Entitas.IMatcher<GameEntity> _matcherBusy;

    public static Entitas.IMatcher<GameEntity> Busy {
        get {
            if (_matcherBusy == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Busy);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherBusy = matcher;
            }

            return _matcherBusy;
        }
    }
}

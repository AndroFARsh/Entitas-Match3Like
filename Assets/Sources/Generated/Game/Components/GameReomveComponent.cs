//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly Game.ReomveComponent reomveComponent = new Game.ReomveComponent();

    public bool isReomve {
        get { return HasComponent(GameComponentsLookup.Reomve); }
        set {
            if (value != isReomve) {
                if (value) {
                    AddComponent(GameComponentsLookup.Reomve, reomveComponent);
                } else {
                    RemoveComponent(GameComponentsLookup.Reomve);
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

    static Entitas.IMatcher<GameEntity> _matcherReomve;

    public static Entitas.IMatcher<GameEntity> Reomve {
        get {
            if (_matcherReomve == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Reomve);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherReomve = matcher;
            }

            return _matcherReomve;
        }
    }
}

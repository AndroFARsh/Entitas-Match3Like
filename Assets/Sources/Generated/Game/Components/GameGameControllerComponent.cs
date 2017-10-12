//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameContext {

    public GameEntity gameControllerEntity { get { return GetGroup(GameMatcher.GameController).GetSingleEntity(); } }

    public bool isGameController {
        get { return gameControllerEntity != null; }
        set {
            var entity = gameControllerEntity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().isGameController = true;
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

    static readonly Game.GameControllerComponent gameControllerComponent = new Game.GameControllerComponent();

    public bool isGameController {
        get { return HasComponent(GameComponentsLookup.GameController); }
        set {
            if (value != isGameController) {
                if (value) {
                    AddComponent(GameComponentsLookup.GameController, gameControllerComponent);
                } else {
                    RemoveComponent(GameComponentsLookup.GameController);
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

    static Entitas.IMatcher<GameEntity> _matcherGameController;

    public static Entitas.IMatcher<GameEntity> GameController {
        get {
            if (_matcherGameController == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GameController);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherGameController = matcher;
            }

            return _matcherGameController;
        }
    }
}
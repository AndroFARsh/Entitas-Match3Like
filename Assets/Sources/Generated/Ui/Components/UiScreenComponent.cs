//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class UiContext {

    public UiEntity screenEntity { get { return GetGroup(UiMatcher.Screen).GetSingleEntity(); } }
    public Ui.ScreenComponent screen { get { return screenEntity.screen; } }
    public bool hasScreen { get { return screenEntity != null; } }

    public UiEntity SetScreen(Ui.Screen newValue) {
        if (hasScreen) {
            throw new Entitas.EntitasException("Could not set Screen!\n" + this + " already has an entity with Ui.ScreenComponent!",
                "You should check if the context already has a screenEntity before setting it or use context.ReplaceScreen().");
        }
        var entity = CreateEntity();
        entity.AddScreen(newValue);
        return entity;
    }

    public void ReplaceScreen(Ui.Screen newValue) {
        var entity = screenEntity;
        if (entity == null) {
            entity = SetScreen(newValue);
        } else {
            entity.ReplaceScreen(newValue);
        }
    }

    public void RemoveScreen() {
        screenEntity.Destroy();
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
public partial class UiEntity {

    public Ui.ScreenComponent screen { get { return (Ui.ScreenComponent)GetComponent(UiComponentsLookup.Screen); } }
    public bool hasScreen { get { return HasComponent(UiComponentsLookup.Screen); } }

    public void AddScreen(Ui.Screen newValue) {
        var index = UiComponentsLookup.Screen;
        var component = CreateComponent<Ui.ScreenComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceScreen(Ui.Screen newValue) {
        var index = UiComponentsLookup.Screen;
        var component = CreateComponent<Ui.ScreenComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveScreen() {
        RemoveComponent(UiComponentsLookup.Screen);
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
public sealed partial class UiMatcher {

    static Entitas.IMatcher<UiEntity> _matcherScreen;

    public static Entitas.IMatcher<UiEntity> Screen {
        get {
            if (_matcherScreen == null) {
                var matcher = (Entitas.Matcher<UiEntity>)Entitas.Matcher<UiEntity>.AllOf(UiComponentsLookup.Screen);
                matcher.componentNames = UiComponentsLookup.componentNames;
                _matcherScreen = matcher;
            }

            return _matcherScreen;
        }
    }
}
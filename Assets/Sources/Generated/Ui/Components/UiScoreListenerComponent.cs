//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class UiEntity {

    public Ui.ScoreListenerComponent scoreListener { get { return (Ui.ScoreListenerComponent)GetComponent(UiComponentsLookup.ScoreListener); } }
    public bool hasScoreListener { get { return HasComponent(UiComponentsLookup.ScoreListener); } }

    public void AddScoreListener(Ui.OnScoreChangedDelegate newValue) {
        var index = UiComponentsLookup.ScoreListener;
        var component = CreateComponent<Ui.ScoreListenerComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceScoreListener(Ui.OnScoreChangedDelegate newValue) {
        var index = UiComponentsLookup.ScoreListener;
        var component = CreateComponent<Ui.ScoreListenerComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveScoreListener() {
        RemoveComponent(UiComponentsLookup.ScoreListener);
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

    static Entitas.IMatcher<UiEntity> _matcherScoreListener;

    public static Entitas.IMatcher<UiEntity> ScoreListener {
        get {
            if (_matcherScoreListener == null) {
                var matcher = (Entitas.Matcher<UiEntity>)Entitas.Matcher<UiEntity>.AllOf(UiComponentsLookup.ScoreListener);
                matcher.componentNames = UiComponentsLookup.componentNames;
                _matcherScoreListener = matcher;
            }

            return _matcherScoreListener;
        }
    }
}
using Entitas;
using Game;
using Input;
using Ui;
using UnityEngine;

namespace Match3Like
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private GameConfigBlueprint _config;
        
        private Systems _systems;
        
        private void Start()
        {
            var contexts = Contexts.sharedInstance;
            contexts.game.SetConfig(_config);
            var entity = contexts.game.CreateEntity();
            entity.isGameController = true;
            entity.AddView(gameObject);
            
            _systems = new Feature("Game Systems")
                // Game 
                .Add(new GameSystems(contexts))
                // Input
                .Add(new InputSystems(contexts))
                // Ui
                .Add(new UiSystems(contexts));

            _systems.Initialize();
        }

        private void Update()
        {
            _systems.Execute();
            
            _systems.Cleanup();
        }

        private void OnDestroy()
        {
            _systems.TearDown();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Platform_Creator_CS.Sources.Entities.Actions;
using Platform_Creator_CS.Sources.Entities.Containers;
using Platform_Creator_CS.Utility;
using IUpdateable = Platform_Creator_CS.Utility.IUpdateable;

namespace Platform_Creator_CS.Sources.Entities {
    [JsonObject(IsReference = true)]
    public class Entity : IUpdateable, IRenderable {
        [JsonProperty("states")] private readonly List<EntityState> _states = new List<EntityState>();

        private EntityContainer _container;

        private int _currentState;
        private int _layer;

        public Entity(string tag, string name, Rect box, EntityState defaultState, EntityContainer container = null,
            params EntityState[] otherStates) {
            Tag = tag;
            Name = name;
            Box = box;

            if (defaultState != null)
                _states.Add(defaultState);

            if (otherStates != null)
                _states.AddRange(otherStates);

            Container = container;
        }

        [UI.UI]
        public string Tag { get; set; }

        public string Name { get; set; }

        public Rect Box { get; set; }

        public int Layer {
            get => _layer;
            set {
                if (value >= Constants.MinLayer && value <= Constants.MaxLayer)
                    _layer = value;
            }
        }

        public int InitialState { get; set; } = 0;

        public EntityContainer Container {
            get => _container;
            set {
                _container = value;
                if (value != null) SetState(InitialState, true);
            }
        }

        public Action OnOutOfMapAction { get; set; } = new RemoveEntityAction();

        public List<GridCell> GridCells { get; } = new List<GridCell>();

        public void Render(SpriteBatch batch, float alpha) {
            GetCurrentState().Render(batch, alpha);
        }

        public void Update(GameTime gameTime) {
            GetCurrentState().Update(gameTime);
        }

        public IEnumerable<EntityState> GetStates() {
            return _states;
        }

        public EntityState GetCurrentState() {
            return _states[_currentState];
        }

        public EntityState GetStateOrDefault(int index) {
            return _states.ElementAtOrDefault(index) ?? _states[0];
        }

        public int AddState(EntityState state) {
            _states.Add(state);
            return _states.Count - 1;
        }

        public void RemoveState(int state) {
            if (_states.Any())
                _states.RemoveAt(state);
        }

        public void SetState(int index, bool triggerStartAction) {
            if (index >= 0 && index < _states.Count) {
                GetCurrentState().ToggleDisable();

                _currentState = index;

                if (Container != null)
                    GetCurrentState().ToggleActive(this, Container, triggerStartAction);
            }
        }

        public void RemoveFromParent() {
            Container?.RemoveEntity(this);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            SetState(InitialState, true);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Entities.Actions;
using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Platform_Creator_CS.Entities {
    public class Entity : Utility.IUpdateable, IRenderable {
        private readonly List<EntityState> _states = new List<EntityState>();
        private int _currentState;
        private int _layer;
        private EntityContainer _container;

        public string Tag { get; set; }
        public string Name { get; set; }
        public Rect Box { get; set; }
        public int Layer { 
            get => _layer; 
            set {
                if(value >= Constants.MinLayer && value <= Constants.MaxLayer)
                    _layer = value;
            }
        }
        
        public int InitialState { get; set; } = 0;

        public EntityContainer Container {
            get => _container; 
            set {
                _container = value;
                if(value != null) {
                    SetState(InitialState, true);
                }
            } 
        }
        
        public Action OnOutOfMapAction { get; set; } = new RemoveEntityAction();

        public List<GridCell> GridCells { get; } = new List<GridCell>();

        public Entity(string tag, string name, Rect box, EntityState defaultState, EntityContainer container = null, params EntityState[] otherStates) {
            Tag = tag;
            Name = name;
            Box = box;
            
            _states.Add(defaultState);
            _states.AddRange(otherStates);

            Container = container;
        }

        public IEnumerable<EntityState> GetStates() => _states;

        public EntityState GetCurrentState() => _states[_currentState];

        public EntityState GetStateOrDefault(int index) => _states.ElementAtOrDefault(index) ?? _states[0];

        public Utility.Point GetPosition() => Box.Position;
        public Utility.Size GetSize() => Box.Size;

        public int AddState(EntityState state) {
            _states.Add(state);
            return _states.Count - 1;
        }

        public void RemoveState(int state) {
            if(_states.Any())
                _states.RemoveAt(state);
        }

        public void SetState(int index, bool triggerStartAction) {
            if(index >= 0 && index < _states.Count) {
                GetCurrentState().ToggleDisable();

                _currentState = index;

                if(Container != null)
                    GetCurrentState().ToggleActive(this, Container, triggerStartAction);
            }
        }

        public void RemoveFromParent() {
            Container?.RemoveEntity(this);
        }

        public void Render(SpriteBatch batch, float alpha) {
            GetCurrentState().Render(batch, alpha);
        }

        public void Update(GameTime gameTime) {
            GetCurrentState().Update(gameTime);
        }
    }
}
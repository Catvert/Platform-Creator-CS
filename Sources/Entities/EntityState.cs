using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Entities.Actions;
using Platform_Creator_CS.Entities.Components;
using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Utility;
using System.Linq;

namespace Platform_Creator_CS.Entities {
    public class EntityState : Utility.IUpdateable, IRenderable
    {
        private readonly List<Component> _components = new List<Component>();

        private Entity _entity;

        private bool _active;

        public string Name { get; set; }

        public Actions.Action StartAction { get; set; } = new EmptyAction();
        
        public IEnumerable<Component> GetComponents() => _components;

        public EntityState(string name, params Component[] components) {
            Name = name;

            foreach(var comp in components)
                AddComponent(comp);
        }
        
        public void ToggleActive(Entity entity, EntityContainer container, bool triggerStartAction) {
            foreach(var component in _components) {
                component.OnStateActive(entity, this, container);
            }

            if(triggerStartAction)
                StartAction.Invoke(entity);

            _entity = entity;

            _active = true;
        }

        public void ToggleDisable() {
            _active = false;
        }

        public void AddComponent(Component component) {
            if(_components.All(c => c.GetType() != component.GetType())) {
                _components.Add(component);

                if(_active)
                    component.OnStateActive(_entity, this, _entity.Container);
            }
        }

        public void RemoveComponent(Component component) {
            _components.Remove(component);
        }

        public T GetComponent<T>() where T : Component => (T)_components.FirstOrDefault(c => c.GetType() == typeof(T));

        public bool HasComponent<T>() where T : Component => _components.Any(c => c.GetType() == typeof(T));

        public void Render(SpriteBatch batch, float alpha) {
            foreach(var renderable in _components.OfType<IRenderable>()) {
                renderable.Render(batch, alpha);
            }
        }

        public void Update(GameTime gameTime) {
            foreach(var updeatable in _components.OfType<Utility.IUpdateable>()) {
                updeatable.Update(gameTime);
            }
        }

        public override string ToString() => Name;
    }
}
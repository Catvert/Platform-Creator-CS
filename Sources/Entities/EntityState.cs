using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Platform_Creator_CS.Entities.Actions;
using Platform_Creator_CS.Entities.Components;
using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Utility;
using IUpdateable = Platform_Creator_CS.Utility.IUpdateable;

namespace Platform_Creator_CS.Entities {
    public class EntityState : IUpdateable, IRenderable {
        [JsonProperty("components")] private readonly List<Component> _components = new List<Component>();

        private bool _active;

        private Entity _entity;

        public EntityState(string name, params Component[] components) {
            Name = name;

            foreach (var comp in components)
                AddComponent(comp);
        }

        public string Name { get; set; }

        public Action StartAction { get; set; } = new EmptyAction();

        public void Render(SpriteBatch batch, float alpha) {
            foreach (var renderable in _components.OfType<IRenderable>()) renderable.Render(batch, alpha);
        }

        public void Update(GameTime gameTime) {
            foreach (var updeatable in _components.OfType<IUpdateable>()) updeatable.Update(gameTime);
        }

        public IEnumerable<Component> GetComponents() {
            return _components;
        }

        public void ToggleActive(Entity entity, EntityContainer container, bool triggerStartAction) {
            foreach (var component in _components) component.OnStateActive(entity, this, container);

            if (triggerStartAction)
                StartAction.Invoke(entity);

            _entity = entity;

            _active = true;
        }

        public void ToggleDisable() {
            _active = false;
        }

        public void AddComponent(Component component) {
            if (_components.All(c => c.GetType() != component.GetType())) {
                _components.Add(component);

                if (_active)
                    component.OnStateActive(_entity, this, _entity.Container);
            }
        }

        public void RemoveComponent(Component component) {
            _components.Remove(component);
        }

        public T GetComponent<T>() where T : Component {
            return (T) _components.FirstOrDefault(c => c.GetType() == typeof(T));
        }

        public bool HasComponent<T>() where T : Component {
            return _components.Any(c => c.GetType() == typeof(T));
        }

        public override string ToString() {
            return Name;
        }
    }
}
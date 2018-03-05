using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Platform_Creator_CS.Utility;
using IUpdateable = Platform_Creator_CS.Utility.IUpdateable;

namespace Platform_Creator_CS.Entities.Containers {
    [JsonObject(IsReference = true)]
    public class EntityContainer : IUpdateable, IRenderable {
        private readonly List<Entity> _removeEntities = new List<Entity>();

        [JsonProperty("entities")]
        protected List<Entity> Entities { get; } = new List<Entity>();

        protected Action<Entity> OnRemoveEntity { get; set; } = null;

        [JsonIgnore]
        public bool AllowRenderingEntities { get; set; } = true;

        [JsonIgnore]
        public bool AllowUpdatingEntities { get; set; } = true;

        public virtual void Render(SpriteBatch batch, float alpha) {
            if (AllowRenderingEntities)
                foreach (var entity in GetProcessEntities().OrderBy(e => e.Layer))
                    entity.Render(batch, alpha);
        }

        public virtual void Update(GameTime gameTime) {
            if (AllowUpdatingEntities) {
                foreach (var entity in GetProcessEntities()) entity.Update(gameTime);

                _removeEntities.RemoveAll(e => {
                    OnRemoveEntity?.Invoke(e);
                    Entities.Remove(e);
                    e.Container = null;

                    return true;
                });
            }
        }

        protected virtual IEnumerable<Entity> GetProcessEntities() {
            return Entities;
        }

        public IEnumerable<Entity> GetEntities() {
            return Entities;
        }

        public virtual IEnumerable<Entity> FindEntitiesByTag(string tag) {
            return Entities.FindAll(e => e.Tag == tag);
        }

        public virtual Entity AddEntity(Entity entity) {
            Entities.Add(entity);

            entity.Container = this;

            return entity;
        }

        public void RemoveEntity(Entity entity) {
            _removeEntities.Add(entity);
        }
    }
}
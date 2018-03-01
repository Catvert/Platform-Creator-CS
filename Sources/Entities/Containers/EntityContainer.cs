using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Utility;
using System.Linq;
using Newtonsoft.Json;

namespace Platform_Creator_CS.Entities.Containers {
    [JsonObject(IsReference = true)]
    public class EntityContainer : Utility.IUpdateable, IRenderable {
        [JsonProperty("entities")]
        protected List<Entity> Entities { get; } = new List<Entity>();

        protected System.Action<Entity> OnRemoveEntity { get; set; } = null;

        [JsonIgnore]
        public bool AllowRenderingEntities { get; set; } = true;
        [JsonIgnore]
        public bool AllowUpdatingEntities { get; set; } = true;

        private readonly List<Entity> _removeEntities = new List<Entity>();

        protected virtual IEnumerable<Entity> GetProcessEntities() => Entities;

        public IEnumerable<Entity> GetEntities() => Entities;

        public virtual IEnumerable<Entity> FindEntitiesByTag(string tag) => Entities.FindAll(e => e.Tag == tag);

        public virtual Entity AddEntity(Entity entity) {
            Entities.Add(entity);

            entity.Container = this;

            return entity;
        }

        public void RemoveEntity(Entity entity) {
            _removeEntities.Add(entity);
        }
        
        public virtual void Render(SpriteBatch batch, float alpha) {
            if(AllowRenderingEntities)
                foreach(var entity in GetProcessEntities().OrderBy(e => e.Layer))
                    entity.Render(batch, alpha);
        }

        public virtual void Update(GameTime gameTime) {
            if(AllowUpdatingEntities) {
                foreach(var entity in GetProcessEntities()) {
                    entity.Update(gameTime);

                    if(entity.GetPosition().X < 0) {
                        entity.OnOutOfMapAction.Invoke(entity);
                    }
                }
                
                _removeEntities.RemoveAll(e => {
                    OnRemoveEntity?.Invoke(e);
                    Entities.Remove(e);
                    e.Container = null;

                    return true;
                });
            }
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform_Creator_CS.Utility;
using System.Linq;

namespace Platform_Creator_CS.Entities.Containers {
    public class EntityContainer : Utility.IUpdateable, IRenderable {
        protected List<Entity> Entities { get; } = new List<Entity>();
        protected System.Action<Entity> OnRemoveEntity { get; set; } = null;
        public bool AllowRenderingEntities { get; set; } = true;
        public bool AllowUpdatingEntities { get; set; } = true;

        private List<Entity> RemoveEntities = new List<Entity>();

        protected virtual IEnumerable<Entity> GetProcessEntities() => Entities;

        public IEnumerable<Entity> GetEntities() => Entities;

        public IEnumerable<Entity> FindEntitiesByTag(string tag) => Entities.FindAll(e => e.Tag == tag);

        public Entity AddEntity(Entity entity) {
            Entities.Add(entity);

            entity.Container = this;

            return entity;
        }

        public void RemoveEntity(Entity entity) {
            RemoveEntities.Add(entity);
        }
        
        public EntityContainer() {
           
        }
        
        public void Render(SpriteBatch batch, float alpha) {
            if(AllowRenderingEntities)
                foreach(var entity in GetProcessEntities().OrderBy(e => e.Layer))
                    entity.Render(batch, alpha);
        }

        public void Update(GameTime gameTime) {
            if(AllowUpdatingEntities) {
                foreach(var entity in GetProcessEntities()) {
                    entity.Update(gameTime);

                    if(entity.GetPosition().X < 0) {
                        entity.OnOutOfMapAction.Invoke(entity);
                    }
                }
                
                RemoveEntities.RemoveAll(e => {
                    OnRemoveEntity?.Invoke(e);
                    Entities.Remove(e);
                    e.Container = null;

                    return true;
                });
            }
        }
    }
}
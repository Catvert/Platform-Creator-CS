using Newtonsoft.Json;
using Platform_Creator_CS.Sources.Entities.Containers;

namespace Platform_Creator_CS.Sources.Entities.Components {
    public abstract class Component {
        protected Entity Entity { get; private set; }
        
        [JsonIgnore]
        public bool Active { get; set; }
        
        public virtual void OnStateActive(Entity entity, EntityState state, EntityContainer container) {
            Entity = entity;
        }
    }
}
using Platform_Creator_CS.Entities.Containers;

namespace Platform_Creator_CS.Entities.Components {
    public abstract class Component {
        protected Entity Entity { get; private set; }
        
        public bool Active { get; set; }
        
        public virtual void OnStateActive(Entity entity, EntityState state, EntityContainer container) {
            Entity = entity;
        }
    }
}
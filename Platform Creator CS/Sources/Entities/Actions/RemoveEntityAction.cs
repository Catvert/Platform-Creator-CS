namespace Platform_Creator_CS.Sources.Entities.Actions {
    public class RemoveEntityAction : Action {
        public override void Invoke(Entity entity) {
            entity.RemoveFromParent();
        }
    }
}
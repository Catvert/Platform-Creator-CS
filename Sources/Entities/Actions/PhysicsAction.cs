using Platform_Creator_CS.Entities;
using Platform_Creator_CS.Entities.Actions;
using Platform_Creator_CS.Entities.Components;

namespace Platform_Creator_CS.Sources.Entities.Actions {
    public class PhysicsAction : Action {
        public PhysicsAction(PhysicsComponent.PhysicsActions action) {
            Action = action;
        }

        private PhysicsAction(): this(PhysicsComponent.PhysicsActions.MoveLeft) { }

        public PhysicsComponent.PhysicsActions Action { get; set; }

        public override void Invoke(Entity entity) {
            entity.GetCurrentState().GetComponent<PhysicsComponent>()?.AddPhysicsAction(Action);
        }
    }
}
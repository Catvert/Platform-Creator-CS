using System;
using System.Collections.Generic;
using Platform_Creator_CS.Sources.Entities.Actions;

namespace Platform_Creator_CS.Entities.Actions {
    public abstract class Action {
        public static Type[] Actions = {
            typeof(EmptyAction),
            typeof(MoveAction),
            typeof(PhysicsAction),
            typeof(RemoveEntityAction)
        };

        public abstract void Invoke(Entity entity);
    }
}
namespace Platform_Creator_CS.Entities.Actions {
    public class MoveAction : Action
    {
        public int MoveX { get; set; }
        public int MoveY { get; set; }

        public MoveAction(int moveX, int moveY) {
            MoveX = moveX;
            MoveY = moveY;
        }

        public override void Invoke(Entity entity) {
            entity.Box.Position.X += MoveX;
            entity.Box.Position.Y += MoveY;
        }
    }
}
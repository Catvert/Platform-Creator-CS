namespace Platform_Creator_CS.Entities.Actions {
    public class MoveAction : Action {
        public MoveAction(int moveX, int moveY) {
            MoveX = moveX;
            MoveY = moveY;
        }

        private MoveAction() : this(0, 0) { }

        public int MoveX { get; set; }
        public int MoveY { get; set; }

        public override void Invoke(Entity entity) {
            entity.Box.Move(MoveX, MoveY);
        }
    }
}
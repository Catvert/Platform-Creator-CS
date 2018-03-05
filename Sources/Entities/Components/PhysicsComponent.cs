using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Platform_Creator_CS.Entities;
using Platform_Creator_CS.Entities.Actions;
using Platform_Creator_CS.Entities.Components;
using Platform_Creator_CS.Entities.Containers;
using Platform_Creator_CS.Utilities;
using Platform_Creator_CS.Utility;
using Action = Platform_Creator_CS.Entities.Actions.Action;
using IUpdateable = Platform_Creator_CS.Utility.IUpdateable;
using Point = Platform_Creator_CS.Utility.Point;

namespace Platform_Creator_CS.Entities.Components {
    public class PhysicsComponent : Component, IUpdateable {
        public delegate void CollisionDelegate(CollisionData data);

        public enum MovementType {
            Smooth,
            Linear
        }

        public enum PhysicsActions {
            MoveLeft,
            MoveRight,
            MoveUp,
            MoveDown,
            Jump,
            ForceJump
        }

        private readonly HashSet<PhysicsActions> _nextPhysicsActions = new HashSet<PhysicsActions>();

        private float _actualMoveSpeedX;
        private float _actualMoveSpeedY;

        private JumpData _jumpData;

        private Level _level;

        public PhysicsComponent(bool isStatic, int moveSpeed = 0, int jumpHeight = 0,
            MovementType movementType = MovementType.Smooth, bool gravity = false, bool isPlatform = false) {
            IsStatic = isStatic;
            MoveSpeed = moveSpeed;
            Movement = movementType;
            Gravity = gravity;
            IsPlatform = isPlatform;
            JumpHeight = jumpHeight;
        }

        public bool IsStatic { get; set; }
        public int MoveSpeed { get; set; }
        public int JumpHeight { get; set; }
        public MovementType Movement { get; set; }
        public bool Gravity { get; set; }
        public bool IsPlatform { get; set; }

        public List<string> IgnoreTags { get; } = new List<string>();
        public List<CollisionAction> CollisionActions { get; } = new List<CollisionAction>();

        public Action OnLeftAction { get; set; } = new EmptyAction();
        public Action OnRightAction { get; set; } = new EmptyAction();
        public Action OnUpAction { get; set; } = new EmptyAction();
        public Action OnDownAction { get; set; } = new EmptyAction();
        public Action OnJumpAction { get; set; } = new EmptyAction();
        public Action OnNothingAction { get; set; } = new EmptyAction();

        public void Update(GameTime gameTime) {
            if (IsStatic)
                return;

            var moveSpeedX = 0f;
            var moveSpeedY = 0f;
            var addJumpAfterClear = false;

            if (Gravity && _level.ApplyGravity)
                moveSpeedY += _level.GravitySpeed;

            foreach (var action in _nextPhysicsActions)
                switch (action) {
                    case PhysicsActions.MoveLeft:
                        moveSpeedX -= MoveSpeed;
                        break;
                    case PhysicsActions.MoveRight:
                        moveSpeedX += MoveSpeed;
                        break;
                    case PhysicsActions.MoveUp:
                        moveSpeedY -= MoveSpeed;
                        break;
                    case PhysicsActions.MoveDown:
                        moveSpeedY += MoveSpeed;
                        break;
                    case PhysicsActions.Jump:
                    case PhysicsActions.ForceJump: {
                        if (_level.ApplyGravity)
                            if (!_jumpData.IsJumping) {
                                if (Gravity) {
                                    if (action != PhysicsActions.ForceJump)
                                        if (!Move(false, 0f, 1f))
                                            continue;

                                    _jumpData.IsJumping = true;
                                    _jumpData.TargetHeight = Entity.Box.Position.Y.Round() - JumpHeight;

                                    Gravity = false;

                                    moveSpeedY = _level.GravitySpeed;
                                    addJumpAfterClear = true;

                                    OnJumpAction.Invoke(Entity);
                                }
                            }
                            else {
                                // Vérifie si l'entité est arrivé à la bonne hauteur de saut ou s'il rencontre un obstacle au-dessus de lui
                                if (Entity.Box.Position.Y <= _jumpData.TargetHeight || Move(false, 0f, -1f)) {
                                    Gravity = true;
                                    _jumpData.IsJumping = false;
                                }
                                else {
                                    moveSpeedY = -_level.GravitySpeed;
                                    addJumpAfterClear = true;
                                }
                            }
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            _nextPhysicsActions.Clear();

            // TODO moveSpeedX *= Utility.GetDeltaTime() * Constants.PhysicsDeltaSpeed

            if (Movement == MovementType.Smooth) {
                moveSpeedX =
                    MathHelper.Lerp(
                        (_actualMoveSpeedX / Constants.PhysicsEpsilon).Round() * Constants.PhysicsEpsilon,
                        moveSpeedX, 0.2f);
                moveSpeedY =
                    MathHelper.Lerp(
                        (_actualMoveSpeedY / Constants.PhysicsEpsilon).Round() * Constants.PhysicsEpsilon,
                        moveSpeedY, 0.2f);
            }

            _actualMoveSpeedX = moveSpeedX;
            _actualMoveSpeedY = moveSpeedY;

            var lastPos = Entity.Box.Position;

            Move(true, moveSpeedX, moveSpeedY);

            if (Entity.Box.Position.EqualsEpsilon(lastPos, Constants.PhysicsEpsilon))
                OnNothingAction.Invoke(Entity);

            if (addJumpAfterClear)
                _nextPhysicsActions.Add(PhysicsActions.Jump);
        }

        public event CollisionDelegate CollisionEvent;

        public override void OnStateActive(Entity entity, EntityState state, EntityContainer container) {
            base.OnStateActive(entity, state, container);

            if (container is Level level)
                _level = level;
            else
                throw new InvalidCastException("Impossible d'utiliser un PhysicsComponent en dehors d'un niveau");
        }

        public void AddPhysicsAction(PhysicsActions action) {
            _nextPhysicsActions.Add(action);
        }

        private void TriggerCollisionEvent(Entity collideEntity, BoxSide side, int collideIndex) {
            var thisColAction = CollisionActions.FirstOrDefault(c =>
                (c.Side == side || c.Side == BoxSide.All) && c.TargetTag == collideEntity.Tag);

            thisColAction?.Action.Invoke(thisColAction.ApplyActionOnCollider ? collideEntity : Entity);

            CollisionEvent?.Invoke(new CollisionData(Entity, collideEntity, side, collideIndex));

            var collideComp = collideEntity.GetCurrentState().GetComponent<PhysicsComponent>();
            var collideColAction = collideComp?.CollisionActions.FirstOrDefault(c =>
                (c.Side == side.Inversed() || c.Side == BoxSide.All) && c.TargetTag == Entity.Tag);

            collideColAction?.Action.Invoke(collideColAction.ApplyActionOnCollider ? Entity : collideEntity);

            collideComp?.CollisionEvent?.Invoke(new CollisionData(collideEntity, Entity, side.Inversed(),
                collideIndex));
        }

        public bool Move(bool move, float targetMoveX, float targetMoveY) {
            var moveX = 0f;
            var moveY = 0f;

            var returnCollide = false;

            var potentialCollideEntities = _level.GetAllEntitiesInCells(
                Entity.Box.Merge(new Rect(
                    new Point(Entity.Box.Position.X + targetMoveX, Entity.Box.Position.Y + targetMoveY),
                    Entity.Box.Size))).Where(filter => {
                if (!ReferenceEquals(Entity, filter) && filter.GetCurrentState().HasComponent<PhysicsComponent>()) {
                    var filterComp = filter.GetCurrentState().GetComponent<PhysicsComponent>();
                    return !filterComp.IgnoreTags.Contains(Entity.Tag) &&
                           (!filterComp.IsPlatform || Entity.Box.Bottom() <= filter.Box.Top());
                }

                return false;
            }).ToArray();

            var checkRect = new Rect(Entity.Box);
            var triggerCallCounter = 0;

            // Do-while à la place d'un while pour vérifier si l'entité n'est pas déjà en overlaps avec une autre (mal placé)
            do {
                checkRect.Y = Entity.Box.Y + moveY + Math.Sign(targetMoveY) * Constants.PhysicsEpsilon;

                var collide = false;
                foreach (var it in potentialCollideEntities)
                    if (checkRect.Overlaps(it.Box)) {
                        BoxSide side;

                        if (moveY < 0) {
                            side = BoxSide.Up;
                        }
                        else if (moveY > 0) {
                            side = BoxSide.Down;
                        }
                        else {
                            // Permet de téléporter cette entité au-dessus de l'autre
                            if (move)
                                Entity.Box.Y = Math.Max(it.Box.Top() - Entity.Box.Height - Constants.PhysicsEpsilon,
                                    _level.MatrixRect.Top());

                            continue;
                        }

                        collide = true;

                        TriggerCollisionEvent(it, side, triggerCallCounter++);
                    }

                if (!collide && !moveY.EqualsEpsilon(targetMoveY, Constants.PhysicsEpsilon)) {
                    moveY += Math.Sign(targetMoveY) * Constants.PhysicsEpsilon;
                }
                else {
                    if (!collide) {
                        if (moveY > 0 && !Entity.Box.Top()
                                .EqualsEpsilon(_level.MatrixRect.Top(), Constants.PhysicsEpsilon))
                            OnUpAction.Invoke(Entity);
                        else if (moveY < 0)
                            OnDownAction.Invoke(Entity);
                    }
                    else {
                        returnCollide = true;
                    }

                    if (move)
                        Entity.Box.Y =
                            Math.Max(Entity.Box.Y + moveY - Math.Sign(targetMoveY) * Constants.PhysicsEpsilon,
                                _level.MatrixRect.Top());

                    checkRect.Y = Entity.Box.Y;
                    break;
                }
            } while (Math.Abs(moveY) < Math.Abs(targetMoveY));

            while (Math.Abs(moveX) < Math.Abs(targetMoveX)) {
                checkRect.Position =
                    new Point(Entity.Box.X + moveX + Math.Sign(targetMoveX) * Constants.PhysicsEpsilon,
                        checkRect.Position.Y);
                var collide = false;

                foreach (var it in potentialCollideEntities)
                    if (checkRect.Overlaps(it.Box)) {
                        BoxSide side;
                        if (moveX > 0)
                            side = BoxSide.Right;
                        else if (moveX < 0)
                            side = BoxSide.Left;
                        else
                            continue;

                        collide = true;

                        TriggerCollisionEvent(it, side, triggerCallCounter++);
                    }

                if (!collide && !moveX.EqualsEpsilon(targetMoveX, Constants.PhysicsEpsilon)) {
                    moveX += Math.Sign(targetMoveX) * Constants.PhysicsEpsilon;
                }
                else {
                    if (!collide) {
                        if (moveX > 0 && !Entity.Box.Right()
                                .EqualsEpsilon(_level.MatrixRect.Right(), Constants.PhysicsEpsilon))
                            OnRightAction.Invoke(Entity);
                        else if (moveX < 0 && !Entity.Box.Left()
                                     .EqualsEpsilon(_level.MatrixRect.Left(), Constants.PhysicsEpsilon))
                            OnLeftAction.Invoke(Entity);
                    }
                    else {
                        returnCollide = true;
                    }

                    if (move)
                        Entity.Box.X = Math.Clamp(
                            Entity.Box.X + moveX - Math.Sign(targetMoveX) * Constants.PhysicsEpsilon,
                            _level.MatrixRect.Left(), _level.MatrixRect.Right() - Entity.Box.Width);

                    checkRect.X = Entity.Box.X;
                    break;
                }
            }

            return returnCollide;
        }

        public struct JumpData {
            public bool IsJumping { get; set; }
            public int TargetHeight { get; set; }
        }

        public class CollisionData {
            public CollisionData(Entity entity, Entity collideEntity, BoxSide side, int triggerCallCount) {
                Entity = entity;
                CollideEntity = collideEntity;
                Side = side;
                TriggerCallCount = triggerCallCount;
            }

            public Entity Entity { get; }
            public Entity CollideEntity { get; }
            public BoxSide Side { get; }
            public int TriggerCallCount { get; }
        }

        public class CollisionAction {
            public BoxSide Side { get; set; } = BoxSide.Left;
            public string TargetTag { get; set; } = Tags.Player.GetTag();
            public Action Action { get; set; } = new EmptyAction();
            public bool ApplyActionOnCollider { get; set; } = false;
        }
    }
}
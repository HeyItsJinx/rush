// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Rush.Objects;
using osuTK.Input;

namespace osu.Game.Rulesets.Rush.Edit.Blueprints
{
    public abstract class LanedPlacementBlueprint<T> : RushPlacementBlueprint<T>
        where T : RushHitObject
    {
        protected CompositeDrawable Piece { get; }

        protected abstract CompositeDrawable CreatePiece();

        protected LanedPlacementBlueprint(T hitObject)
            : base(hitObject)
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = Piece = CreatePiece().With(d => d.Origin = Anchor.Centre);
        }

        public override void UpdatePosition(SnapResult result)
        {
            base.UpdatePosition(result);

            Piece.Position = ToLocalSpace(result.ScreenSpacePosition);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button != MouseButton.Left)
                return false;

            base.OnMouseDown(e);

            EndPlacement(true);

            return true;
        }
    }
}

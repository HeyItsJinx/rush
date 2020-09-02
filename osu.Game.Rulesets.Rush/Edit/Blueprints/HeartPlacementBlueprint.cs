// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Rush.Edit.Blueprints.Components;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Edit.Blueprints
{
    public class HeartPlacementBlueprint : LanedPlacementBlueprint<Heart>
    {
        protected override CompositeDrawable CreatePiece() => new EditHeartPiece();

        public HeartPlacementBlueprint()
            : base(new Heart())
        {
        }
    }
}

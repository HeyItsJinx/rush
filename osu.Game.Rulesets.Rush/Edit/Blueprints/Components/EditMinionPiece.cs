// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Rush.UI;
using osuTK;

namespace osu.Game.Rulesets.Rush.Edit.Blueprints.Components
{
    public class EditMinionPiece : CompositeDrawable
    {
        public EditMinionPiece()
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE);
            InternalChild = new MinionPiece();
        }
    }
}

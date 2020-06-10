// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Edit.Blueprints;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Rush.Edit
{
    public class RushBlueprintContainer : ComposeBlueprintContainer
    {
        public RushBlueprintContainer(IEnumerable<DrawableHitObject> hitObjects)
            : base(hitObjects)
        {
        }

        protected override SelectionHandler CreateSelectionHandler() => new RushSelectionHandler();

        public override OverlaySelectionBlueprint CreateBlueprintFor(DrawableHitObject hitObject) =>
            new RushSelectionBlueprint(hitObject);
    }
}

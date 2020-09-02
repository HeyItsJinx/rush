// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Rush.Edit
{
    public class DrawableRushEditRuleset : DrawableRushRuleset
    {
        public new IScrollingInfo ScrollingInfo => base.ScrollingInfo;

        public DrawableRushEditRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override Playfield CreatePlayfield() => new RushEditPlayfield();
    }
}

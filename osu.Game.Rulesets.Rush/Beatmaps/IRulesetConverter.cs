// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    public interface IRulesetConverter
    {
        IEnumerable<RushHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap);
    }
}

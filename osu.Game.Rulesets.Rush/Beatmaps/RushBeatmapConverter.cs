// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Beatmaps.Converters;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    public class RushBeatmapConverter : BeatmapConverter<RushHitObject>
    {
        private IRulesetConverter rulesetConverter;

        public RushBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        protected override Beatmap<RushHitObject> CreateBeatmap() => new RushBeatmap();

        protected override Beatmap<RushHitObject> ConvertBeatmap(IBeatmap original)
        {
            rulesetConverter = new OsuRulesetConverter(original);
            var beatmap = base.ConvertBeatmap(original);
            rulesetConverter = null;

            return beatmap;
        }

        public override bool CanConvert() => true;

        protected override IEnumerable<RushHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap) =>
            rulesetConverter.ConvertHitObject(original, beatmap);
    }
}

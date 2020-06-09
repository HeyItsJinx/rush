// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Beatmaps.Converters;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    public class RushBeatmapConverter : BeatmapConverter<RushHitObject>
    {
        private readonly IRulesetConverter rulesetConverter;

        public RushBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
            rulesetConverter = converterForBeatmap(beatmap, ruleset);
        }

        private static IRulesetConverter converterForBeatmap(IBeatmap beatmap, Ruleset ruleset)
        {
            if (beatmap.BeatmapInfo.Ruleset.Equals(ruleset.RulesetInfo))
                return null;

            return beatmap.BeatmapInfo.Ruleset.ID switch
            {
                TaikoRulesetConverter.RULESET_ID => new TaikoRulesetConverter(beatmap),
                _ => new OsuRulesetConverter(beatmap)
            };
        }

        protected override Beatmap<RushHitObject> CreateBeatmap() => new RushBeatmap();

        public override bool CanConvert() => true;

        protected override IEnumerable<RushHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap) =>
            rulesetConverter?.ConvertHitObject(original, beatmap) ?? Enumerable.Empty<RushHitObject>();
    }
}

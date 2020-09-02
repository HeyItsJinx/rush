// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Rulesets.Rush.UI;

namespace osu.Game.Rulesets.Rush.Edit
{
    public class RushEditPlayfield : RushPlayfield
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            LeftAreaContainer.Hide();
        }
    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class PauseStats : UIButton {
        
        public PauseStats(Vector2 pos, Vector2 dimen) : base(null, pos, dimen) {
            hoverGrow = false;
            selectable = false;
        }

        public override void render(SpriteBatch spriteBatch) {
            base.render(spriteBatch);

            LevelStats levelStats = Runner.stats.getLevelStates(Runner.levelName);
            
            Vector2 tl = pos - dimen / 2;
            spriteBatch.DrawString(font, "High Score: " + levelStats.bestTimeStr(), tl + new Vector2(20, 20), textColor);
            spriteBatch.DrawString(font, "Times Completed: " + levelStats.timesCompleted, tl + new Vector2(20, 70), textColor);
            spriteBatch.DrawString(font, "Times Failed: " + levelStats.timesFailed, tl + new Vector2(20, 120), textColor);
        }
    }
}
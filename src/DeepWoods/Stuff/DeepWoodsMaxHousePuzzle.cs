﻿
using DeepWoodsMod.API.Impl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace DeepWoodsMod.Stuff
{
    public class DeepWoodsMaxHousePuzzle : LargeTerrainFeature
    {
        private static readonly int NUM_COLUMNS = 4;
        private static readonly float COLUMN_SPEED = 3;
        private static readonly int COLUMN_LENGTH = 3;

        private NetArray<bool, NetBool> currentState = new NetArray<bool, NetBool>();
        private NetArray<float, NetFloat> columnPositions = new NetArray<float, NetFloat>();

        public DeepWoodsMaxHousePuzzle()
           : base(false)
        {
            for (int i = 0; i < NUM_COLUMNS; i++)
            {
                this.currentState.Add(true);
                this.columnPositions.Add(1f);
            }
        }

        public DeepWoodsMaxHousePuzzle(Vector2 tileLocation)
            : this()
        {
            this.Tile = tileLocation;
        }

        public override void initNetFields()
        {
            base.initNetFields();
            this.NetFields.AddField(this.currentState).AddField(this.columnPositions);
        }

        public override bool isActionable()
        {
            return false;
        }

        public override Rectangle getBoundingBox()
        {
            return new Rectangle((int)Tile.X * 64, (int)Tile.Y * 64, 128, 128);
        }

        public override bool isPassable(Character c = null)
        {
            return !this.currentState.Contains(true);
        }

        public override bool performUseAction(Vector2 tileLocation)
        {
            if (this.currentState.Contains(true))
            {
                Location.playSound(Sounds.THUD_STEP, this.Tile);
                Game1.showRedMessage(I18N.ExcaliburNopeMessage);
            }

            return true;
        }

        public override bool tickUpdate(GameTime time)
        {
            if (!Game1.IsMasterGame)
                return false;

            bool isDirty = false;

            for (int i = 0; i < NUM_COLUMNS; i++)
            {
                // if column state is disabled, but column isn't fully down yet, move it down
                if (!this.currentState[i] && this.columnPositions[i] > 0f)
                {
                    this.columnPositions[i] -= (float)(COLUMN_SPEED * time.ElapsedGameTime.TotalSeconds);
                    if (this.columnPositions[i] < 0f)
                    {
                        this.columnPositions[i] = 0f;
                    }
                    isDirty = true;
                }
                // if column state is enabled, but column isn't fully up yet, move it up
                else if (this.currentState[i] && this.columnPositions[i] < 1f)
                {
                    this.columnPositions[i] += (float)(COLUMN_SPEED * time.ElapsedGameTime.TotalSeconds);
                    if (this.columnPositions[i] > 1f)
                    {
                        this.columnPositions[i] = 1f;
                    }
                    isDirty = true;
                }
            }

            if (isDirty)
            {
                this.columnPositions.MarkDirty();
            }

            return !isDirty && !this.currentState.Contains(true);
        }

        public void doPuzzle(string tileId)
        {
            if (!this.currentState.Contains(true))
                return;

            this.Location.playSound("button1");

            // TODO: Disabled until more story is implemented
            // this.currentLocation.playSound(Sounds.THUD_STEP);
            Game1.showRedMessage(I18N.MaxHousePuzzleNopeMessage);

            /*
            // TODO: if not powered yet
            //if (...)
            //{
            //    return;
            //}

            if (int.TryParse(tileId, out int result) && result >= 1 && result <= 5)
            {
                doPuzzle(result);
            }
            */
        }

        /*
        private void doPuzzle(int index)
        {
            this.currentLocation.playSound("furnace");
            this.currentLocation.playSound("trainLoop");
            this.currentLocation.playSound("thunder_small");
            //this.currentLocation.playSoundPitched("thunder_small", 50);

            switch (index)
            {
                case 1:
                    this.currentState[0] = !this.currentState[0];
                    this.currentState[2] = !this.currentState[2];
                    break;
                case 2:
                    this.currentState[0] = !this.currentState[0];
                    this.currentState[1] = !this.currentState[1];
                    this.currentState[3] = !this.currentState[3];
                    break;
                case 3:
                    this.currentState[3] = !this.currentState[3];
                    break;
                case 4:
                    this.currentState[1] = !this.currentState[1];
                    this.currentState[3] = !this.currentState[3];
                    break;
                case 5:
                    this.currentState[1] = !this.currentState[1];
                    this.currentState[2] = !this.currentState[2];
                    break;
            }

            if (!this.currentState.Contains(true))
            {
                this.currentLocation.playSound("secret1");
            }

            this.currentState.MarkDirty();
        }
        */


        public override void dayUpdate()
        {
        }

        public override bool seasonUpdate(bool onLoad)
        {
            return false;
        }

        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation)
        {
            return false;
        }

        public override void performPlayerEntryAction()
        {
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Vector2 globalPosition = Tile * 64f;

            Rectangle sourceRectangle = new Rectangle(0, 0, 16, 16);

            for (int i = 0; i < NUM_COLUMNS; i++)
            {
                for (int j = 0; j < COLUMN_LENGTH; j++)
                {
                    Vector2 position = globalPosition + new Vector2(i * 32 - 16, (COLUMN_LENGTH - 1) * 64 - (columnPositions[i] * COLUMN_LENGTH - j) * 64 - 16);

                    spriteBatch.Draw(DeepWoodsTextures.Textures.DeepWoodsMaxHousePuzzleColumn, Game1.GlobalToLocal(Game1.viewport, position), sourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((Tile.Y + 1f) * 64f / 10000f + Tile.X / 100000f));
                }
            }
        }
    }
}

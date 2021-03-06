﻿using System;
using System.Collections.Generic;
using Engine.Gui.Base;
using IniParser;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Texture = Engine.Gui.Base.Texture;

namespace Engine.Gui
{
    public sealed class DialogGui : GuiItem
    {
        private TextGui _text;
        private TextGui _selectA;
        private TextGui _selectB;
        private GuiItem _portrait;
        private Dictionary<int, Texture> _portraitList;
        private readonly Color _defaultTextColor = Color.Black * 0.8f;
        public bool IsInSelecting { get; private set; }
        public int Selection { get; private set; }

        private void LoadPortrait()
        {
            const string path = @"ini\ui\dialog\HeadFile.ini";
            _portraitList = new Dictionary<int, Texture>();
            try
            {
                var parser = new FileIniDataParser();
                var data = parser.ReadFile(path, Globals.LocalEncoding);
                foreach (var item in data["PORTRAIT"])
                {
                    _portraitList[int.Parse(item.KeyName)] = new Texture(Utils.GetAsf(@"asf\portrait\", item.Value));
                }
            }
            catch (Exception exception)
            {
                Log.LogFileLoadError("Portrait", path, exception);
            }
        }

        public DialogGui()
        {
            LoadPortrait();

            var cfg = GuiManager.Setttings.Sections["Dialog"];

            BaseTexture = new Texture(Utils.GetAsf(null, cfg["Image"]));
            Width = BaseTexture.Width;
            Height = BaseTexture.Height;
            Position = new Vector2((Globals.WindowWidth - BaseTexture.Width) / 2f + int.Parse(cfg["LeftAdjust"]),
                Globals.WindowHeight + int.Parse(cfg["TopAdjust"]));

            cfg = GuiManager.Setttings.Sections["Dialog_Txt"];
            _text = new TextGui(this,
                new Vector2(int.Parse(cfg["Left"]), int.Parse(cfg["Top"])),
                int.Parse(cfg["Width"]),
                int.Parse(cfg["Height"]),
                Globals.FontSize12,
                int.Parse(cfg["CharSpace"]),
                int.Parse(cfg["LineSpace"]),
                "",
                Utils.GetColor(cfg["Color"]));

            cfg = GuiManager.Setttings.Sections["Dialog_SelA"];
            _selectA = new TextGui(this,
                new Vector2(int.Parse(cfg["Left"]), int.Parse(cfg["Top"])),
                int.Parse(cfg["Width"]),
                int.Parse(cfg["Height"]),
                Globals.FontSize12,
                int.Parse(cfg["CharSpace"]),
                int.Parse(cfg["LineSpace"]),
                "",
                Utils.GetColor(cfg["Color"]));

            cfg = GuiManager.Setttings.Sections["Dialog_SelB"];
            _selectB = new TextGui(this,
                new Vector2(int.Parse(cfg["Left"]), int.Parse(cfg["Top"])),
                int.Parse(cfg["Width"]),
                int.Parse(cfg["Height"]),
                Globals.FontSize12,
                int.Parse(cfg["CharSpace"]),
                int.Parse(cfg["LineSpace"]),
                "",
                Utils.GetColor(cfg["Color"]));

            cfg = GuiManager.Setttings.Sections["Dialog_Portrait"];
            _portrait = new GuiItem(this,
                new Vector2(int.Parse(cfg["Left"]), int.Parse(cfg["Top"])),
                int.Parse(cfg["Width"]),
                int.Parse(cfg["Height"]),
                null);
            _selectA.MouseEnter += (arg1, arg2) => _selectA.SetDrawColor(Color.Red * 0.8f);
            _selectA.MouseLeave += (arg1, arg2) => _selectA.SetDrawColor(Color.Blue * 0.8f);
            _selectA.MouseLeftDown += (arg1, arg2) =>
            {
                if (IsInSelecting)
                {
                    IsInSelecting = false;
                    Selection = 0;
                }
            };
            _selectB.MouseEnter += (arg1, arg2) => _selectB.SetDrawColor(Color.Red * 0.8f);
            _selectB.MouseLeave += (arg1, arg2) => _selectB.SetDrawColor(Color.Blue * 0.8f);
            _selectB.MouseLeftDown += (arg1, arg2) =>
            {
                if (IsInSelecting)
                {
                    IsInSelecting = false;
                    Selection = 1;
                }
            };

            IsShow = false;
        }

        public void ShowText(string text, int portraitIndex = -1)
        {
            IsShow = true;
            _text.Text = text;
            if (portraitIndex != -1 && _portraitList.ContainsKey(portraitIndex))
            {
                _portrait.BaseTexture = _portraitList[portraitIndex];
            }
            else _portrait.BaseTexture = null;
        }

        public bool NextPage()
        {
            return _text.NextPage();
        }

        public void Select(string message, string selectA, string selectB)
        {
            IsInSelecting = true;
            IsShow = true;
            _text.Text = message;
            _selectA.Text = selectA;
            _selectB.Text = selectB;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsShow) return;
            base.Update(gameTime);
            _text.Update(gameTime);
            _portrait.Update(gameTime);

            //Updata selection
            _selectA.Update(gameTime);
            _selectB.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsShow) return;
            base.Draw(spriteBatch);
            _portrait.Draw(spriteBatch);
            _text.Draw(spriteBatch);
            if (IsInSelecting)
            {
                _selectA.Draw(spriteBatch);
                _selectB.Draw(spriteBatch);
            }
        }
    }
}
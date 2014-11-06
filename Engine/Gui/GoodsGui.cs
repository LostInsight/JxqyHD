﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Gui
{
    public class GoodsGui
    {
        private ListView _listView;

        public static Action<object, DragDropItem.DropEvent> DropHandler = (arg1, arg2) =>
        {
            var item = (DragDropItem)arg1;
            var sourceItem = arg2.Source;
            var data = item.Data as GoodItemData;
            var sourceData = sourceItem.Data as GoodItemData;
            if (data != null && sourceData != null)
            {
                GoodsListManager.ExchangeListItem(data.Index, sourceData.Index);
                item.BaseTexture = GoodsListManager.GetTexture(data.Index);
                sourceItem.BaseTexture = GoodsListManager.GetTexture(sourceData.Index);
                var info = GoodsListManager.GetItemInfo(data.Index);
                var sourceInfo = GoodsListManager.GetItemInfo(sourceData.Index);
                item.TopLeftText = info != null ? info.Count.ToString() : "";
                sourceItem.TopLeftText = sourceInfo != null ? sourceInfo.Count.ToString() : "";
            }
        };

        public bool IsShow { set; get; }

        public GoodsGui()
        {
            var baseTexture = new Texture(Utils.GetAsf(@"asf\ui\common\panel3.asf"));
            var position = new Vector2(
                Globals.WindowWidth / 2f,
                0f);
            _listView = new ListView(null,
                position,
                baseTexture.Width,
                baseTexture.Height,
                baseTexture,
                66);
            _listView.Scrolled += delegate
            {
                UpdateItems();
            };
            _listView.RegisterItemDragHandler((arg1, arg2) =>
            {
                
            });
            _listView.RegisterItemDropHandler(DropHandler);
        }

        public void UpdateItems()
        {
            for (var i = 0; i < 9; i++)
            {
                var index = _listView.ToListIndex(i);
                var info = GoodsListManager.GetItemInfo(index);
                Good good = null;
                if (info != null) good = info.TheGood;
                var image = (good == null ? null : good.Image);
                _listView.SetListItem(i, new Texture(image), new GoodItemData(index));
                _listView.SetItemTopLeftText(i, info != null ? info.Count.ToString() : "");
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!IsShow) return;

            _listView.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsShow) return;

            _listView.Draw(spriteBatch);
        }

        public class GoodItemData
        {
            public int Index { private set; get; }
            public GoodItemData(int index)
            {
                Index = index;
            }
        }
    }
}
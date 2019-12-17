using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace spooki_bar
{
    class ContentManager2 : ContentManager
    {
        public ContentManager2(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public override T Load<T>(string assetName)
        {
            try
            { return ReadAsset<T>(assetName, IgnoreDisposableAsset); }
            catch (Exception e)
            {
                
               // return default(T);
                throw e;
            }
        }

        void IgnoreDisposableAsset(IDisposable disposable)
        {
        }
    }
}

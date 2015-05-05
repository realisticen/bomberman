using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.BaseClass;
using Bomberman.GameStuff;
using Bomberman.Utilities;

namespace Bomberman.Screens
{
    class Player2Screen : Screen
    {

        private Map map;
        public Player2Screen(ScreenManager owner, Map _map)
            : base(owner)
        {
            map = _map;
        }
    }
}

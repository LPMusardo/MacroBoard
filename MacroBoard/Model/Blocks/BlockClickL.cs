﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using static MacroBoard.Utils;

namespace MacroBoard
{
    [Serializable]
    public class BlockClickL : Block
    {

        public BlockClickL()
        {
            base.Name = "Left Click";
            base.LogoUrl = "/Resources/Logo_Blocks/Logo_BlockClickL.png";
            base.info = "Simulate a Left mouse click in the current position of the mouse pointer.";
            base.category = Categories.Controls;
        }



        public override void Execute()
        {
            mouse_event((int)MouseEventFlags.LeftDown, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlags.LeftUp, 0, 0, 0, 0);
        }

        public override void Accept(IBlockVisitor visitor)
        {
            visitor.Visit(this);
        }


    }
}

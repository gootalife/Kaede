using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaedeConsole {
    public class AnimationFrame {
        public Bitmap Bitmap { get; }
        public string AnimationName { get; }
        public string Name { get; }
        public Point Origin { get; }
        public int Delay { get; }

        public AnimationFrame(Bitmap bitmap, string animationName, string name, Point origin, int delay = 0) {
            Bitmap = bitmap;
            AnimationName = animationName;
            Name = name;
            Origin = origin;
            Delay = delay;
        }
    }
}

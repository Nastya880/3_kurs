using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GameApp
{
    public partial class PlayerControl : PictureBox
    {
        public PlayerControl() : base()
        {
            InitializeComponent();
            Image image = Image.FromFile("D:\\Проектирование и архитектура программных систем\\игра\\winforms-maze-master\\winforms-maze-master\\img\\octopus.png"); // загружаем изображение игрока
            Image = new Bitmap(image, Sizes.PlayerSize, Sizes.PlayerSize); // строим изображение

            GraphicsPath playerPath;
            playerPath = new GraphicsPath();
            //playerPath.AddEllipse(DisplayRectangle);
            playerPath.AddRectangle(DisplayRectangle); // рисуем прямоугольную область, в которой будет изображение игрока
            Region = new Region(playerPath);
        }

        // обновление расположения картинки при перемещении игрока
        public void updateLocation(Point move)
        {
            Location = new Point(Left + move.X, Top + move.Y); // смещение влево и вверх на положительное или отрицательное число шагов
            Region.Translate(move.X, move.Y);
        }
    }
}

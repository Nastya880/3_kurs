using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp
{
    //Дополнительный класс для удобства конфигурации 
    //отказ от перечисления для лучшей читаемости кода
    //Используется в контроле плеера и в мейз билдере
    class Maze
    {
        Random rand;
        public GraphicsPath Path; // стены
        public Region Walls;

        private Maze()
        {
            rand = new Random(DateTime.Now.Millisecond);
        }
        public Maze(Rectangle area) : this()
        {
            Path = new GraphicsPath();
            Path.FillMode = FillMode.Winding;
            // рисуем прямоугольники с заданным расположением краёв
            Path.AddRectangles(new[]{ 
                Rectangle.FromLTRB(area.Left, area.Top, area.Right, area.Top + Sizes.WallThickness),
                Rectangle.FromLTRB(area.Right - Sizes.WallThickness, area.Top,area.Right, area.Bottom),
                Rectangle.FromLTRB(area.Left, area.Top, area.Left + Sizes.WallThickness, area.Bottom),
                Rectangle.FromLTRB(area.Left, area.Bottom - Sizes.WallThickness, area.Right, area.Bottom)
            });
            Walls = new Region(Path);
        }

        // паттерн строитель

        // рекурсия
        public void RecursiveDivideRect(Rectangle rect)
        {
            //Конец рекурсии при отсутствии дальнейшего разделения
            if (rect.Width > rect.Height)
                foreach (Rectangle divided in this.DivideRectByWidth(rect, Sizes.WallThickness))
                    RecursiveDivideRect(divided);
            else
                foreach (Rectangle divided in this.DivideRectByHeight(rect,Sizes.WallThickness))
                    RecursiveDivideRect(divided);
        }

        // вертикальная стенка
        private IEnumerable<Rectangle> DivideRectByHeight(Rectangle rect,int dividerThickness)
        {
            if (rect.Height <= 2 * Sizes.PlayerSize + Sizes.WallThickness)
                yield break;
            int dividerTop = rand.Next(rect.Top + Sizes.PlayerSize, rect.Bottom - Sizes.PlayerSize - Sizes.WallThickness); // координата y для начала разделения
            Rectangle wall = new Rectangle(rect.Left, dividerTop, rect.Width, dividerThickness); // мтроим стенку
            this.UpdatePath(this.DivideRectByWidth(wall, Sizes.Pass)); // обновляем

            //свободные для дальнейших итераций
            yield return Rectangle.FromLTRB(rect.Left, rect.Top, wall.Right, wall.Top); // от прямого левого верхнего угла до правого верхнего угла стены
            yield return Rectangle.FromLTRB(wall.Left, wall.Bottom, rect.Right, rect.Bottom); // от левого нижнего угла стены до прямого правого нижнего угла
        }

        // горизонтальная стенка
        private IEnumerable<Rectangle> DivideRectByWidth(Rectangle rect,int dividerThickness)
        {
            if (rect.Width <= 2 * Sizes.PlayerSize + dividerThickness)
                yield break;

            //Координата х для начала разделителя
            int dividerLeft = rand.Next(rect.Left + Sizes.PlayerSize, rect.Right - Sizes.PlayerSize - dividerThickness);
            //Разделитель
            Rectangle divider = new Rectangle(dividerLeft, rect.Top, dividerThickness, rect.Height);// 
            this.UpdatePath(this.DivideRectByHeight(divider, Sizes.Pass));

            //Свободные для дальнейших итераций
            yield return Rectangle.FromLTRB(rect.Left, rect.Top, divider.Left, divider.Bottom); // от левого верхнего угла до левого нижнего угла стены
            yield return Rectangle.FromLTRB(divider.Right, divider.Top, rect.Right, rect.Bottom); // от правого верхнего угла стены до правой нижней части  
        }
        private void UpdatePath(IEnumerable<Rectangle> rects)
        {
            // исключаем момент попадания стены на проход
            foreach (Rectangle rect in rects)
                if (!isClear(rect))
                    Path.AddRectangle(rect);
            Walls = new Region(Path);
        }

        // для получения расположения игрока
        public Point GetLocation(Size size,Rectangle area) 
        {
            Rectangle pos;
            do
            {
                pos = new Rectangle(rand.Next(area.Left + size.Width, area.Right - size.Width), rand.Next(area.Top + size.Height, area.Bottom - size.Height), size.Width,size.Height);
            } while (!this.isClear(pos)); // пока pos содержится в Walls
            return pos.Location;
        }

        // проверяем, содержится ли rect в Walls
        public bool isClear(Rectangle rect) => !Walls.IsVisible(Rectangle.FromLTRB(rect.Left - 1, rect.Top - 1, rect.Right + 1, rect.Bottom + 1));
    }
}

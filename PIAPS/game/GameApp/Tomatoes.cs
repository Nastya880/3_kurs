using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace GameApp
{
    public class Tomatoes : IEnumerable<PictureBox>
    {
        #region fields
        public List<PictureBox> tomatoes; // массив из помидорок
        public int tomatoesCount; // количество помидорок
        Image tomatoImage; // картинка помидор

        GraphicsPath tomatoesPath;
        Region tomatoesRegion;
        #endregion fields

        #region ctor
        public Tomatoes(int count)
        {
            tomatoesPath = new GraphicsPath();
            tomatoes = new List<PictureBox>();
            tomatoesCount = count;
            Image image = Image.FromFile("D:\\Проектирование и архитектура программных систем\\игра\\winforms-maze-master\\winforms-maze-master\\img\\tomato.png"); // загружаем изображение помидора
            tomatoImage = new Bitmap(image, Sizes.TomatoSize, Sizes.TomatoSize); // строим изображение
            InitializeCollection();
        }
        #endregion ctor

        // инициализация массива из помидорок
        private void InitializeCollection()
        {
            while (tomatoes.Count != tomatoesCount)
            {
                PictureBox pictBox = new PictureBox(); // создаём объект PictureBox
                pictBox.Size = new Size(Sizes.TomatoSize, Sizes.TomatoSize); // устанавливаем размер PictureBox
                //pictBox.SizeMode = PictureBoxSizeMode.CenterImage;
                pictBox.Image = this.tomatoImage; // помещаем в PictureBox картинку
                pictBox.LocationChanged += PBox_LocationChanged;
                tomatoes.Add(pictBox); // добавляем помидор в массив
            }
        }

        private void PBox_LocationChanged(object sender, EventArgs e)
        {
            PictureBox pBox = (PictureBox)sender;
            Rectangle area = new Rectangle(pBox.Location, pBox.Size);
            tomatoesPath.AddRectangle(area);
            tomatoesRegion = new Region(tomatoesPath);
        }

        // касание помидорки и игрока
        public bool isTaken(Rectangle rect) => tomatoesRegion.IsVisible(rect.X + Sizes.PlayerSize / 2, rect.Y + Sizes.PlayerSize / 2);

        // удаление помидорки
        public void Remove(PictureBox tomato)
        {
            tomatoesRegion.Exclude(tomato.Bounds); // обновляем область tomatoesRegion
            tomatoes.Remove(tomato); // удаляем PictureBox
            //tomatoesPath.Reset();
            /*if (tomatoes.Count != 0)
                tomatoesPath.AddRectangles(tomatoes.Select(t => t.Bounds).ToArray());*/
        }

        // функции встроенного класса IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return tomatoes.GetEnumerator();
        }

        public IEnumerator<PictureBox> GetEnumerator()
        {
            return tomatoes.GetEnumerator();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameApp
{
    public partial class Form1 : Form
    {
        Maze maze; // лабиринт
        PlayerControl player; // игрок
        Tomatoes tomatoes; // помидорки
        SoundPlayer soundPlayer; // звуковой сигнал
        SoundPlayer soundEnd; // звуковой сигнал в конце игры
        DateTime startTime;
        List<Keys> keyDown = new List<Keys>();
        Dictionary<Keys, Point> move = new Dictionary<Keys, Point>() // движение игрока в разные стороны на один шаг (скорость и направление)
        {
            { Keys.Up, new Point(0,-1) }, // вверх
            { Keys.Down, new Point(0,1) }, // вниз
            { Keys.Left, new Point(-1,0) }, // влево
            { Keys.Right, new Point(1,0) }, // вправо
        };

        public Form1()
        {
            InitializeComponent();
            startTime = DateTime.Now;
            soundPlayer = new SoundPlayer("D:\\Проектирование и архитектура программных систем\\игра\\winforms-maze-master\\winforms-maze-master\\sound\\sound.wav");// звуковой сигнал
            soundEnd = new SoundPlayer("tada.wav"); // загружаем звук в конце
            mazeCreate(); // добавляем лабиринт
            playerCreate(); // добавляем игрока
            tomatoCreate(50); // добавляем помидорки
            
            this.BackColor = Color.White; // задаём цвет фона
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.ControlRemoved += Form1_ControlRemoved;
        }

        // создание лабиринта
        private void mazeCreate()
        {
            maze = new Maze(this.ClientRectangle); // создаём объект "лабиринт"
            maze.RecursiveDivideRect(this.ClientRectangle);
        }

        //создание игрока
        private void playerCreate()
        {
            Rectangle playerArea = new Rectangle(this.ClientRectangle.Right / 3,this.ClientRectangle.Bottom / 3,this.ClientRectangle.Width / 3,this.ClientRectangle.Height / 3); // "площадь" игрока (картинки)
            player = new PlayerControl(); // создаём объект "игрок"
            player.Location = maze.GetLocation(player.Size, playerArea); // расположение игрока 
            Controls.Add(player); // добавляем объект "игрок" на форму
        }

        // создание помидорок
        private void tomatoCreate(int count)
        {
            tomatoes = new Tomatoes(count); // создаём массив помидорок в количестве count
            foreach (var t in tomatoes)
                t.Location = maze.GetLocation(t.Size, this.ClientRectangle); // расположение помидорок
            Controls.AddRange(tomatoes.ToArray()); // добавляем массив из помидорок на форму
        }

        // удаление помидорок
        private void Form1_ControlRemoved(object sender, ControlEventArgs e)
        {
            tomatoes.Remove(e.Control as PictureBox); // удаление изображения помидора
            soundPlayer.Play(); // воспроизведение звукового сигнала
            if (tomatoes.Count() == 0) // если помидорок не осталось
                endGame(); // конец игры
        }
        
        private void Form1_KeyUp(object sender, KeyEventArgs e) => keyDown.RemoveAll(key => key == e.KeyCode);
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(keyDown.FindAll(key => key == e.KeyCode).Count < 3) //Acceleration
                keyDown.Add(e.KeyCode);
            Point movement;
            keyDown.ForEach(key =>
            {
                if (move.TryGetValue(key, out movement))
                {
                    Rectangle moveTo = movePlayer(movement);
                    PlayerMoved(moveTo);
                }
            });
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillPath(Brushes.DarkBlue,this.maze.Path); // здесь определяется цвет стен
        }

        // движение игрока
        private Rectangle movePlayer(Point movement)
        {
            Rectangle moveTo = new Rectangle(player.Location, player.Size); // новое изображение игрока
            moveTo.Offset(movement);
            if (maze.isClear(moveTo))
                player.updateLocation(movement); // обновляем положение игроку
            return moveTo;
        }


        private void PlayerMoved(Rectangle moveTo)
        {
            if (tomatoes.isTaken(moveTo))
            {
                PictureBox x = tomatoes.First(t => t.Bounds.IntersectsWith(moveTo));
                Controls.Remove(x); // удаление помидорки с формы
            }
        }

        // конец игры
        private void endGame()
        {
            TimeSpan interval = DateTime.Now.Subtract(startTime);
            string time = $"{interval.Minutes}:{interval.Seconds}"; // время
            soundEnd.Play(); // звуковой сигнал о конце игры
            MessageBox.Show($"BRAVO! {time}"); // вывод сообщения об окончании игры и времени
        }

    }
}

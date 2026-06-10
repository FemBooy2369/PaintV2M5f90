using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinFormsApp8
{
    public partial class Form1 : Form
    {
        private int penisSize = 5;
        private Color primaryColor = Color.Black;
        private Color secondaryColor = Color.Red;
        private Bitmap canvas;
        private Point lastPoint;
        private bool painting = false;

        public Form1()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Zovдомик, Zovдерево и Zovсолнце";
            this.Size = new Size(920, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.DoubleBuffered = true;

            canvas = new Bitmap(800, 550);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
            }

            this.Paint += Form1_Paint;
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;

            MenuStrip menuStrip = new MenuStrip();
            this.MainMenuStrip = menuStrip;

            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Файл");
            ToolStripMenuItem editMenu = new ToolStripMenuItem("Правка");
            ToolStripMenuItem toolsMenu = new ToolStripMenuItem("Инструменты");

            ToolStripMenuItem saveItem = new ToolStripMenuItem("Сохранить как...");
            saveItem.Click += SaveImage;
            fileMenu.DropDownItems.Add(saveItem);

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Выход");
            exitItem.Click += (s, e) => Application.Exit();
            fileMenu.DropDownItems.Add(exitItem);

            ToolStripMenuItem clearItem = new ToolStripMenuItem("Очистить холст");
            clearItem.Click += ClearCanvas;
            editMenu.DropDownItems.Add(clearItem);

            ToolStripMenuItem colorItem = new ToolStripMenuItem("Выбрать основной цвет");
            colorItem.Click += ChoosePrimaryColor;
            toolsMenu.DropDownItems.Add(colorItem);

            ToolStripMenuItem color2Item = new ToolStripMenuItem("Выбрать второй цвет");
            color2Item.Click += ChooseSecondaryColor;
            toolsMenu.DropDownItems.Add(color2Item);

            ToolStripMenuItem sizeItem = new ToolStripMenuItem("Размер кисти");
            for (int i = 1; i <= 30; i += 2)
            {
                ToolStripMenuItem sizeSub = new ToolStripMenuItem(i.ToString());
                int sizeVal = i;
                sizeSub.Click += (s, e) => { penisSize = sizeVal; };
                sizeItem.DropDownItems.Add(sizeSub);
            }
            toolsMenu.DropDownItems.Add(sizeItem);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(editMenu);
            menuStrip.Items.Add(toolsMenu);
            this.Controls.Add(menuStrip);
        }

        private void Form1_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImage(canvas, 50, 100);

            // Отображение выбранных цветов
            using (SolidBrush primaryBrush = new SolidBrush(primaryColor))
            using (SolidBrush secondaryBrush = new SolidBrush(secondaryColor))
            {
                e.Graphics.FillRectangle(primaryBrush, 50, 50, 60, 40);
                e.Graphics.FillRectangle(secondaryBrush, 130, 50, 60, 40);

                e.Graphics.DrawString("Основной", new Font("Segoe UI", 10), Brushes.White, 50, 20);
                e.Graphics.DrawString("Второй (ПКМ)", new Font("Segoe UI", 10), Brushes.White, 130, 20);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Color drawColor = (e.Button == MouseButtons.Right) ? secondaryColor : primaryColor;

            painting = true;
            lastPoint = new Point(e.X - 50, e.Y - 100);

            // Рисуем первую точку
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(drawColor, penisSize))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;
                    g.FillEllipse(pen.Brush, lastPoint.X - penisSize / 2, lastPoint.Y - penisSize / 2, penisSize, penisSize);
                }
            }
            this.Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!painting) return;

            Point currentPoint = new Point(e.X - 50, e.Y - 100);

            if (currentPoint.X < 0 || currentPoint.Y < 0 ||
                currentPoint.X >= canvas.Width || currentPoint.Y >= canvas.Height)
                return;

            Color drawColor = (e.Button == MouseButtons.Right) ? secondaryColor : primaryColor;

            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using (Pen pen = new Pen(drawColor, penisSize))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;
                    g.DrawLine(pen, lastPoint, currentPoint);
                }
            }

            lastPoint = currentPoint;
            this.Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            painting = false;
        }

        private void ClearCanvas(object? sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
            }
            this.Invalidate();
        }

        private void ChoosePrimaryColor(object? sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    primaryColor = cd.Color;
                    this.Invalidate();
                }
            }
        }

        private void ChooseSecondaryColor(object? sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    secondaryColor = cd.Color;
                    this.Invalidate();
                }
            }
        }

        private void SaveImage(object? sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    canvas.Save(sfd.FileName);
                }
            }
        }
    }
}
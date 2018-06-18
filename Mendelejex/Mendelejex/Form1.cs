using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;


namespace Mendelejex
{
    //Dzięki słowu kluczowemu partial możliwe jest podzielenie klasy, struktury, interfejsu lub metody na kilka części w osobnych plikach.
    public partial class Form1 : Form //Klasa częściowa, ta klasa jest częścią po klasie Form, Kompozycja w UML
	{
		ElementBuilder builder;
		Bitmap bmp; // Obiekt, obszar pamięci zarezerwowany na rysowanie
		Rectangle tile; // Kafelek
		Size tileSize; // X, Y
		Color tileColor;
		Font font; // Informacja o czcionce
		List<KeyValuePair<Rectangle, Element>> tiles; // Lista <Przechowująca parę<Rectangle,Element>
		ToolTip tp; //Obiekt czysto windowsowy

		const int tileOffset = 3; // Offset pomiędzy kafelkami, 3 pikselowy odstep miedzy kafelkami

		public Form1() // Konstruktor, tworzy nam obiekt klasy Form1()
		{
			InitializeComponent();

			// Podstawowe ustawienia formatki (uniezależniamy się od designera)
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Size = new Size(1200, 860);
			this.FormBorderStyle = FormBorderStyle.FixedSingle; //Nie zmniejszamy, ani powiększamy
			this.AutoScaleMode = AutoScaleMode.Font; //Skalowanie czcionki
			this.BackColor = Color.White;
			this.MaximizeBox = false;
			this.Text = "Mendelejex";
			
			tileSize = new Size(55, 80); // Rozmiar kafelka
		}

		private void Form1_Load(object sender, EventArgs e) //Metoda wywołana podczas ładowania formatki
        {
			builder = new ElementBuilder(); // Inicjalizuję obiekt klasy element builder

			if(!builder.CreateElements()) // Jeżeli jest coś nie tak z wczytywaniem pliku, zamykamy aplikację
				Application.Exit();

			this.Paint += Form1_Paint; // Event do pętli odrysowywania formatki
			this.MouseClick += Form1_MouseClick; // Event do wykrywania kliknięcia myszą

			bmp = new Bitmap(this.Width, this.Height); // Tworzymy bufor do rysowania w pamięci
			GenerateTable(); // Rysujemy w buforze cały układ okresowy
		}

		private void Form1_MouseClick(object sender, MouseEventArgs e) //Event który się wywołuje kiedy klikniemy myszą
		{
			// Gdy LPM wciśnięty, sprawdzamy dla każdego elementu listy, czy kursor jest zawarty w danym kafelku.
			if(e.Button == MouseButtons.Left)
			{
				foreach(KeyValuePair<Rectangle, Element> tile in tiles) //Przelatujemy przez całą listę i sprawdzamy czy kursor(x,y) są zawarte w danym prostokącie 
				{
					if(tile.Key.Contains(e.Location))
					{
						// Jeśli jest zawarty, tworzymy tooltipa z określonymi wartościami i wyświetlamy go przez 2.5 sek.
						tp = new ToolTip();
						tp.ToolTipTitle = tile.Value.Nazwa;
						tp.Show("\nTemperatura topnienia: " + tile.Value.TempTopnienia +
							"°C\nTemperatura wrzenia: " + tile.Value.TempWrzenia + "°C", this, e.Location, 2500);
					}
				}
			}
		}

		private void Form1_Paint(object sender, PaintEventArgs e) //Przerzucenie z bufora bitmapy na ekran
		{
			Graphics g = e.Graphics; //Wielka klasa służąca do obsługi rysowania po formatce
			g.DrawImage(bmp, new Point(0, 0)); // Rysujemy wcześniej przygotowany bufor

			// Sprzątamy po narysowaniu
			if(tp != null && !string.IsNullOrEmpty(tp.GetToolTip(this)))
				tp.Dispose();
            bmp.Dispose();
			g.Dispose();
		}

		void GenerateTable() //Rysowanie na bitmapie (w pamięci)
		{
			Graphics g = Graphics.FromImage(bmp);
			g.TextRenderingHint = TextRenderingHint.AntiAlias;

			if(builder.GetElements() != null)
			{
				int x = 0, y = 0;
				int X_LocalOffset = 23;
				int Y_LocalOffset = 40;

				tiles = new List<KeyValuePair<Rectangle, Element>>();

				// Centrowanie wyświetlanych napisów
				StringFormat sf = new StringFormat
				{
					Alignment = StringAlignment.Center //Centrowanie napisów
				};

				#region Legenda
				font = new Font("Arial", 12, FontStyle.Regular);

				// Metale, półmetale, niemetale
				g.FillRectangle(new SolidBrush(Color.CornflowerBlue), new Rectangle(new Point(300, 60), new Size(130, 36)));
				g.FillRectangle(new SolidBrush(Color.MediumAquamarine), new Rectangle(new Point(300, 96), new Size(130, 36)));
				g.FillRectangle(new SolidBrush(Color.IndianRed), new Rectangle(new Point(300, 132), new Size(130, 36)));
				g.DrawString("metale", font, new SolidBrush(Color.Black), new Point(365, 68), sf);
				g.DrawString("półmetale", font, new SolidBrush(Color.Black), new Point(365, 105), sf);
				g.DrawString("niemetale", font, new SolidBrush(Color.Black), new Point(365, 140), sf);

				// Kafelek 'przykładowy pierwiastek'
				g.FillRectangle(new SolidBrush(Color.CornflowerBlue), new Rectangle(new Point(620, 65), tileSize));

				font = new Font("Arial", 10, FontStyle.Regular);
				g.DrawString(builder.GetElements()[42].LiczbaAtomowa.ToString(), font, new SolidBrush(Color.Black), new Point(622, 67));

				font = new Font("Arial", 18, FontStyle.Bold);
				g.DrawString(builder.GetElements()[42].Symbol, font, new SolidBrush(Color.Black), new Point(628, 83));

				font = new Font("Arial", 10, FontStyle.Italic);
				g.DrawString(builder.GetElements()[42].Nazwa, font, new SolidBrush(Color.Black), new Point(620, 110));

				font = new Font("Arial", 9, FontStyle.Regular);
				g.DrawString(builder.GetElements()[42].LiczbaMasowa.ToString(), font, new SolidBrush(Color.Black), new Point(622, 129));

				font = new Font("Arial", 10, FontStyle.Regular);
				g.DrawString("Liczba atomowa", font, new SolidBrush(Color.Black), new Point(510, 65));
				g.DrawString("Symbol", font, new SolidBrush(Color.Black), new Point(680, 90));
				g.DrawString("Nazwa", font, new SolidBrush(Color.Black), new Point(680, 110));
				g.DrawString("Masa", font, new SolidBrush(Color.Black), new Point(575, 131));

				// Obramówka lantanowców i aktynowców
				Pen p = new Pen(Color.DarkViolet, 3);
				PointF[] points =
				{
					new Point(1067, 641),
					new Point(252, 641),
					new Point(253, 641),
					new Point(253, 455),
					new Point(253, 456),
					new Point(194, 456),
					new Point(195, 456),
					new Point(195, 807),
					new Point(1067, 807),
					new Point(1065, 807),
					new Point(1065, 641)
				};
				g.DrawLines(p, points);
				#endregion

				#region Numery grup i okresów
				g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit; //Ustawiamy troszeczkę inne renderowanie
				font = new Font("Arial", 18, FontStyle.Bold);
				
				// Numery grup
				for(int i = 1; i <= 18; i++)
				{
					if(i == 1 || i == 18)
						g.DrawString(i.ToString(), font, new SolidBrush(Color.Black), new Point(50 + (i * (tileSize.Width + tileOffset)), Y_LocalOffset - 30), sf);
					if(i == 2 || (i > 12 && i < 18))
						g.DrawString(i.ToString(), font, new SolidBrush(Color.Black), new Point(50 + (i * (tileSize.Width + tileOffset)), Y_LocalOffset + 50), sf);
					if(i > 2 && i < 13)
						g.DrawString(i.ToString(), font, new SolidBrush(Color.Black), new Point(50 + (i * (tileSize.Width + tileOffset)), Y_LocalOffset + 215), sf);
				}

				// Numery okresów
				for(int i = 1; i < 8; i++)
				{
					g.DrawString(i.ToString(), font, new SolidBrush(Color.Black),
						new Point(65, (tileSize.Height / 2) + (i * (tileSize.Height + tileOffset)) - Y_LocalOffset - 15), sf);
				}
				#endregion

				foreach(Element el in builder.GetElements())
				{
					//Przeliczanie położenia kafelków
					if(el.Okres == 6 && (el.LiczbaAtomowa > 57 && el.LiczbaAtomowa < 72))
					{
						x = (el.Grupa * (tileSize.Width + tileOffset));
						y = (el.Okres * (tileSize.Height + tileOffset)) + (tileSize.Height * 3) - 55;
					}
					else if(el.Okres == 7 && (el.LiczbaAtomowa > 89 && el.LiczbaAtomowa < 104))
					{
						x = (el.Grupa * (tileSize.Width + tileOffset));
						y = (el.Okres * (tileSize.Height + tileOffset)) + (tileSize.Height * 3) - 55;
					}
					else
					{
						x = el.Grupa * (tileSize.Width + tileOffset);
						y = el.Okres * (tileSize.Height + tileOffset);
					}

                    // Położenia pierwiastków są już obliczone.
					// Tworzymy parę klucz-wartość obiektów Rectangle i Element, i przechowujemy ją na liście.
					// Dzięki temu każdy kafelek (a dokładniej jego położenie i rozmiar) jest powiązany z odpowiednim dla niego pierwiastkiem.
					tiles.Add(new KeyValuePair<Rectangle, Element>(new Rectangle(new Point(x + X_LocalOffset, y - Y_LocalOffset), tileSize), el));

					// Ustalenie koloru kafelka zależnie od właściwości metalicznych danego pierwiastka
					if(el.WlasciwosciMetaliczne == "Metal")
						tileColor = Color.CornflowerBlue;
					if(el.WlasciwosciMetaliczne == "Półmetal")
						tileColor = Color.MediumAquamarine;
					if(el.WlasciwosciMetaliczne == "Niemetal")
						tileColor = Color.IndianRed;

					// Tworzenie i rysowanie kafelka
					tile = new Rectangle(new Point(x + X_LocalOffset, y - Y_LocalOffset), tileSize);
					g.FillRectangle(new SolidBrush(tileColor), tile);

					#region Wartości w kafelkach
					g.TextRenderingHint = TextRenderingHint.AntiAlias;
					// Główny ciąg
					if((el.LiczbaAtomowa < 58 || el.LiczbaAtomowa > 71) &&
						(el.LiczbaAtomowa < 90 || el.LiczbaAtomowa > 103))
					{
						// Liczba atomowa
						font = new Font("Arial", 10, FontStyle.Regular);
						g.DrawString(el.LiczbaAtomowa.ToString(), font, new SolidBrush(Color.Black), new Point(x + X_LocalOffset + 2, y - Y_LocalOffset + 2));

						// Symbol
						font = new Font("Arial", 18, FontStyle.Bold);
						g.DrawString(el.Symbol, font, new SolidBrush(Color.Black), new Point(x + (tileSize.Width / 2) + X_LocalOffset, y - Y_LocalOffset + 18), sf);

						// Nazwa
						if(el.LiczbaAtomowa == 104 || el.LiczbaAtomowa == 110 || el.LiczbaAtomowa == 111 || el.LiczbaAtomowa == 112 ||
							el.LiczbaAtomowa == 113 || el.LiczbaAtomowa == 115 || el.LiczbaAtomowa == 116 || el.LiczbaAtomowa == 117 || el.LiczbaAtomowa == 118)
							font = new Font("Arial", 8, FontStyle.Italic);
						else
							font = new Font("Arial", 10, FontStyle.Italic);
						g.DrawString(el.Nazwa, font, new SolidBrush(Color.Black), new Point(x + (tileSize.Width / 2) + X_LocalOffset, y - Y_LocalOffset + 45), sf);

						// Masa
						font = new Font("Arial", 9, FontStyle.Regular);
						g.DrawString(el.LiczbaMasowa.ToString(), font, new SolidBrush(Color.Black), new Point(x + X_LocalOffset + 2, y - Y_LocalOffset + 64));
					}
					else // Lantanowce i aktynowce
					{
						// Liczba atomowa
						font = new Font("Arial", 10, FontStyle.Regular);
						g.DrawString(el.LiczbaAtomowa.ToString(), font, new SolidBrush(Color.Black), new Point(x + X_LocalOffset + 2, y - Y_LocalOffset + 2));

						// Symbol
						font = new Font("Arial", 18, FontStyle.Bold);
						g.DrawString(el.Symbol, font, new SolidBrush(Color.Black), new Point(x + (tileSize.Width / 2) + X_LocalOffset, y - Y_LocalOffset + 18), sf);

						// Nazwa
						if(el.LiczbaAtomowa == 59 || el.LiczbaAtomowa == 101)
							font = new Font("Arial", 8, FontStyle.Italic);
						else
							font = new Font("Arial", 10, FontStyle.Italic);
						g.DrawString(el.Nazwa, font, new SolidBrush(Color.Black), new Point(x + (tileSize.Width / 2) + X_LocalOffset, y - Y_LocalOffset + 45), sf);

						// Masa
						font = new Font("Arial", 9, FontStyle.Regular);
						g.DrawString(el.LiczbaMasowa.ToString(), font, new SolidBrush(Color.Black), new Point(x + X_LocalOffset + 2, y - Y_LocalOffset + 64));
					}
					#endregion
				}
				g.Dispose(); // Sprzątamy
			}
		}
	}
}

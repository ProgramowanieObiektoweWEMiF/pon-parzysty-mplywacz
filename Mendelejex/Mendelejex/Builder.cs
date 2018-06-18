using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Windows.Forms;


namespace Mendelejex
{
	public interface IBuilder
	{
		bool CreateElements();
	}

	public class Element
	{
		public int LiczbaAtomowa { get; set; }
		public string Symbol { get; set; }
		public string Nazwa { get; set; }
		public float LiczbaMasowa { get; set; }
		public string WlasciwosciMetaliczne { get; set; }
		public float TempTopnienia { get; set; }
		public float TempWrzenia { get; set; }
		public int Grupa { get; set; }
		public int Okres { get; set; }


		public Element(int latom, string sym, string nazwa, float masa, string wlascMet, float top, float wrz, int grupa, int okres)
		{
			LiczbaAtomowa = latom;
			Symbol = sym;
			Nazwa = nazwa;
			LiczbaMasowa = masa;
			WlasciwosciMetaliczne = wlascMet;
			TempTopnienia = top;
			TempWrzenia = wrz;
			Grupa = grupa;
			Okres = okres;
		}
	}

	public class ElementBuilder : IBuilder
	{
		List<Element> elements;
		const string file = @"Pierwiastki.csv";

		public bool CreateElements()
		{
			if(!File.Exists(file))
			{
				MessageBox.Show("Plik \"Pierwiastki.csv\" nie istnieje lub jest uszkodzony!", "Błąd wczytywania pliku", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			NumberFormatInfo nfi = new NumberFormatInfo(); //Każdy kraj inaczej formatuje liczby
			nfi.NegativeSign = "-";
			nfi.NumberDecimalSeparator = ".";

			elements = new List<Element>(); // Inicjalizacja listy
			using(var reader = new StreamReader(file)) //Using - tworzy zmienną lokalną i ta zmiennna jest używana tylko w tylko w tym bloku 
			{
				while(!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					var val = line.Split(',');

					elements.Add(new Element(int.Parse(val[0]), val[1], val[2], float.Parse(val[3], nfi), val[4],
						(val[5] == "NaN" ? float.NaN : float.Parse(val[5], nfi)), (val[6] == "NaN" ? float.NaN : float.Parse(val[6] , nfi)),
						int.Parse(val[7]), int.Parse(val[8])));
				}
				reader.Close();
			}
			return true;
		}

		public List<Element> GetElements() //Zwraca listę
		{
			return elements;
		}
	}
}

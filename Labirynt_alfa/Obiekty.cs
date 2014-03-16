using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labirynt_alfa
{
    enum DIR { D_UP = 0, D_DOWN, D_RIGHT, D_LEFT };

    abstract class Obiekt
    {
        //Pola odpowiedzialne za polozenia
        protected int m_X;
        protected int m_Y;

        public int GetX() { return m_X; }
        public int GetY() { return m_Y; }
    }


    class Gracz : Obiekt
    {
        int m_hp;
        int m_Punkty;
        List<Klucz> m_Posiadane_klucze;

        public Gracz(Komnata obecna)
        {
            Random rand = new Random();
            m_Posiadane_klucze = new List<Klucz>();

            do
            {
                m_X = rand.Next() % (obecna.GetSizeX() - 2) + 1;
                m_Y = rand.Next() % (obecna.GetSizeY() - 2) + 1;

            } while (obecna.GetFiled()[m_X, m_Y] != FIELD.F_WOLNE);

            m_hp = 100;
            m_Punkty = 0;
        }

        public int GetHP() { return m_hp; }
        public int GetPunkty() { return m_Punkty; }
        public void AddPunkty(int wartosc) { m_Punkty += wartosc; }

        public void AddKey(Klucz Key) { if (Key != null) m_Posiadane_klucze.Add(Key); }
        public Klucz UseKey(int ID)
        {
            for (int i = 0; i < m_Posiadane_klucze.Count; i++)
            {
                if (m_Posiadane_klucze[i].GetID() == ID)
                    return m_Posiadane_klucze[i];
            }

            return null;
        }
        public int GetLiczbaKluczy() { return m_Posiadane_klucze.Count; }

        public void SetPoz(int X = 0, int Y = 0)
        {
            m_X = X;
            m_Y = Y;
        }

        //zmiejszenie zycia
        public bool DecreaseHP(int ile)
        {
            m_hp -= ile;

            if (m_hp < 0) m_hp = 0;

            return m_hp == 0 ? false : true;
        }

        //zmiejszenie hp
        public void DecreasePunkty(int ile)
        {
            m_Punkty -= ile;

            if (m_Punkty < 0)
            {
                this.DecreaseHP(-m_Punkty);
                m_Punkty = 0;
            }
        }

    }

    class Przejscie : Obiekt
    {
        protected bool m_zamkniete;
        protected Komnata m_nastepna;
        protected Komnata m_obecna;
        protected int m_id;

        protected static int last_id = 0;

        public Przejscie()
        {
            m_X = 1;
            m_Y = 1;

            m_obecna = null;
            m_nastepna = null;

            m_zamkniete = true;
        }

        public Przejscie(int X, int Y, bool zamknij, Komnata obecna, Komnata nastepna = null, int id = -1)
        {
            //wymagane do robienia tych samych drzwi w dwuch komnatach
            if (id == -1)
            {
                m_id = last_id;
                ++last_id;
            }
            else m_id = id;

            m_X = X;
            m_Y = Y;

            m_obecna = obecna;
            m_nastepna = nastepna;

            m_zamkniete = zamknij;
        }

        public void SetNastena(Komnata nastepna) { m_nastepna = nastepna; }
        public Komnata Przejdz() { return m_nastepna; }

        public int GetID() { return m_id; }

        public bool is_lock() { return m_zamkniete; }
        public void Open(Klucz key)
        {
            if (key != null && key.GetID() == m_id)
                m_zamkniete = false;
        }

        public virtual bool is_win_door() { return false; }
    }


    class WinDoor : Przejscie
    {
        public WinDoor(int X, int Y)
        {
            m_id = -10;

            m_X = X;
            m_Y = Y;

            m_obecna = null;
            m_nastepna = null;

            m_zamkniete = true;
        }

        //public bool is_win_door() { return true; }
    }

    class Klucz : Obiekt
    {
        private int m_id;

        public Klucz(int ID)
        {
            m_id = ID;
        }

        public void SetPoz(int X, int Y) { m_X = X; m_Y = Y; }

        public int GetID() { return m_id; }
    }


    class Skarb : Obiekt
    {
        private int m_wartosc;

        public Skarb(int X, int Y)
        {
            m_X = X;
            m_Y = Y;

            m_wartosc = 2;
        }

        public int GetWartosc() { return m_wartosc; }
    }

    /*dodaj poruszanie sie*/
    class Thief : Obiekt
    {
        private static Random rand = new Random();

        private int m_Atak;
        private int m_Skarby;
        private int m_Zasieg;

        public Thief(int X, int Y)
        {
            m_X = X;
            m_Y = Y;

            //ewenetualnie przerobic na generowane losowo przy wejsci w zasieg
            m_Atak = rand.Next() % 5 + 1;
            m_Skarby = rand.Next() % 10 + 1;

            m_Zasieg = 2;//rand.Next() % 3 + 1;
        }

        public bool is_in_range(int X, int Y)
        {
            return Math.Sqrt(Math.Pow(X - m_X, 2) + Math.Pow(Y - m_Y, 2)) < m_Zasieg;
        }

        //public void move(int x; int y)

        public int GetIleSkarbow() { return m_Skarby; }
        public int GetIleAtak() { return m_Atak; }

        public void Move(DIR kierunek)
        {
            switch (kierunek)
            {
                case DIR.D_UP: m_Y -= 1; break;
                case DIR.D_DOWN: m_Y += 1; break;
                case DIR.D_LEFT: m_X -= 1; break;
                case DIR.D_RIGHT: m_X += 1; break;
            }
        }
    }
    
}

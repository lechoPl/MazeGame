using System;
using System.Collections.Generic;

namespace Labirynt_alfa
{
    enum FIELD { F_WOLNE = 0, F_ZAJETE };

    class Podziemia
    {
        private Komnata m_root;
        private Komnata m_obecna;

        public static int Pozostalo_komnat;
        public static int Liczba_komnat;

        public Podziemia()
        {
            Random rand = new Random();

            Pozostalo_komnat = rand.Next() % 1 + 3;
            Liczba_komnat = Pozostalo_komnat;

            m_root = new Komnata();
            m_obecna = m_root;
        }

        public Komnata GetObecnna() { return m_obecna; }
        public void SetObecna(Komnata nastepna) { m_obecna = nastepna; }
    }


    class Komnata
    {
        private static int g_IloscGotowychKomnat = 0;
        private static int mc_LiczbaUtworzonychKomnat = 0;
        private static int mc_LiczbaRoztawionychKluczy = 0;
        private static int mc_LiczbaOtwartychDrzwi = 0;

        private int mc_pozostalo_pol = 0;

        private int[] m_size_xy; //[0] <- x; [1] <- y
        private FIELD[,] m_field; //1..30 1..60

        private int m_liczba_pustych_pol;
        static private Random rand;

        static private List<Klucz> ListaKluczyDoWstawienia;
        static bool AddWinDoor = true;
        static bool AddWinKey = true;

        private Przejscie[] m_door;
        private Skarb[] m_skarby;
        private Thief[] m_thief;
        private Klucz[] m_klucze;
        private WinDoor _WinDoor;
        private Klucz _KluczWinDoor;

        private void GenKomnata()
        {
            if (rand == null) rand = new Random();

            int[] Pozycja = new int[2]; //[0] <- x; [1] <- y

            Pozycja[0] = rand.Next() % (m_size_xy[0] - 2) + 1;
            Pozycja[1] = rand.Next() % (m_size_xy[1] - 2) + 1;

            m_field = new FIELD[m_size_xy[0], m_size_xy[1]];

            //czyszczenie labiryntu
            for (int y = 0; y < m_size_xy[1]; y++)
            {
                for (int x = 0; x < m_size_xy[0]; x++)
                    m_field[x, y] = FIELD.F_ZAJETE;
            }
            m_field[Pozycja[0], Pozycja[1]] = FIELD.F_WOLNE;
            m_liczba_pustych_pol = 1;


            DIR kierunek;

            int WolnePola = m_size_xy[0] * m_size_xy[1] / 2;

            //liczba pol ktore maja byc pokonna w danym kierunku o ile jest to mozliwe
            int ile;

            while (m_liczba_pustych_pol < WolnePola)
            {
                kierunek = (DIR)(rand.Next() % 4);
                ile = rand.Next() % 5 + 1;

                for (int j = 0; j < ile; j++)
                {
                    switch (kierunek)
                    {
                        case DIR.D_UP:
                            if (Pozycja[1] - 1 > 0)
                            {
                                if (m_field[Pozycja[0], Pozycja[1] - 1] == FIELD.F_ZAJETE) ++m_liczba_pustych_pol;

                                m_field[Pozycja[0], Pozycja[1] - 1] = FIELD.F_WOLNE;
                                Pozycja[1] = Pozycja[1] - 1;
                            };
                            break;

                        case DIR.D_DOWN:
                            if (Pozycja[1] + 1 < m_size_xy[1] - 1)
                            {
                                if (m_field[Pozycja[0], Pozycja[1] + 1] == FIELD.F_ZAJETE) ++m_liczba_pustych_pol;

                                m_field[Pozycja[0], Pozycja[1] + 1] = FIELD.F_WOLNE;

                                Pozycja[1] = Pozycja[1] + 1;
                            };
                            break;

                        case DIR.D_LEFT:
                            if (Pozycja[0] - 1 > 0)
                            {
                                if (m_field[Pozycja[0] - 1, Pozycja[1]] == FIELD.F_ZAJETE) ++m_liczba_pustych_pol;

                                m_field[Pozycja[0] - 1, Pozycja[1]] = FIELD.F_WOLNE;

                                Pozycja[0] = Pozycja[0] - 1;
                            };
                            break;

                        case DIR.D_RIGHT:
                            if (Pozycja[0] + 1 < m_size_xy[0] - 1)
                            {
                                if (m_field[Pozycja[0] + 1, Pozycja[1]] == FIELD.F_ZAJETE) ++m_liczba_pustych_pol;

                                m_field[Pozycja[0] + 1, Pozycja[1]] = FIELD.F_WOLNE;

                                Pozycja[0] = Pozycja[0] + 1;
                            };
                            break;
                    }
                }
            }

            mc_pozostalo_pol = m_liczba_pustych_pol;
        }
        private void GenWsp(ref int X, ref int Y)
        {
            bool wsp_is_ok;

            //ustalanie pozycji klucza
            do
            {
                wsp_is_ok = true;

                X = rand.Next() % m_size_xy[0];
                Y = rand.Next() % m_size_xy[1];

                if (m_field[X, Y] == FIELD.F_ZAJETE)
                {
                    wsp_is_ok = false;
                    continue;
                }

                //sprawdzenie czy nie pokrywa sie z polozeniem drzwi
                if (m_door != null)
                {
                    for (int j = 0; j < m_door.Length; j++)
                    {
                        if (m_door[j] != null && m_door[j].GetX() == X && m_door[j].GetY() == Y)
                        {
                            wsp_is_ok = false;
                            break;
                        }
                    }
                    if (wsp_is_ok == false) continue;
                }

                //sprawdzenie czy nie pokrywa sie polozeniem skarbow
                if (m_skarby != null)
                {
                    for (int j = 0; j < m_skarby.Length; j++)
                    {
                        if (m_skarby[j] != null && m_skarby[j].GetX() == X && m_skarby[j].GetY() == Y)
                        {
                            wsp_is_ok = false;
                            break;
                        }
                    }
                    if (wsp_is_ok == false) continue;
                }

                //sprawdzenie czy nie pokrywa sie polozenie zlodzieji
                if (m_thief != null)
                {
                    for (int j = 0; j < m_thief.Length; j++)
                    {
                        if (m_thief[j] != null && m_thief[j].GetX() == X && m_thief[j].GetY() == Y)
                        {
                            wsp_is_ok = false;
                            break;
                        }
                    }
                    if (wsp_is_ok == false) continue;
                }

                //sprawdzenie czy nie pokrywa sie z kluczami
                if (m_klucze != null)
                {
                    for (int j = 0; j < m_klucze.Length; j++)
                    {
                        if (m_klucze[j] != null && m_klucze[j].GetX() == X && m_klucze[j].GetY() == Y)
                        {
                            wsp_is_ok = false;
                            break;
                        }
                    }
                    if (wsp_is_ok == false) continue;
                }

                if (_WinDoor != null)
                {
                    if (_WinDoor.GetX() == X && _WinDoor.GetY() == Y) wsp_is_ok = false;
                }

                if (_KluczWinDoor != null)
                {
                    if (_KluczWinDoor.GetX() == X && _KluczWinDoor.GetY() == Y) wsp_is_ok = false;
                }

            } while (!wsp_is_ok);
        }
        private void WstawDrzwi(Komnata komnata_poprzednia = null, int id_przejscia = -1)
        {
            if (rand == null) rand = new Random();

            int ile = 0;

            if (Podziemia.Pozostalo_komnat != 0)
            {
                do
                {
                    ile = rand.Next() % 5 + 1;
                } while (ile > mc_pozostalo_pol - 1 || ile > Podziemia.Pozostalo_komnat);

                Podziemia.Pozostalo_komnat = Podziemia.Pozostalo_komnat - ile;
            }

            //zwiekszenie liczby komnat o ile trzeba dodac drzwi do poprzednije komnaty
            if (komnata_poprzednia != null) ++ile;

            mc_pozostalo_pol -= ile;

            m_door = new Przejscie[ile];

            int X = 0;
            int Y = 0;

            for (int i = 0; i < ile; i++)
            {
                GenWsp(ref X, ref Y);

                /** tworzenie odpowiedniego przejscia **/

                // wstawienie przejscia do poprzedniej komnaty
                if (i == 0 && komnata_poprzednia != null)
                {
                    m_door[i] = new Przejscie(X, Y, false, this, komnata_poprzednia, id_przejscia);
                    continue;
                }

                //30% szans na stowrzenie przejscia do ktorego nie jest wymagany klucz
                if (rand.Next() % 10 < 3)
                {
                    m_door[i] = new Przejscie(X, Y, false, this);

                    ++mc_LiczbaOtwartychDrzwi;
                }
                else
                {
                    if (ListaKluczyDoWstawienia == null) ListaKluczyDoWstawienia = new List<Klucz>();

                    m_door[i] = new Przejscie(X, Y, true, this);

                    ListaKluczyDoWstawienia.Add(new Klucz(m_door[i].GetID()));
                }
            }
        }
        private void WstawSkarby()
        {
            if (rand == null) rand = new Random();

            int ile = 0;

            do
            {
                ile = rand.Next() % 10;
            } while (ile > mc_pozostalo_pol/*m_liczba_pustych_pol - m_door.Length*/ );

            mc_pozostalo_pol -= ile;

            m_skarby = new Skarb[ile];

            int X = 0;
            int Y = 0;

            for (int i = 0; i < ile; i++)
            {
                GenWsp(ref X, ref Y);

                m_skarby[i] = new Skarb(X, Y);
            }
        }
        private void WstawThief()
        {
            if (rand == null) rand = new Random();

            int ile;
            do
            {
                ile = rand.Next() % 6;
            } while (ile > mc_pozostalo_pol/*m_liczba_pustych_pol - m_door.Length - m_skarby.Length*/);

            mc_pozostalo_pol -= ile;

            m_thief = new Thief[ile];

            int X = 0;
            int Y = 0;

            for (int i = 0; i < ile; i++)
            {
                GenWsp(ref X, ref Y);
                m_thief[i] = new Thief(X, Y);
            }
        }
        //Troche do poprawienia
        private void WstawKlucze()
        {
            if (rand == null) rand = new Random();

            //Sprawdzanie czy sa jakies klucze czekajace na wstawienie
            if (ListaKluczyDoWstawienia == null || ListaKluczyDoWstawienia.Count == 0) return;

            int ile = 0; // liczba kluczy do wstawienia w danej komnacie

            if (Podziemia.Pozostalo_komnat != 0)
            {
                do
                {
                    ile = rand.Next() % ListaKluczyDoWstawienia.Count;

                    if (ile != ListaKluczyDoWstawienia.Count && rand.Next() % 10 < 4) ++ile;

                } while (ile + mc_LiczbaRoztawionychKluczy + mc_LiczbaOtwartychDrzwi < mc_LiczbaUtworzonychKomnat || ile > mc_pozostalo_pol);

            }
            else ile = ListaKluczyDoWstawienia.Count;


            //sprawdzanie czy nie trzeba wstawic klucza aby przedostac sie do reszty komnat
            bool AllClose = true;

            //brak drzwi traktujemy tak jakby wszystkie byly otwarte
            if (m_door.Length == 0) AllClose = false;

            //sprawdznie czy wszystkie sa zamkniete
            for (int i = 0; i < m_door.Length; i++)
            {
                if (m_door[i].is_lock() == false)
                {
                    AllClose = false;
                    break;
                }
            }

            //w ostaniej komnacie na pewno nie bedziemy mieli zadnych drzwi wiec w szczegolnosci
            //wszystkie drzwi nie beda zamkniiete

            //zmienna przechowujaca klucz ktory jest wymagany do przejscia dalej
            Klucz WymaganyKlucz = null;

            if (AllClose)
            {
                //sprawdzenie czy klucz do przynajmniej jednych drzwi nie byl juz wczesniej umieszczony w podziemiach
                int temp_ID;
                bool wystapil = false;

                for (int i = 0; i < m_door.Length; i++)
                {
                    temp_ID = m_door[i].GetID();
                    wystapil = false;

                    //sprawdzanie czy klucz do danych drzwi znajduje sie na liscie
                    for (int j = 0; j < ListaKluczyDoWstawienia.Count; j++)
                    {
                        if (ListaKluczyDoWstawienia[j].GetID() == temp_ID) wystapil = true;
                    }

                    if (!wystapil) break; // klucza do przynajmniej jednych drzwi nie ma na liscie => jest juz w jakiejsc komnacie
                }

                if (wystapil) // trzeba umiescic klucz do drzwi z danej komnaty
                {
                    temp_ID = m_door[rand.Next() % m_door.Length].GetID();

                    for (int i = 0; i < ListaKluczyDoWstawienia.Count; i++)
                    {
                        if (ListaKluczyDoWstawienia[i].GetID() == temp_ID)
                        {
                            WymaganyKlucz = ListaKluczyDoWstawienia[i];
                            ListaKluczyDoWstawienia.Remove(WymaganyKlucz);

                            break;
                        }
                    }
                }
            }

            if (ile == 0 && WymaganyKlucz != null) ++ile;

            m_klucze = new Klucz[ile];

            mc_LiczbaRoztawionychKluczy += ile;

            int nr_klucza_z_listy;

            int X = 0;
            int Y = 0;

            for (int i = 0; i < ile; i++)
            {
                if (i == 0 && WymaganyKlucz != null) m_klucze[i] = WymaganyKlucz;
                else
                {
                    //wylosowanie klucza do wstawienia
                    nr_klucza_z_listy = rand.Next() % ListaKluczyDoWstawienia.Count;

                    m_klucze[i] = /*ListaKluczyDoWstawienia[0];//*/ListaKluczyDoWstawienia[nr_klucza_z_listy];

                    ListaKluczyDoWstawienia.Remove(m_klucze[i]);
                    //m_klucze[i] = ListaKluczyDoWstawienia[0];
                    //ListaKluczyDoWstawienia.Remove(m_klucze[i]);
                }

                //ustalanie pozycji klucza
                GenWsp(ref X, ref Y);
                if (m_klucze[i] != null) m_klucze[i].SetPoz(X, Y);
            }

        }
        private void UstawienieOstatnichDrzwi()
        {
            if (AddWinDoor && g_IloscGotowychKomnat == Podziemia.Liczba_komnat)
            {
                AddWinDoor = false;

                int X = 0;
                int Y = 0;

                GenWsp(ref X, ref Y);

                _WinDoor = new WinDoor(X, Y);
            }
        }
        private void UstawienieKluczDoOstatnichDrzwi()
        {
            if (AddWinKey && (g_IloscGotowychKomnat == Podziemia.Liczba_komnat - 1 || rand.Next() % 100 < 30))
            {
                AddWinKey = false;

                int X = 0;
                int Y = 0;

                GenWsp(ref X, ref Y);

                _KluczWinDoor = new Klucz(-10);
                _KluczWinDoor.SetPoz(X, Y);
            }
        }
        private void TworzenieKolejnychKomnat()
        {
            if (m_door != null)
            {
                for (int i = 0; i < m_door.Length; i++)
                {
                    if (m_door[i].Przejdz() == null && !m_door[i].is_lock())
                        m_door[i].SetNastena(new Komnata(this, m_door[i].GetID()));

                }

                for (int i = 0; i < m_door.Length; i++)
                {
                    if (m_door[i].Przejdz() == null && m_door[i].is_lock())
                        m_door[i].SetNastena(new Komnata(this, m_door[i].GetID()));

                }
            }
        }

        /* KONSTRUKTOR */
        /***************/
        public Komnata(Komnata Komnata_Pop = null, int id_przejscia = -1)
        {
            ++g_IloscGotowychKomnat;
            ++mc_LiczbaUtworzonychKomnat;

            m_size_xy = new int[2];

            if (rand == null) rand = new Random();

            //tworzenie losowego rozmiaru komnaty
            m_size_xy[0] = 10 + rand.Next() % 45; // os X
            m_size_xy[1] = 5 + rand.Next() % 20; // os y

            //genertownie wygladu komnaty
            GenKomnata();

            WstawDrzwi(Komnata_Pop, id_przejscia);

            WstawSkarby();

            WstawThief();

            //popraw wstawianie kluczy
            WstawKlucze();

            UstawienieOstatnichDrzwi();
            UstawienieKluczDoOstatnichDrzwi();

            TworzenieKolejnychKomnat();
            //ewnetulanie wstawienie drzwi konca gry
        }

        public bool is_Przejscie(int X, int Y)
        {
            for (int i = 0; i < m_door.Length; i++)
            {
                if (m_door[i].GetX() == X && m_door[i].GetY() == Y)
                    return true;
            }

            return false;
        }
        public bool is_Skarb(int X, int Y)
        {
            if (m_skarby != null)
            {
                for (int i = 0; i < m_skarby.Length; i++)
                {
                    if (m_skarby[i].GetX() == X && m_skarby[i].GetY() == Y)
                        return true;
                }
            }

            return false;
        }
        public void is_thief(Gracz Player)
        {
            if (m_thief != null)
            {
                for (int i = 0; i < m_thief.Length; i++)
                {
                    if (m_thief[i].is_in_range(Player.GetX(), Player.GetY()))
                    {
                        Player.DecreasePunkty(m_thief[i].GetIleSkarbow());
                        Player.DecreaseHP(m_thief[i].GetIleAtak());
                    }
                }
            }
        }
        public bool is_key(int X, int Y)
        {
            if (m_klucze != null)
            {
                for (int i = 0; i < m_klucze.Length; i++)
                {
                    if (m_klucze[i].GetX() == X && m_klucze[i].GetY() == Y)
                        return true;
                }
            }

            return false;
        }
        public bool is_WinDoor(int X, int Y)
        {
            if (_WinDoor != null)
                return _WinDoor.GetX() == X && _WinDoor.GetY() == Y;
            else
                return false;
        }
        public bool is_KeyWinDoor(int X, int Y)
        {
            if (_KluczWinDoor != null)
                return _KluczWinDoor.GetX() == X && _KluczWinDoor.GetY() == Y;
            else
                return false;
        }

        public Skarb RemoveSkarb(int X, int Y)
        {
            Skarb temp = null;

            if (m_skarby.Length > 1)
            {
                Skarb[] ListaSkarbow = m_skarby;
                m_skarby = new Skarb[ListaSkarbow.Length - 1];

                for (int i = 0, j = 0; i < ListaSkarbow.Length; i++)
                {
                    if (ListaSkarbow[i].GetX() == X && ListaSkarbow[i].GetY() == Y)
                        temp = ListaSkarbow[i];
                    else
                    {
                        //temp = ListaSkarbow[0];
                        m_skarby[j] = ListaSkarbow[i];
                        ++j;
                    }
                }
            }
            else
            {
                if (m_skarby.Length == 1) temp = m_skarby[0];
                m_skarby = null;
            }

            return temp;
        }
        public Klucz RemoveKey(int X, int Y)
        {
            Klucz temp = null;

            if (m_klucze.Length > 1)
            {
                Klucz[] ListaKluczy = m_klucze;
                m_klucze = new Klucz[ListaKluczy.Length - 1];

                for (int i = 0, j = 0; i < ListaKluczy.Length; i++)
                {
                    if (ListaKluczy[i].GetX() == X && ListaKluczy[i].GetY() == Y)
                        temp = ListaKluczy[i];
                    else
                    {
                        //temp = ListaSkarbow[0];
                        m_klucze[j] = ListaKluczy[i];
                        ++j;
                    }
                }
            }
            else
            {
                if (m_klucze.Length == 1) temp = m_klucze[0];
                m_klucze = null;
            }

            return temp;
        }
        public Klucz RemoveWinKey(int X, int Y)
        {
            if (is_KeyWinDoor(X, Y))
            {
                Klucz temp;

                temp = _KluczWinDoor;
                _KluczWinDoor = null;

                return temp;
            }

            return null;
        }

        //Metody do wyswietlania
        public FIELD[,] GetFiled() { return m_field; }
        public Przejscie[] GetDoors() { return m_door; }
        public Skarb[] GetSkarby() { return m_skarby; }
        public Thief[] GetThief() { return m_thief; }
        public Klucz[] GetKlucze() { return m_klucze; }
        public WinDoor GetWinDoor() { return _WinDoor; }
        public Klucz GetWinKey() { return _KluczWinDoor; }

        //do przejscia miedzy komnatami
        public Przejscie GetDoor(int X, int Y)
        {
            for (int i = 0; i < m_door.Length; i++)
            {
                if (m_door[i].GetX() == X && m_door[i].GetY() == Y)
                    return m_door[i];
            }

            return null;
        }
        public Przejscie FindDoor(int id)
        {
            for (int i = 0; i < m_door.Length; i++)
            {
                if (m_door[i].GetID() == id)
                    return m_door[i];
            }

            return null;
        }

        //Rozmiar obecnej planszy
        public int GetSizeX() { return m_size_xy[0]; }
        public int GetSizeY() { return m_size_xy[1]; }

    }
}

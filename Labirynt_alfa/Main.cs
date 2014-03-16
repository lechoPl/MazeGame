using System;
using System.Threading;
using System.Collections.Generic; //Do obslugi list

namespace Labirynt_alfa
{
    enum STANGRY { S_NEW = 0, S_WON, S_GAMEOVER };

    class Game
    {
        static void Main(string[] args)
        {
            Kontroler init = new Kontroler();
        }
    }

    /*********************************************************************************************************************/
    class Kontroler
    {
        /* Dorobic cos w stylu watkow aby zlodzieje sie ruszali bez naciskania klawiaszy */
        static Random rand;

        Podziemia main_Podziemia;
        Gracz Zawodnik;
        
        string komunikat;
        STANGRY GAME_Stan;

        //private void ObslugaWejscia(char Key)
        
        private void MoveTief()
        {
            if (main_Podziemia.GetObecnna().GetThief() != null)
            {
                if (rand == null) rand = new Random();

                DIR kierunek;
                bool ruszyc;

                for (int i = 0; i < main_Podziemia.GetObecnna().GetThief().Length; i++)
                {
                    kierunek = (DIR)(rand.Next() % 4);
                    ruszyc = true;

                    switch (kierunek)
                    {
                        case DIR.D_UP:
                            if (main_Podziemia.GetObecnna().GetFiled()[main_Podziemia.GetObecnna().GetThief()[i].GetX(), main_Podziemia.GetObecnna().GetThief()[i].GetY() - 1] == FIELD.F_ZAJETE)
                                ruszyc = false; break;
                        case DIR.D_DOWN:
                            if (main_Podziemia.GetObecnna().GetFiled()[main_Podziemia.GetObecnna().GetThief()[i].GetX(), main_Podziemia.GetObecnna().GetThief()[i].GetY() + 1] == FIELD.F_ZAJETE)
                                ruszyc = false; break;
                        case DIR.D_LEFT:
                            if (main_Podziemia.GetObecnna().GetFiled()[main_Podziemia.GetObecnna().GetThief()[i].GetX() - 1, main_Podziemia.GetObecnna().GetThief()[i].GetY()] == FIELD.F_ZAJETE)
                                ruszyc = false; break;
                        case DIR.D_RIGHT:
                            if (main_Podziemia.GetObecnna().GetFiled()[main_Podziemia.GetObecnna().GetThief()[i].GetX() + 1, main_Podziemia.GetObecnna().GetThief()[i].GetY() - 1] == FIELD.F_ZAJETE)
                                ruszyc = false; break;
                    }

                    if (ruszyc) main_Podziemia.GetObecnna().GetThief()[i].Move(kierunek);
                }
            }
        }
        private void MovePlayer( char wejscie)
        {
            //char wejscie = Console.ReadKey().KeyChar;

            switch (wejscie)
            {
                case 'w':
                    if (main_Podziemia.GetObecnna().GetFiled()[Zawodnik.GetX(), Zawodnik.GetY() - 1] == FIELD.F_WOLNE)
                        Zawodnik.SetPoz(Zawodnik.GetX(), Zawodnik.GetY() - 1);
                    break;
                case 's':
                    if (main_Podziemia.GetObecnna().GetFiled()[Zawodnik.GetX(), Zawodnik.GetY() + 1] == FIELD.F_WOLNE)
                        Zawodnik.SetPoz(Zawodnik.GetX(), Zawodnik.GetY() + 1);
                    break;
                case 'a':
                    if (main_Podziemia.GetObecnna().GetFiled()[Zawodnik.GetX() - 1, Zawodnik.GetY()] == FIELD.F_WOLNE)
                        Zawodnik.SetPoz(Zawodnik.GetX() - 1, Zawodnik.GetY());
                    break;
                case 'd':
                    if (main_Podziemia.GetObecnna().GetFiled()[Zawodnik.GetX() + 1, Zawodnik.GetY()] == FIELD.F_WOLNE)
                        Zawodnik.SetPoz(Zawodnik.GetX() + 1, Zawodnik.GetY());
                    break;

                /*****************/
                /* PRZECHODZENIE */
                /*****************/
                case 'o':
                    if (main_Podziemia.GetObecnna().is_Przejscie(Zawodnik.GetX(), Zawodnik.GetY()))
                    {
                        Przejscie temp_przejscie = main_Podziemia.GetObecnna().GetDoor(Zawodnik.GetX(), Zawodnik.GetY());

                        if (!temp_przejscie.is_lock())
                        {
                            //poraznie id drzwi przez ktore zmieniana jest komnata
                            int temp_id = main_Podziemia.GetObecnna().GetDoor(Zawodnik.GetX(), Zawodnik.GetY()).GetID();

                            //zmiana obecnej komnaty
                            main_Podziemia.SetObecna(main_Podziemia.GetObecnna().GetDoor(Zawodnik.GetX(), Zawodnik.GetY()).Przejdz());

                            //odnalezienie drzwi o danym id
                            Przejscie temp_door = main_Podziemia.GetObecnna().FindDoor(temp_id);

                            //ustawienie graczowi wsp drzwi
                            if (temp_door != null)
                                Zawodnik.SetPoz(temp_door.GetX(), temp_door.GetY());
                        }
                        else komunikat = "Zamkniete( ID:" + temp_przejscie.GetID() + " )";
                    }

                    if (main_Podziemia.GetObecnna().is_WinDoor(Zawodnik.GetX(), Zawodnik.GetY()))
                    {
                        if (!main_Podziemia.GetObecnna().GetWinDoor().is_lock())
                        {
                            GAME_Stan = STANGRY.S_WON;
                        }
                        else komunikat = "Zamkniete";
                    }
                    break;

                /***************/
                /* PODNOSZENIE */
                /***************/
                case 'p':
                    if (main_Podziemia.GetObecnna().is_Skarb(Zawodnik.GetX(), Zawodnik.GetY()))
                    {
                        Skarb temp_skarb = main_Podziemia.GetObecnna().RemoveSkarb(Zawodnik.GetX(), Zawodnik.GetY());

                        if (temp_skarb != null) Zawodnik.AddPunkty(temp_skarb.GetWartosc());
                    }

                    if (main_Podziemia.GetObecnna().is_key(Zawodnik.GetX(), Zawodnik.GetY()))
                    {
                        Klucz temp_key = main_Podziemia.GetObecnna().RemoveKey(Zawodnik.GetX(), Zawodnik.GetY());

                        if (temp_key != null)
                        {
                            Zawodnik.AddKey(temp_key);
                            komunikat = "Klucz( ID:" + temp_key.GetID() + " )";
                        }
                    }

                    if (main_Podziemia.GetObecnna().is_KeyWinDoor(Zawodnik.GetX(), Zawodnik.GetY()))
                    {
                        Klucz temp_key = main_Podziemia.GetObecnna().RemoveWinKey(Zawodnik.GetX(), Zawodnik.GetY());

                        if (temp_key != null)
                        {
                            Zawodnik.AddKey(temp_key);
                            komunikat = "! Klucz do wyjscia !";
                        }
                    }
                    break;

                /********************/
                /* OTWIERANIE DRZWI */
                /********************/
                case 'l':
                    if (main_Podziemia.GetObecnna().is_Przejscie(Zawodnik.GetX(), Zawodnik.GetY()))
                    {
                        Przejscie temp_przejscie = main_Podziemia.GetObecnna().GetDoor(Zawodnik.GetX(), Zawodnik.GetY());

                        if (temp_przejscie.is_lock())
                        {
                            Klucz temp_key = Zawodnik.UseKey(temp_przejscie.GetID());

                            if (temp_key == null)
                                komunikat = "Brak klucza";
                            else
                            {
                                temp_przejscie.Open(temp_key);
                                komunikat = "Otworzono";
                            }
                        }
                        else komunikat = "Otwarte";
                    }

                    if (main_Podziemia.GetObecnna().is_WinDoor(Zawodnik.GetX(), Zawodnik.GetY()))
                    {
                        Przejscie temp_przejscie = main_Podziemia.GetObecnna().GetWinDoor();

                        if (temp_przejscie.is_lock())
                        {
                            Klucz temp_key = Zawodnik.UseKey(temp_przejscie.GetID());

                            if (temp_key == null)
                                komunikat = "Brak klucza";
                            else
                            {
                                temp_przejscie.Open(temp_key);
                                komunikat = "Otworzono";
                            }
                        }
                        else komunikat = "Otwarte";
                    }
                    break;
            }
        }
        
        public Kontroler()
        {
            
            main_Podziemia = new Podziemia();
            Zawodnik = new Gracz(main_Podziemia.GetObecnna());
            
            GAME_Stan = STANGRY.S_NEW;
            komunikat = null ;

            char wejscie = '\0';


            //ConsoleKeyInfo cki = new ConsoleKeyInfo();

            do
            {
                //Console.WriteLine("\nPress a key to display; press the 'x' key to quit.");
                Widok.Show(main_Podziemia.GetObecnna(), Zawodnik);

                if (komunikat != null)
                {
                    Widok.Komunikat(komunikat);
                    komunikat = null;
                }               

                while (Console.KeyAvailable == false)
                {
                    Thread.Sleep(250);
                    Widok.Show(main_Podziemia.GetObecnna(), Zawodnik);
                    
                    MoveTief();
                    
                    main_Podziemia.GetObecnna().is_thief(Zawodnik);
                    if (Zawodnik.GetHP() <= 0)
                    {
                        GAME_Stan = STANGRY.S_GAMEOVER;
                        break;
                    }
                  
                }

                if (GAME_Stan == STANGRY.S_NEW)
                {
                    wejscie = Console.ReadKey().KeyChar;
                    MovePlayer(wejscie);
                    main_Podziemia.GetObecnna().is_thief(Zawodnik);

                    if (Zawodnik.GetHP() <= 0) GAME_Stan = STANGRY.S_GAMEOVER;
                }
            } while (wejscie != (char)(27) && GAME_Stan == STANGRY.S_NEW);

            /*do stara wersja ruch zlodzieji po ruchu gracza
            {
                Widok.Show(main_Podziemia.GetObecnna(), Zawodnik);
                
                if (komunikat != null)
                {
                    Widok.Komunikat(komunikat);
                    komunikat = null;
                }
                
                wejscie = Console.ReadKey().KeyChar;

                MovePlayer(wejscie);

                main_Podziemia.GetObecnna().is_thief(Zawodnik);

                //ruszenie zlodzieji temp
                MoveTief();
                //do POPRAWIENIA
                                
                if (Zawodnik.GetHP() <= 0) GAME_Stan = STANGRY.S_GAMEOVER;
            
            } while (wejscie != (char)(27) && GAME_Stan == STANGRY.S_NEW);
            */

            switch (GAME_Stan)
            {
                case STANGRY.S_WON:
                    do
                    {
                        Widok.Show(main_Podziemia.GetObecnna(), Zawodnik);
                        Widok.Wygrana();
                    } while (Console.ReadKey().KeyChar != 'q');
                    break;

                case STANGRY.S_GAMEOVER:
                    do
                    {
                        Widok.Show(main_Podziemia.GetObecnna(), Zawodnik);
                        Widok.Przegrana();
                    } while (Console.ReadKey().KeyChar != 'q');
                    break;
            }
        }
    }
    
    

}

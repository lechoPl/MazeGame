using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labirynt_alfa
{
    class Widok
    {
        public static void Show(Komnata widoczna_komnata, Gracz Player = null)
        {
            Console.Clear();

            //Splaszcznie planszy i elementow znajdujacych sie na planszy
            int[,] field_temp = new int[widoczna_komnata.GetSizeX(), widoczna_komnata.GetSizeY()];

            for (int y = 0; y < widoczna_komnata.GetSizeY(); y++)
            {
                for (int x = 0; x < widoczna_komnata.GetSizeX(); x++)
                {
                    if (widoczna_komnata.GetFiled()[x, y] == FIELD.F_WOLNE)
                        field_temp[x, y] = 0;
                    else
                        field_temp[x, y] = 1;
                }
            }

            if (widoczna_komnata.GetDoors() != null)
            {
                for (int i = 0; i < widoczna_komnata.GetDoors().Length; i++)
                {
                    field_temp[widoczna_komnata.GetDoors()[i].GetX(), widoczna_komnata.GetDoors()[i].GetY()] = 2;
                }
            }

            if (widoczna_komnata.GetSkarby() != null)
            {
                for (int i = 0; i < widoczna_komnata.GetSkarby().Length; i++)
                {
                    if (widoczna_komnata.GetSkarby()[i] != null) field_temp[widoczna_komnata.GetSkarby()[i].GetX(), widoczna_komnata.GetSkarby()[i].GetY()] = 3;
                }
            }

            if (widoczna_komnata.GetThief() != null)
            {
                for (int i = 0; i < widoczna_komnata.GetThief().Length; i++)
                {
                    if (widoczna_komnata.GetThief()[i] != null) field_temp[widoczna_komnata.GetThief()[i].GetX(), widoczna_komnata.GetThief()[i].GetY()] = 4;
                }
            }

            if (widoczna_komnata.GetKlucze() != null)
            {
                for (int i = 0; i < widoczna_komnata.GetKlucze().Length; i++)
                {
                    if (widoczna_komnata.GetKlucze()[i] != null) field_temp[widoczna_komnata.GetKlucze()[i].GetX(), widoczna_komnata.GetKlucze()[i].GetY()] = 5;
                }
            }

            if (widoczna_komnata.GetWinDoor() != null)
            {
                field_temp[widoczna_komnata.GetWinDoor().GetX(), widoczna_komnata.GetWinDoor().GetY()] = 6;
            }

            if (widoczna_komnata.GetWinKey() != null)
            {
                field_temp[widoczna_komnata.GetWinKey().GetX(), widoczna_komnata.GetWinKey().GetY()] = 7;
            }

            //Rysowanie wlasciwej planszy
            for (int y = 0; y < widoczna_komnata.GetSizeY(); y++)
            {
                for (int x = 0; x < widoczna_komnata.GetSizeX(); x++)
                {
                    if (Player != null && Player.GetX() == x && Player.GetY() == y)
                    {
                        Console.Write((char)(1));
                        continue;
                    }

                    switch (field_temp[x, y])
                    {
                        case 0: Console.Write(' ');
                            break;
                        case 1: Console.Write((char)(4));
                            break;
                        case 2: Console.Write('D');
                            break;
                        case 3: Console.Write('$');
                            break;
                        case 4: Console.Write((char)(2));
                            break;
                        case 5: Console.Write('K');
                            break;
                        case 6: Console.Write('H');
                            break;
                        case 7: Console.Write('I');
                            break;
                    }
                }
                Console.Write("\n");
            }
            //Console.Write("\n");
            Console.SetCursorPosition(60, 2);
            Console.Write("HP: {0,4}", Player.GetHP());
            Console.SetCursorPosition(60, 3);
            Console.Write("Pkt: {0,3}", Player.GetPunkty());
            Console.SetCursorPosition(60, 4);
            Console.Write("Key: {0,3}", Player.GetLiczbaKluczy());
            //------------------------------------------------------------------------------------------------------------------------------------
        }

        public static void Przegrana()
        {
            Console.SetCursorPosition(15, 19);
            Console.WriteLine("==========================");
            Console.SetCursorPosition(15, 20);
            Console.WriteLine("||   ! GAME OVER !      ||");
            Console.SetCursorPosition(15, 21);
            Console.WriteLine("==========================");

            Console.SetCursorPosition(15, 23);
            Console.WriteLine("  nacisnij 'q' aby wyjsc");
        }

        public static void Wygrana()
        {
            Console.SetCursorPosition(15, 18);
            Console.WriteLine("==========================");
            Console.SetCursorPosition(15, 19);
            Console.WriteLine("||   !   Wygrana !      ||");
            Console.SetCursorPosition(15, 20);
            Console.WriteLine("||   ! GRATULACJE !     ||");
            Console.SetCursorPosition(15, 21);
            Console.WriteLine("==========================");

            Console.SetCursorPosition(15, 23);
            Console.WriteLine("  nacisnij 'q' aby wyjsc");
        }

        public static void Komunikat(string str)
        {
            Console.SetCursorPosition(60, 19);
            Console.WriteLine("Komunikat:");
            Console.SetCursorPosition(60, 20);
            Console.WriteLine(str);
        }
    }
}

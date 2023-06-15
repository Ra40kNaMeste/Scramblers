// ScramblerDLLConsoleApp.cpp : Этот файл содержит функцию "main". Здесь начинается и заканчивается выполнение программы.
//

#include <iostream>
#include "ScramblerDLL.h"
#include <string.h>

int main()
{
    std::cout << "Hello World!\n";
    //Initialize a Fibonacci relation sequence.


    std::string str = "Hello world! My name is Hutler!!";

    Two_Fish fish = Two_Fish();
    
    fish.set_size_key(256);

    char key[32];
    for (int i = 0; i < 32; i++)
    {
        std::cout << key[i];
    }
    fish.set_fish_key(key);

    std::cout << "\nCode:\n";

    char* res = fish.Encode(str.c_str(), 0);
    for (int i = 0; i < 32; i++)
    {
        std::cout << res[i];
    }
    std::cout << "\nDecode:\n";

    char* res2 = fish.Decode(res, 0);
    for (int i = 0; i < 32; i++)
    {
        std::cout << res2[i];
    }
    delete[] res;
    delete[] res2;
    fish.dispose();
}

// Запуск программы: CTRL+F5 или меню "Отладка" > "Запуск без отладки"
// Отладка программы: F5 или меню "Отладка" > "Запустить отладку"

// Советы по началу работы 
//   1. В окне обозревателя решений можно добавлять файлы и управлять ими.
//   2. В окне Team Explorer можно подключиться к системе управления версиями.
//   3. В окне "Выходные данные" можно просматривать выходные данные сборки и другие сообщения.
//   4. В окне "Список ошибок" можно просматривать ошибки.
//   5. Последовательно выберите пункты меню "Проект" > "Добавить новый элемент", чтобы создать файлы кода, или "Проект" > "Добавить существующий элемент", чтобы добавить в проект существующие файлы кода.
//   6. Чтобы снова открыть этот проект позже, выберите пункты меню "Файл" > "Открыть" > "Проект" и выберите SLN-файл.

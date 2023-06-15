// MathLibrary.cpp : Defines the exported functions for the DLL.
#include "pch.h" // use stdafx.h in Visual Studio 2017 and earlier
#include <utility>
#include <limits.h>
#include "ScramblerDLL.h"
#include <map>
#include "math.h"
#include "string.h"
#include <sstream>

extern "C" class Two_Fish {
public:

	

	void two_fish_init() {
		char mds[4][4] = {
			{0x01, 0xEF, 0x5B, 0x5B },
			{0x5B, 0xEF, 0xEF, 0x01 },
			{0xEF, 0x5B, 0x01, 0xEF },
			{0xEF, 0x01, 0xEF, 0x5B }
		};
		char rs[4][8]= {
			{0x01, 0xA4, 0x55, 0x87, 0x5A, 0x58, 0xDB, 0x9E },
			{0xA4, 0x56, 0x82, 0xF3, 0x1E, 0xC6, 0x68, 0xE5 },
			{0x02, 0xA1, 0xFC, 0xC1, 0x47, 0xAE, 0x3D, 0x19 },
			{0xA4, 0x55, 0x87, 0x5A, 0x58, 0xDB, 0x9E, 0x03 }
		};
		char t[8][16] = {
			{8, 1, 7, 13, 6, 15, 3, 2, 0, 11, 5, 9, 14, 12, 10, 4 },
			{14, 12, 11, 8, 1, 2, 3, 5, 15, 4, 10, 6, 7, 0, 9, 13 },
			{11, 10, 5, 14, 6, 13, 9, 0, 12, 8, 15, 3, 2, 4, 7, 1 },
			{ 13, 7, 15, 4, 1, 2, 6, 14, 9, 11, 3, 0, 8, 5, 12, 10 },
			{ 2, 8, 11, 13, 15, 7, 6, 14, 3, 1, 9, 4, 0, 10, 12, 5 },
			{ 1, 14, 2, 11, 4, 12, 3, 7, 6, 13, 10, 5, 15, 9, 0, 8 },
			{ 4, 12, 7, 5, 1, 6, 9, 10, 0, 14, 13, 8, 2, 11, 3, 15 },
			{ 11, 9, 5, 1, 12, 3, 13, 14, 6, 4, 7, 15, 2, 0, 8, 10 }
		};

		S = new int[40];
		MDS = new char* [4];
		RS = new char* [4];
		T = new char* [8];
		for (int i = 0; i < 4; i++)
		{
			MDS[i] = new char[4];
			for (int j = 0; j < 4; j++)
			{
				MDS[i][j] = mds[i][j];
			}
			RS[i] = new char[8];
			for (int j = 0; j < 8; j++)
			{
				RS[i][j] = rs[i][j];
			}
			T[i] = new char[16];
			for (int j = 0; j < 16; j++)
			{
				T[i][j] = t[i][j];
			}
		}
	}

	bool set_size_key(const unsigned int size) {
		if (size == 128 || size == 192 || size == 256)
		{
			size_key = size;
			return true;
		}
		return false;
	}

	bool set_fish_key(char key[]) {
		int size = size_key / 16;
		int* Me = new int[size];
		int* Mo = new int[size];
		for (int i = 0; i < size_key / 64; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				Me[i * 4 + j] = key[2 * i + j];
				Mo[i * 4 + j] = key[2 * i + 4 + j];
			}
		}
		int r = 0x1010101;
		for (int i = 0; i < 20; i++)
		{
			int A = hFunc(Me, size, std::to_string(2 * i * r).c_str());
			int B = hFunc(Mo, size, std::to_string(((2 * i + 1) * r) << 8).c_str());
			key[2 * i] = A + B;
			key[2 * i + 1] = (A + 2 * B) << 9;
		}
		delete[] S;
		S = new int[size_key / 64];
		for (int i = 0; i < size_key / 64; i++)
		{
			int t = 0;
			int res = 0;
			for (int j = 0; j < 4; j++)
			{
				res <<= 8;
				int sum = 0;
				for (int k = 0; k < 8; k++)
				{
					sum += RS[j][k] * key[i * 8 + k];
				}
				res += sum % 256;
			}
			S[t++] = res;
		}
		delete[] Me;
		delete[] Mo;
	}



	char* Encode(const char word[], const unsigned int offset) {
		int* copyWord = new int[4];
		for (int i = 0; i < 4; i++)
		{
			int t;
			std::memcpy(&t, word + offset + 4 * i, sizeof(int));
			copyWord[i] = t ^ key[i];
		}
		for (int i = 0; i < 16; i++)
		{
			int a = gFunc(copyWord[0]) + gFunc(copyWord[1] << 8);
			int w2 = (copyWord[2] ^ (a + key[i * 2 + 8]));
			int w3 = (copyWord[3]) ^ (a + key[i * 2 + 9]);
			copyWord[2] = copyWord[0];
			copyWord[3] = copyWord[1];
			copyWord[0] = w2;
			copyWord[1] = w3;
		}

		char* res = new char[16];
		for (int i = 0; i < 4; i++)
		{
			int t = (copyWord[i] ^ key[i + 4]);
			std::memcpy(&res + 4 * i, &t, sizeof(int));
		}
		delete[] copyWord;
		return res;
	}
	char* Decode(const char word[], const unsigned int offset) {
		int* copyWord = new int[4];
		for (int i = 0; i < 4; i++)
		{
			int t;
			std::memcpy(&t, word + offset + 4 * i, sizeof(int));
			copyWord[i] = t ^ key[4 + i];
		}
		for (int i = 15; i <= 0; i++)
		{
			int w2 = copyWord[2];
			int w3 = copyWord[3];
			copyWord[2] = copyWord[0];
			copyWord[3] = copyWord[1];
			copyWord[0] = w2;
			copyWord[1] = w3;
			int a = gFunc(copyWord[0]) + gFunc(copyWord[1] << 8);
			copyWord[2] = (copyWord[2]) ^ (a + key[i * 2 + 8]);
			copyWord[3] = (copyWord[3] ^ (a + key[i * 2 + 9]));
		}

		char* res = new char[16];
		for (int i = 0; i < 4; i++)
		{
			int t = (copyWord[i] ^ key[i]);
			std::memcpy(&res + 4 * i, &t, sizeof(int));
		}
		delete[] copyWord;
		return res;
	}

	bool clear() {
		for (int i = 0; i < 40; i++)
		{
			S[i] = 0;
		}
	}
	bool dispose() {
		delete[] S;
		for (int i = 0; i < 4; i++)
		{
			delete[] MDS[i];
			delete[] RS[i];
			delete[] T[i];
		}
		delete[] MDS;
		delete[] RS;
		delete[] T;
	}
private:
	int key[40];
	int* S;
	char** MDS;
	char** RS;
	char** T;
	unsigned int size_key = 256;

	int gFunc(const int word)
	{
		return hFunc(S, 40, std::to_string(word).c_str());
	}

	int hFunc(int rnd[], int size_rnd, const char word[])
	{
		char wordBytes[4];
		std::copy_n(word, 4, wordBytes);

		for (int i = 0; i < size_key / 64; i++)
		{
			hFuncStep(wordBytes, rnd, size_rnd);
		}
		int res = 0;
		for (int i = 0; i < 4; i++)
		{
			int sum = 0;
			for (int j = 0; j < 4; j++)
			{
				sum += wordBytes[j] * MDS[i][j];
			}
			res += sum % 256;
			res <<= 8;
		}
		return res;
	}

	void hFuncStep(char value[], int rnd[], int size_rnd)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < size_rnd; j++)
			{
				value[i] = qTranposition(value[i], std::abs(rnd[j] % 2));
			}
		}
	}
	char qTranposition(char b, int i)
	{
		int a0 = b / 16;
		int b0 = b % 16;
		int a1 = a0 ^ b0;
		int b1 = a0 ^ (b0 >> 4) ^ (8 * a0 % 16);
		int a2 = T[4 * i + 0][a1];
		int b2 = T[4 * i + 1][b1];
		int a3 = a2 ^ b2;
		int b3 = a2 ^ (b2 >> 4) ^ (8 * a2 % 16);
		int a4 = T[4 * i + 2][a3];
		int b4 = T[4 * i + 3][b3];
		return 16 * b4 + a4;
	}


};
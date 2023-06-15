#pragma once

#ifdef  SCRAMBLERDLL_EXPORTS
#define SCRAMBLERDLL_API __declspec(dllexport)
#else
#define SCRAMBLERDLL_API __declspec(dllimport)
#endif //  SCRAMBLERDLL_EXPROTS

extern "C" class SCRAMBLERDLL_API Two_Fish {
public:
	void two_fish_init();

	bool set_size_key(const unsigned int size);
	bool set_fish_key(char key[]);

	char* Encode(const char word[], const unsigned int offset);
	char* Decode(const char word[], const unsigned int offset);

	bool clear();
	bool dispose();

};


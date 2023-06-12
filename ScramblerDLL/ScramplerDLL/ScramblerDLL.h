#pragma once

#ifdef  SCRAMBLERDLL_EXPORTS
#define SCRAMBLERDLL_API __declspec(dllexport)
#else
#define SCRAMBLERDLL_API __declspec(dllimport)
#endif //  SCRAMBLERDLL_EXPROTS

extern "C" SCRAMBLERDLL_API void fibonacci_init(
    const unsigned long long a, const unsigned long long b);


// Produce the next value in the sequence.
// Returns true on success and updates current value and index;
// false on overflow, leaves current value and index unchanged.
extern "C" SCRAMBLERDLL_API bool fibonacci_next();

// Get the current value in the sequence.
extern "C" SCRAMBLERDLL_API unsigned long long fibonacci_current();

// Get the position of the current value in the sequence.
extern "C" SCRAMBLERDLL_API unsigned fibonacci_index();
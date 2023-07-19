using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Model
{

    public class PasswordGenerator
    {
        public PasswordGenerator()
        {
            FillGenerators();
        }

        private void FillGenerators()
        {
            Generators = new List<PasswordGeneratorItem>()
            {
                new PasswordGeneratorItem(new CapitalChars()),
                new PasswordGeneratorItem(new UppercaseChars()),
                new PasswordGeneratorItem(new NumberChars()),
                new PasswordGeneratorItem(new SpecialSymbols())
            };
        }
        public IEnumerable<PasswordGeneratorItem> Generators { get; private set; }
        public int Size { get; set; }
        public string GenerateString()
        {
            var useGenerators = Generators.Where(i => i.IsEnable);
            int countG = useGenerators.Count();
            if (countG == 0)
                throw new Exception();
            var rbytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(Size * 2);
            var passwordBuffer = new char[Size];
            for (int i = 0; i < Size; i++)
            {
                var generator = useGenerators.ElementAt(rbytes[2 * i] % countG).CharRange;
                char? ch = generator.GetChar(rbytes[2 * i + 1] % generator.Count);
                if (ch == null)
                    throw new Exception();
                passwordBuffer[i] = ch.Value;
            }
            return new string(passwordBuffer);
        }
    }

    public class PasswordGeneratorItem
    {
        public PasswordGeneratorItem() 
        {
            IsEnable = true;
        }
        public PasswordGeneratorItem(ICharRange charRange):this()
        {
            CharRange = charRange;
        }
        public bool IsEnable { get; set; }
        public ICharRange CharRange { get; set; }
    }

    public interface ICharRange
    {
        public int Count { get; }
        public char? GetChar(int index);
    }
    public class CharRangeBase : ICharRange
    {
        public CharRangeBase(char start, char end)
        {
            _start = start;
            _end = end;
        }
        private char _start;
        private char _end;
        public int Count => _end - _start;

        public char? GetChar(int index)
        {
            if (index < 0 || index >= Count)
                return null;
            return (char)(_start + index);
        }
    }

    public class CapitalChars : CharRangeBase
    {
        public CapitalChars() : base('A', 'Z') { }
    }
    public class UppercaseChars : CharRangeBase
    {
        public UppercaseChars() : base('a', 'z') { }
    }
    public class NumberChars : CharRangeBase
    {
        public NumberChars() : base('0', '9') { }
    }
    public class SpecialSymbols : ICharRange
    {

        private List<char> _chars = new() { '@', '$', '%', '*', ')', '?', '~' };

        public int Count => _chars.Count;

        public char? GetChar(int index)
        {
            if (index < 0 || index >= Count)
                return null;
            return _chars[index];
        }
    }
}

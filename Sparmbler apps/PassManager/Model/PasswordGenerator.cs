using PassManager.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Model
{
    public enum PasswordGenerateMode
    {
        None = 0,
        CapitalChars = 1,
        UppercaseChars = 2,
        NumberChars = 4,
        SpecialSymbol = 8,
        All
    }
    public class PasswordGenerator
    {
        public PasswordGenerator()
        {
            FillGenerators(PasswordGenerateMode.All);
        }

        public void FillGenerators(PasswordGenerateMode mode)
        {
            Generators = _generateItems.Where(i => (i.Key & mode) != 0).Select(i => i.Value);
        }
        private Dictionary<PasswordGenerateMode, PasswordGeneratorItem> _generateItems = new()
        {
             { PasswordGenerateMode.CapitalChars, new(new CapitalChars()) },
             { PasswordGenerateMode.UppercaseChars, new(new UppercaseChars()) },
             { PasswordGenerateMode.NumberChars, new(new NumberChars()) },
             { PasswordGenerateMode.SpecialSymbol, new(new SpecialSymbols()) },
        };

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

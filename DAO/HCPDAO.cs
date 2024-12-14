namespace BotoxInjectorSite.DAO
{
    public class HCPDAO
    {
        public bool IsValidSequence(string input)
        {
            // Extract numeric sequences from the input
            var numericParts = string.Concat(input.Where(char.IsDigit));

            // Check for recurring identical digits
            for (int i = 0; i <= numericParts.Length - 4; i++)
            {
                string segment = numericParts.Substring(i, 4);
                if (segment.Distinct().Count() == 1)
                    return false; // Recurring identical digits
            }

            // Check for increasing or decreasing sequences
            if (numericParts.Length >= 4 && (IsIncreasingSequence(numericParts) || IsDecreasingSequence(numericParts)))
                return false;

            // Check for recurring patterns
            for (int i = 1; i <= numericParts.Length / 2; i++)
            {
                string pattern = numericParts.Substring(0, i);
                string repeatedPattern = string.Concat(Enumerable.Repeat(pattern, numericParts.Length / i));
                if (numericParts.StartsWith(repeatedPattern))
                    return false; // Recurring pattern found
            }

            return true;
        }

        private bool IsIncreasingSequence(string input)
        {
            for (int i = 0; i <= input.Length - 4; i++)
            {
                bool isIncreasing = true;
                for (int j = i; j < i + 3; j++)
                {
                    if (input[j + 1] != input[j] + 1)
                    {
                        isIncreasing = false;
                        break;
                    }
                }
                if (isIncreasing)
                    return true;
            }
            return false;
        }

        private bool IsDecreasingSequence(string input)
        {
            for (int i = 0; i <= input.Length - 4; i++)
            {
                bool isDecreasing = true;
                for (int j = i; j < i + 3; j++)
                {
                    if (input[j + 1] != input[j] - 1)
                    {
                        isDecreasing = false;
                        break;
                    }
                }
                if (isDecreasing)
                    return true;
            }
            return false;
        }
    }
}
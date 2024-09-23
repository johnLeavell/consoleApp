using System;

namespace sqltest.Helpers
{
    public static class InputHelper
    {
        public static bool CheckForSpecialCommands(string input)
        {
            if (input == null)
            {
                return false;
            }

            if (string.Equals(input, "menu", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
            {
                Environment.Exit(0);
            }
            return false;
        }
    }
}

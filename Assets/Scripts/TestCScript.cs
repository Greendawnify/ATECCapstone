using Boo.Lang.Runtime.DynamicDispatching;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class TestCScript
{
   
}

public class Username
{
    public static bool Validate(string username)
    {
        //throw new NotImplementedException("Waiting to be implemented.");
        if (username.Length < 6 || username.Length > 16) {
            return false;
        }

        char[] charArray = username.ToCharArray();
        if (!char.IsLetter(charArray[0])) {
            return false;
        }

        if (char.Equals('-', charArray[charArray.Length - 1])) {
            return false;
        }
        return true;
    }

    public static void Main(string[] args)
    {
        //Console.WriteLine(Validate("Mike-Standish")); // Valid username
        //Console.WriteLine(Validate("Mike Standish")); // Invalid username
    }
}

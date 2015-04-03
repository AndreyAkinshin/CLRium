using System;

namespace HelloWorld
{
    class Hello 
    {
        static void Main() 
        {
            Console.WriteLine("Hello World!");
            Foo();
            // Bar();
        }

        static int Foo()
        {
            int sum = 0;
            for (int i = 0; i < 5; i++)
              sum += i;
            return sum;
        }

        static int Bar()
        {
            int sum = 0;
            for (int i = 0; i < 10; i++)
              sum += i;
            return sum;
        }
    }
}
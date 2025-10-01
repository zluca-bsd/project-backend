using System;
Console.WriteLine("Hello, World!");

int myInt = 10;
string myString = "string";
bool myBool = true;

Console.WriteLine(myInt);
Console.WriteLine(myString);
Console.WriteLine(myBool);

Console.WriteLine("Insert a number:");
int input = Convert.ToInt32(Console.ReadLine());

if (input % 2 == 0)
{
    Console.WriteLine($"{input} is even");
}
else
{
    Console.WriteLine($"{input} is odd");
}
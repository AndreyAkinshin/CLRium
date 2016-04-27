<Query Kind="Statements" />

while (true)
{
	var start = DateTime.UtcNow;
	Thread.Sleep(1);
	var end = DateTime.UtcNow;
	Console.WriteLine(((end - start).Ticks / 10000.0) + " ms");
}
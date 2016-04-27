<Query Kind="Statements" />

for (int i = 0; i < 100000; i++)
{
	var current = DateTime.UtcNow;
	var last = current;
	while (last == current)
		current = DateTime.UtcNow;
	var diff = current - last;
	Console.WriteLine(diff.Ticks);
	last = current;	
	Thread.Sleep(1000);
}
﻿using Chapter19zNoCoherentAomSizeFix;

var app = new FirstApp();

try
{
	app.Run();

}
catch (Exception ex)
{
	Console.WriteLine($"Error!\n{ex.Message}");
	throw;
}



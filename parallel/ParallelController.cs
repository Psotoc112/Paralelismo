using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using parallel.Models;
namespace parallel
{
	public class ParallelController
	{

		
		private List<string> outputMessages { get; set; } = new List<string>();

	

		public List<string> ProcessFile(string[] args)
		{

            // -----------------------------------------------------------------------------------------------
            //	Mock args para testing

            //	string[] argsTest = new string[] { "-folder", "D:\ParallelTestData", "-s"};

            //	args = argsTest;

            // -----------------------------------------------------------------------------------------------



            // Estado del programa
            int status = -1; // -1 => Sin procesar, 0 => Exitoso, 1 => Error

			// Variable para calcular el tiempo de ejecucion del programa
			DateTime start = DateTime.Now;

			outputMessages.Add("------------------------------------------------");
			outputMessages.Add("Program Start time: " + start.ToString());
			outputMessages.Add("------------------------------------------------");
			// Tomar los argumentos ingresados
			var arguments = ReadArguments(args);



			// Verificacion de que la direccion del folder es correcta
			if (arguments.Path == null)
			{
				throw new ArgumentException("The path is not set");
			}
			else
			{
				if (!System.IO.Directory.Exists(arguments.Path))
				{
					throw new ArgumentException("The path is not a valid directory");
				}
			}

			if (arguments.ExecutionType == null)
			{
				throw new ArgumentException("The execution type is not set");
			}

			// Si ExecutionType = SINGLE
			if (arguments.ExecutionType == "SINGLE")
			{
				status = ProcessFileSingle(arguments.Path);
			}

            // Si ExecutionType = MULTIPLE
            if (arguments.ExecutionType == "MULTIPLE")
			{
				status = ProcessFileMultiple(arguments.Path);
			}
            // Si ExecutionType = SEQUENCIAL
            if (arguments.ExecutionType == "SEQUENCIAL")
			{
				status = ProcessFileSequencial(arguments.Path);
			}


			// Calcular el tiempo transcurrido
			DateTime end = DateTime.Now;
			TimeSpan elapsed = end - start;

			outputMessages.Add("------------------------------------------------");
			outputMessages.Add("Elapsed time: " + elapsed.TotalMilliseconds.ToString() + " milliseconds");
			outputMessages.Add("Status: " + status.ToString());
			outputMessages.Add("------------------------------------------------");

			
			return outputMessages;

			

		}

		private int ProcessFileMultiple(string path)
		{
			try
			{
				outputMessages.Add("------------------------------------------------");
				outputMessages.Add("Execution Type: MULTIPLE ");


				DateTime file0start = DateTime.Now;

				outputMessages.Add("First File Start: " + file0start.ToString());

				
				Parallel.ForEach(System.IO.Directory.GetFiles(path), file =>
				{
					DateTime loadStart = DateTime.Now;
					var FileContentList = new List<string>();
					foreach (string line in System.IO.File.ReadAllLines(file))
					{
						FileContentList.Add(line);

					}
					DateTime loadEnd = DateTime.Now;

					// Agregar al output la hora, el nombre del archivo y lo que se demoro en cargar
					outputMessages.Add(DateTime.Now.ToString() + " | File: " + System.IO.Path.GetFileName(file) + " | " + (loadEnd - loadStart).Milliseconds + " ms");

				});


				outputMessages.Add("Last File End: " + DateTime.Now.ToString());
				return 0; 

			}
			catch (Exception ex)
			{
				outputMessages.Add(ex.Message);
				return 1;
			}
		}

		private int ProcessFileSingle(string path)
		{
			try
			{
				outputMessages.Add("------------------------------------------------");
				outputMessages.Add("Execution Type: SINGLE ");


				DateTime file0start = DateTime.Now;

				outputMessages.Add("First File Start: " + file0start.ToString());

				// Afinidad para single core
				Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)1;

				Parallel.ForEach(System.IO.Directory.GetFiles(path),  file =>
				{
					DateTime loadStart = DateTime.Now;
					var FileContentList = new List<string>();
					foreach (string line in System.IO.File.ReadAllLines(file))
					{
						FileContentList.Add(line);

					}
					DateTime loadEnd = DateTime.Now;

                    // Agregar al output la hora, el nombre del archivo y lo que se demoro en cargar
                    outputMessages.Add(DateTime.Now.ToString() + " | File: " + System.IO.Path.GetFileName(file) + " | " + (loadEnd - loadStart).Milliseconds + " ms");

				});


				outputMessages.Add("Last File End: " + DateTime.Now.ToString());
				return 0; 

			}
			catch (Exception ex)
			{
				
				outputMessages.Add(ex.Message);
				return 1; 
			}

		}

		private int ProcessFileSequencial(string path)
		{
			try
			{


				outputMessages.Add("------------------------------------------------");
				outputMessages.Add("Execution Type: SEQUENCIAL ");


				DateTime file0start = DateTime.Now;

				outputMessages.Add("First File Start: " + file0start.ToString());

				
				Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)2;

				foreach (string file in System.IO.Directory.GetFiles(path))
				{

					DateTime loadStart = DateTime.Now;
					var FileContentList = new List<string>();
					foreach (string line in System.IO.File.ReadAllLines(file))
					{
						FileContentList.Add(line);

					}
					DateTime loadEnd = DateTime.Now;

                    // Agregar al output la hora, el nombre del archivo y lo que se demoro en cargar
                    outputMessages.Add(DateTime.Now.ToString() + " | File: " + System.IO.Path.GetFileName(file) + " | " + (loadEnd - loadStart).Milliseconds + " ms");


				}
				outputMessages.Add("Last File End: " + DateTime.Now.ToString());


				return 0; 
			}
			catch (Exception ex)
			{
				
				outputMessages.Add(ex.Message);
				return 1; 
			}


			
		}


		// Argumentos de ejecucion
		private Arguments ReadArguments(string[] args)
		{
			var arguments = new Arguments();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-folder")
				{
					// Verificar que "path" no este ya asignado
					if (arguments.Path != null)
					{
						throw new ArgumentException("The path can't be set twice");
					}
					else if (i + 1 < args.Length)
					{
						arguments.Path = args[i + 1];
					}					
				}

				if (args[i] == "-s")
				{
					// Verificar que "ExecutionType" no este ya asignado
					if (arguments.ExecutionType != null)
					{
						throw new ArgumentException("The execution type can't be set twice");
					}
					arguments.ExecutionType = "SINGLE";
				}

				if (args[i] == "-m")
				{
                    // Verificar que "ExecutionType" no este ya asignado
                    if (arguments.ExecutionType != null)
					{
						throw new ArgumentException("The execution type can't be set twice");
					}
					arguments.ExecutionType = "MULTIPLE";
				}
			}
			// Si "ExecutionType" no esta asignado se asigna "SEQUENCIAL"
			if (arguments.ExecutionType == null)
			{
				arguments.ExecutionType = "SEQUENCIAL";
			}

			return arguments;
		}




	}

}

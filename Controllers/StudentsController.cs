using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalExercise.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PracticalExercise.Controllers
{
    public class StudentsController : Controller
    {
        List<char> letters = new List<char> {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
            'L', 'M', 'N', 'Ñ', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

        [HttpGet]
        public IActionResult Index(List<Student> students = null)
        {
            students = students == null ? new List<Student>() : students;
            return View(students);
        }

        [HttpPost]
        public IActionResult Index(IFormFile file, [FromServices] IHostingEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using (FileStream fileStream=System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            var students = this.GetStudentList(file.FileName);
            return Index(students);
        }

        private List<Student> GetStudentList(string fName)
        {
            List<Student> students = new List<Student>();
            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName,FileMode.Open, FileAccess.Read))
            {
                try {
                    using (var reader=ExcelReaderFactory.CreateReader(stream))
                    {
                        //saltarse la primer línea (títulos)
                        reader.Read();

                        //Escribir títulos 
                        students.Add(new Student()

                        {
                            Nombres = "Nombre(s)",
                            ApellidoPaterno = "Apellido Paterno",
                            ApellidoMaterno = "Apellido Materno",
                            FechaNacimiento = "Fecha de nacimiento",
                            Grado = "Grado",
                            Grupo = "Grupo",
                            Calificacion = "Calificación",
                            Clave = "Clave"
                        });

                        //Llenar campos con datos del excel
                        while (reader.Read())
                        {
                            var nombres = reader.GetValue(0).ToString();
                            var apaterno = reader.GetValue(2).ToString();
                            var amaterno = reader.GetValue(1).ToString();
                            var fnacimiento = Convert.ToDateTime(reader.GetValue(3).ToString());
                            var grado = reader.GetValue(4).ToString();
                            var grupo = reader.GetValue(5).ToString();
                            var calificacion = reader.GetValue(6).ToString();

                            var clave = CreateID(nombres, amaterno, fnacimiento);

                            students.Add(new Student()
                            {
                                Nombres = nombres,
                                ApellidoPaterno = apaterno,
                                ApellidoMaterno = amaterno,
                                FechaNacimiento = fnacimiento.ToString("dd/MM/yyyy"),
                                Grado = grado,
                                Grupo = grupo,
                                Calificacion = calificacion,
                                Clave = clave

                            });
                        }
                    }
                }
                catch
                {
                    return null;
                }
                
            }
            return students;
        }

        public String CreateID(String nombre, String apmat, DateTime fnacim)
        {
            var clave1 = nombre.Substring(0, 2).ToUpper();
            var mclave1 = MoveLetters(clave1);
            var clave2 = apmat.Substring(apmat.Length - 2, 2).ToUpper();
            var mclave2 = MoveLetters(clave2);

            var fechaactual = DateTime.Today;

            var edad = CalculateAge(fnacim, fechaactual);



            return mclave1.Insert(2, mclave2).Insert(4, edad.ToString()); //.Insert(1, edad.ToString()));
        }

        private String MoveLetters(string iclave)
        {
            //agarrar la iclave y separarla en caracteres

            var xclave = "";
            var i = 0;

            foreach (char c in iclave)
            {
                xclave += EncodeLetters(c).ToString();
                i++;
            }
            //crear método para mover las letras 2 indices a la izq usando un arreglo de letras
            //mandar cada letra de la iclave al método para mover las letras
            //volver a juntar los caracteres en un string
            //retornar el string

            var fclave = xclave;

            return fclave;
        }

        public char EncodeLetters(char letter)
        {
            var xindex = letters.IndexOf(letter);
            var xletter = '\0';

            if(xindex >= 3)
            {
                xletter = letters[xindex - 3];
            }
            else
            {
                xletter = letters[^(3-xindex)];
            }
            
            return xletter;
        }

        public int CalculateAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;

            // For leap years we need this
            if (birthDate > now.AddYears(-age))
                age--;

            return age;
        }

    }

}

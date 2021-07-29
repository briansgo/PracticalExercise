using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalExercise.Models;
using System.Collections.Generic;
using System.IO;

namespace PracticalExercise.Controllers
{
    public class StudentsController : Controller
    {
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
                using (var reader=ExcelReaderFactory.CreateReader(stream))
                {
                    while(reader.Read())
                    {
                        students.Add(new Student()
                        {
                            Nombres = reader.GetValue(0).ToString(),
                            ApellidoPaterno = reader.GetValue(1).ToString(),
                            ApellidoMaterno = reader.GetValue(2).ToString(),
                            FechaNacimiento = reader.GetValue(3).ToString(),
                            Grado = reader.GetValue(4).ToString(),
                            Grupo = reader.GetValue(5).ToString(),
                            Calificacion = reader.GetValue(6).ToString(),
                            Clave = reader.GetValue(0).ToString().Substring(0,2)

                        });
                    }
                }

            }
            return students;
        }
    }

}

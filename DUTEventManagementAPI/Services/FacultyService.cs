using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services.Interfaces;

namespace DUTEventManagementAPI.Services
{
    public class FacultyService : IFacultyService
    {
        private readonly AppDbContext _context;
        public FacultyService(AppDbContext context)
        {
            _context = context;
        }
        public List<Faculty> GetAllFaculties()
        {
            return _context.Faculties.ToList();
        }
        public Faculty? GetFacultyById(string facultyId)
        {
            var faculty = _context.Faculties.Find(facultyId);
            if (faculty == null)
            {
                throw new Exception("Faculty not found with id " + facultyId);
            }
            return faculty;
        }
    }
}

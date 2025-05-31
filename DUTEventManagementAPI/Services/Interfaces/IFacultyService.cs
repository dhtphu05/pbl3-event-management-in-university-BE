using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface IFacultyService
    {
        List<Faculty> GetAllFaculties();
        Faculty? GetFacultyById(string facultyId);
    }
}
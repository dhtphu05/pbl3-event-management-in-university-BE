using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DUTEventManagementAPI.Services.Interfaces;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultiesController : ControllerBase
    {
        private readonly IFacultyService _facultyService;
        public FacultiesController(IFacultyService facultyService)
        {
            _facultyService = facultyService;
        }

        [HttpGet]
        public IActionResult GetAllFaculties()
        {
            try
            {
                var faculties = _facultyService.GetAllFaculties();
                return Ok(faculties);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{facultyId}")]
        public IActionResult GetFacultyById(string facultyId)
        {
            try
            {
                var faculty = _facultyService.GetFacultyById(facultyId);
                if (faculty == null)
                {
                    return NotFound($"Faculty with id {facultyId} not found.");
                }
                return Ok(faculty);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

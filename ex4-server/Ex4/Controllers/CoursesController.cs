﻿
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.BL;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        Course course = new Course();

        // GET: api/<CoursesController>
        [HttpGet]
        public IEnumerable<Course> Get()
        {
            return course.Read();
        }


        // POST api/<CoursesController>
        [HttpPost("NewCourse")]
        public IActionResult PostNewCourse([FromBody] Course value)
        {
            bool result = value.InsertNewCourse();
            if (!result)
            {
                return NotFound(new { message = "Course could not be inserted." });
            }
            return Ok(new { message = "Course inserted successfully." });
        }

        // POST api/<CoursesController>/5
        [HttpPost("addCourseToUser/{userId}")]
        public IActionResult AddCourseToUser(int userId, [FromBody] Course course)
        {
            if (Course.AddCourseToUser(userId, course))
            {
                return Ok(new { message = "Course added to user successfully" });
            }
            return NotFound(new { message = "Failed to add course to user" });
        }

        // PUT api/<CoursesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Course updatedCourse)
        {
            try
            {
                bool result = course.updateCourse(id, updatedCourse);
                if (result) { return Ok(new { message = "Course updated." }); }
                else { return NotFound(new { message = "Course could not be updated." }); }
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error with the exception message
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("searchByDurationForUser/{userId}")]
        public IActionResult GetByDurationRangeForUser(int userId, [FromQuery] double fromDuration, [FromQuery] double toDuration)
        {
            try
            {
                List<Course> courses = Course.GetByDurationRangeForUser(userId, fromDuration, toDuration);
                if (courses == null)
                {
                    return NotFound(new { message = "No courses found for the specified duration range and user." });
                }
                else
                {
                    return Ok(courses);
                }
            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No courses found for the specified duration range and user." });
            }
        }
        [HttpGet("searchByRatingForUser/{userId}")]
        public IActionResult GetByRatingRangeForUser(int userId, [FromQuery] double fromRating, [FromQuery] double toRating)
        {
            try
            {
                List<Course> courses = Course.GetByRatingRangeForUser(userId, fromRating, toRating);
                if (courses == null)
                {
                    return NotFound(new { message = "No courses found for the specified duration range and user." });
                }
                else
                {
                    return Ok(courses);
                }
            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No courses found for the specified duration range and user." });
            }
        }

        // Get courses by instructor id
        [HttpGet("searchByInstructorId/{instructorId}")]
        public IActionResult GetByInstructorId(int instructorId)
        {
            try
            {
                List<Course> courses = Course.GetCoursesByInstructor(instructorId);

                if (courses.Any())
                {
                    return Ok(courses);
                }
                return NotFound(new { message = "No courses found for this instructor." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Get courses by user id
        [HttpGet("UserCourse/{userId}")]
        public IActionResult GetByUserId(int userId)
        {
            try
            {
                List<Course> courses = Course.GetCoursesByUser(userId);

                if (courses.Any())
                {
                    return Ok(courses);
                }
                return NotFound(new { message = "No courses found for this instructor." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //Get courses by top 5 most user
       [HttpGet("Top5Courses")]
        public IActionResult GetTopFiveCourses()
        {
            try
            {
                List<Object> courses = Course.GetTopFiveCourses();

                if (courses.Any())
                {
                    return Ok(courses);
                }
                return NotFound(new { message = "No courses found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("deleteByCourseFromUserList/{userId}")]
        public IActionResult DeleteByCourseFromUserList(int userId, [FromQuery] int coursid)
        {
            try
            {
                if (Course.DeleteCourseFromUser(userId, coursid))
                {
                    return Ok(new { message = "Course deleted from user successfully" });
                }
                return BadRequest(new { message = "Failed to delete course from user list" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //probably dont need
        [HttpDelete("deleteCourse/{courseId}")]
        public IActionResult DeleteCourse(int coursid, [FromQuery] int coursId)
        {
            try
            {
                List<Course> courses =course.DeleteCourse(coursId);

                if (courses.Any())
                {
                    return Ok(courses);
                }
                return NotFound(new { message = "Course not Exist" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // PUT api/<ChangeActiveStatus>
        [HttpPut("ChangeActiveStatus/{id}")]
        public IActionResult ChangeActiveStatus(int id)
        {
            try
            {
                bool result = course.changeActiveStatus(id);
                if (result) { return Ok(new { message = "Status updated." }); }
                else { return NotFound(new { message = "Status could not be updated." }); }
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error with the exception message
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<ChangeActiveStatus>
        [HttpPut("ChangeCourseTitle/{id}/{title}")]
        public IActionResult ChangeCourseTitle(int id, string title)
        {
            try
            {
                bool result = course.changeCourseTitle(id, title);
                if (result) { return Ok(new { message = "Status updated." }); }
                else { return NotFound(new { message = "Status could not be updated." }); }
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error with the exception message
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<uploadFiles>
        [HttpPost("uploadFiles")]
        public async Task<IActionResult> Post([FromForm] List<IFormFile> files)
        {


            List<string> imageLinks = new List<string>();

            string path = System.IO.Directory.GetCurrentDirectory();

            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(path, "uploadedFiles/" + formFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    imageLinks.Add(formFile.FileName);
                }
            }

            // Return status code  
            return Ok(imageLinks);
        }

    }
}

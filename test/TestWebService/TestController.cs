using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestWebService
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TestController : Controller
    {
        [HttpPost("one")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public IActionResult One([FromBody] RequestModel parameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(new ResponseModel
            {
                NonNullable = parameter.NonNullable,
                Nullable = parameter.Nullable,
                SomeDouble = parameter.SomeDouble,
                SomeInt = parameter.SomeInt,
                SomeString = parameter.SomeString
            });
        }

    }

    [BindRequired]
    public class RequestModel
    {
        [Required]
        public Enum NonNullable { get; set; }

        [Required]
        public Enum? Nullable { get; set; }

        [Required]
        public string SomeString { get; set; }

        [Required]
        public int SomeInt { get; set; }

        [Required]
        public double SomeDouble { get; set; }


    }

    public class ResponseModel
    {
        [Required]
        public Enum NonNullable { get; set; }

        [Required]
        public Enum? Nullable { get; set; }

        [Required]
        public string SomeString { get; set; }

        [Required]
        public int SomeInt { get; set; }

        [Required]
        public double SomeDouble { get; set; }


    }
    public enum Enum
    {
        One,
        Two,
        Three
    }
}
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.ModelDto;
using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/Villa")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        private readonly ILogger<VillaApiController> _logger;

        public VillaApiController(ILogger<VillaApiController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("getting all villas");
            return VillaStore.villaList;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //Same as public ActionResult<VillaDto> GetVilla(int id)
        //[ProducesResponseType(200, Type =typeof(VillaDto))]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpGet("id", Name ="GetVilla")]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if (id==0)
            {
                _logger.LogInformation("error occured");
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(a=>a.Id ==id);
            if (villa==null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="villaDto"></param>
        /// <returns></returns>
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpPost("add")]
        public ActionResult<VillaDto> AddVilla([FromBody] VillaDto villaDto)
        {
            #region ModelState
            //这段代码是在没有加apicontroller attribute的时候，依旧能对dataannotation做出判断
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            //modelState example
            //if (VillaStore.villaList.FirstOrDefault(a => a.Name.ToLower() == villaDto.Name.ToLower()) !=null)
            //{
            //    ModelState.AddModelError("this is the key of custom error", "no duplicate name allowed");
            //    return BadRequest(ModelState);
            //}
            #endregion

            if (villaDto == null)
            {
                return BadRequest();
            }

            villaDto.Id = VillaStore.villaList.OrderByDescending(a => a.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDto);
            
            //return Ok(villaDto);
            return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpDelete("delete/{id}", Name = "DeleteVilla")]
        public IActionResult DeleteVilla(int id)
        {
            if (id ==0)
            {
                return BadRequest();
            }
            VillaDto villa = VillaStore.villaList.FirstOrDefault(a => a.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="villaDto"></param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpPut("update/{id}")]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            if (id!= villaDto.Id || villaDto == null)
            {
                return BadRequest();
            }
            VillaDto updatedVilla =  VillaStore.villaList.FirstOrDefault(a => a.Id == id);
            if (updatedVilla == null)
            {
                return NotFound();
            }
            updatedVilla.Name = villaDto.Name;
            updatedVilla.Sqft = villaDto.Sqft;
            updatedVilla.Occupancy = villaDto.Occupancy;
            return NoContent();
        }


        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpPatch("patch/{id}")]
        public IActionResult PatchVilla(int id, JsonPatchDocument<VillaDto> villaDto)
        {
            if (villaDto == null || id==0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(a => a.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            villaDto.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
